namespace Gu.Localization.Analyzers
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeActions;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseResourceFix))]
    internal class UseResourceFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            UseNameOfAnalyzer.DiagnosticId);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                                   .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is LiteralExpressionSyntax literal)
                {
                    // <data name="Key" xml:space="preserve">
                    //   <value>Value</value>
                    // </data>
                    var resx = Path.Combine(Path.GetDirectoryName(context.Document.Project.FilePath), "Properties\\Resources.resx");
                    if (File.Exists(resx) &&
                        TryGetKey(literal.Token.ValueText, out var key))
                    {
                        var xDocument = XDocument.Load(resx);
                        if (xDocument.Root
                            .Descendants("data")
                            .Any(x => x.Attribute("name")?.Value == key))
                        {
                            context.RegisterCodeFix(
                                CodeAction.Create(
                                    "Use existing resource in Translator.Translate",
                                    _ => Task.FromResult(
                                        context.Document.WithSyntaxRoot(
                                            syntaxRoot.ReplaceNode(
                                                literal,
                                                SyntaxFactory.ParseExpression($"Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.{literal.Token.ValueText}))")
                                                             .WithSimplifiedNames()))),
                                    "Move to resource and use Translator.Translate"),
                                diagnostic);
                            context.RegisterCodeFix(
                                CodeAction.Create(
                                    "Use existing resource.",
                                    _ => Task.FromResult(
                                        context.Document.WithSyntaxRoot(
                                            syntaxRoot.ReplaceNode(
                                                literal,
                                                SyntaxFactory.ParseExpression($"Properties.Resources.{literal.Token.ValueText}")
                                                             .WithSimplifiedNames()))),
                                    "Use existing resource."),
                                diagnostic);
                        }
                        else
                        {
                            var xElement = new XElement("data");
                            xElement.Add(new XAttribute("name", key));
                            xElement.Add(new XAttribute(XName.Get("space", "xml"), "preserve"));
                            xElement.Add(new XElement("value", literal.Token.ValueText));
                            xDocument.Root.Add(xElement);
                            using (var stream = File.OpenWrite(resx))
                            {
                                xDocument.Save(stream);
                            }

                            var designer = Path.Combine(Path.GetDirectoryName(context.Document.Project.FilePath), "Properties\\Resources.Designer.cs");
                            if (File.Exists(designer) &&
                                File.ReadAllLines(designer).ToList() is List<string> lines &&
                                lines.Count > 3)
                            {
                                // Adding a temp key so that we don't have a build error until next gen.
                                // internal static string Key => ResourceManager.GetString("Key", resourceCulture);
                                lines.Insert(lines.Count - 2, $"        internal static string {key} => ResourceManager.GetString(\"{key}\", resourceCulture);");
                                File.WriteAllLines(designer, lines);

                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        "Move to resources and use Translator.Translate.",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    literal,
                                                    SyntaxFactory.ParseExpression($"Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.{literal.Token.ValueText}))")
                                                                 .WithSimplifiedNames()))),
                                        "Move to resources and use Translator.Translate."),
                                    diagnostic);

                                context.RegisterCodeFix(
                                    CodeAction.Create(
                                        "Move to resources.",
                                        _ => Task.FromResult(
                                            context.Document.WithSyntaxRoot(
                                                syntaxRoot.ReplaceNode(
                                                    literal,
                                                    SyntaxFactory.ParseExpression($"Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.{literal.Token.ValueText}))")
                                                                 .WithSimplifiedNames()))),
                                        "Move to resources."),
                                    diagnostic);
                            }
                        }
                    }
                }
            }
        }

        private static bool TryGetKey(string text, out string key)
        {
            key = Regex.Replace(text, "{(?<n>\\d+)}", x => $"__{x.Groups["n"].Value}__")
                       .Replace(" ", "_")
                       .Replace(".", "_");

            if (char.IsDigit(key[0]))
            {
                key = "_" + key;
            }

            return SyntaxFacts.IsValidIdentifier(key);
        }
    }
}
