namespace Gu.Localization.Analyzers
{
    using System.Collections.Immutable;
    using System.Composition;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Gu.Localization.Analyzers.FixAll;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseResourceFix))]
    internal class UseResourceFix : DocumentEditorCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds { get; } = ImmutableArray.Create(
            UseNameOfAnalyzer.DiagnosticId);

        protected override async Task RegisterCodeFixesAsync(DocumentEditorCodeFixContext context)
        {
            var syntaxRoot = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
                                                   .ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                if (syntaxRoot.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is LiteralExpressionSyntax literal)
                {
                    var resx = Path.Combine(Path.GetDirectoryName(context.Document.Project.FilePath), "Properties\\Resources.resx");
                    if (File.Exists(resx))
                    {
                        // <data name="Key" xml:space="preserve">
                        //   <value>Value</value>
                        // </data>
                        var key = Key(literal.Token.ValueText);
                        var xDocument = XDocument.Load(resx);
                        if (xDocument.Root
                            .Descendants("data")
                            .Any(x => x.Attribute("name")?.Value == key))
                        {
                            context.RegisterCodeFix(
                                "Use existing resource in Translator.Translate",
                                (e, c) => e.ReplaceNode(
                                    literal,
                                    SyntaxFactory
                                        .ParseExpression(
                                            $"Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.{literal.Token.ValueText}))")
                                        .WithSimplifiedNames()),
                                "Move to resource and use Translator.Translate",
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
                            if (File.Exists(designer))
                            {
                                // Adding a temp key so that we don't have a build error until next gen.
                                // internal static string Key => ResourceManager.GetString("Key", resourceCulture);
                                var lines = File.ReadAllLines(designer).ToList();
                                lines.Insert(lines.Count - 2, $"        internal static string {key} => ResourceManager.GetString(\"{key}\", resourceCulture);");
                                File.WriteAllLines(
                                    designer,
                                    lines);
                            }

                            context.RegisterCodeFix(
                                "Move to resource and use Translator.Translate",
                                (e, c) => e.ReplaceNode(
                                    literal,
                                    SyntaxFactory.ParseExpression($"Gu.Localization.Translator.Translate(Properties.Resources.ResourceManager, nameof(Properties.Resources.{literal.Token.ValueText}))")
                                        .WithSimplifiedNames()),
                                "Move to resource and use Translator.Translate",
                                diagnostic);
                        }
                    }
                }
            }
        }

        private static string Key(string text)
        {
            return Regex.Replace(text, "{(?<n>\\d+)}", x => $"__{x.Groups["n"].Value}__")
                .Replace(" ", "_")
                .Replace(".", "_");
        }
    }
}
