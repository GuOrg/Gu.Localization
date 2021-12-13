namespace Gu.Localization.Analyzers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using System.Xml.Linq;
    using Gu.Roslyn.AnalyzerExtensions;
    using Microsoft.CodeAnalysis;

    internal class ResxFile
    {
        private static readonly ConcurrentDictionary<string, ResxFile?> Cache = new();

        private readonly object gate = new();

        private DateTime lastWriteTimeUtc;

        private ResxFile(string fileName, XDocument document, Encoding encoding, DateTime lastWriteTimeUtc)
        {
            this.lastWriteTimeUtc = lastWriteTimeUtc;
            this.Document = document;
            this.FileName = fileName;
            this.Encoding = encoding;
        }

        internal string FileName { get; }

        internal XDocument Document { get; }

        internal Encoding Encoding { get; }

        internal static bool TryGetDefault(INamedTypeSymbol resourcesType, [NotNullWhen(true)] out ResxFile? resxFile)
        {
            resxFile = null;
            if (resourcesType != null &&
                resourcesType.Name == "Resources" &&
                resourcesType.TryFindProperty("ResourceManager", out _) &&
                resourcesType.DeclaringSyntaxReferences.TrySingle(out var reference) &&
                reference.SyntaxTree?.FilePath is { } resourcesFileName &&
                resourcesFileName.Replace("Resources.Designer.cs", "Resources.resx") is { } fileName &&
                File.Exists(fileName))
            {
                resxFile = Cache.AddOrUpdate(fileName, s => Create(s), (s, file) => Update(s, file));
            }

            return resxFile != null;
        }

        internal static bool TryGetString(XElement data, [NotNullWhen(true)] out string? value)
        {
            if (data.Element("value") is { } valueElement)
            {
                value = valueElement.Value;
                return true;
            }

            value = null;
            return false;
        }

        internal static bool TryGetName(XElement data, [NotNullWhen(true)] out string? value)
        {
            if (data.Attribute("name") is { } attribute)
            {
                value = attribute.Value;
                return true;
            }

            value = null;
            return false;
        }

        internal bool TryGetString(string key, [NotNullWhen(true)] out string? value)
        {
            value = null;
            return this.TryGetDataElement(key, out var data) &&
                   TryGetString(data, out value);
        }

        internal void RenameKey(string oldName, string newName)
        {
            lock (this.gate)
            {
                if (this.TryGetDataElement(oldName, out var data))
                {
                    if (data.Attribute("name") is { } attribute &&
                        attribute.Value == oldName)
                    {
                        attribute.Value = newName;
                        this.Save();
                    }
                }
            }
        }

        internal void Add(string key, string text)
        {
            lock (this.gate)
            {
                if (!this.TryGetDataElement(key, out _))
                {
                    // <data name="Key" xml:space="preserve">
                    //   <value>Value</value>
                    // </data>
                    var xElement = new XElement("data");
                    xElement.Add(new XAttribute("name", key));
                    xElement.Add(new XAttribute(XNamespace.Xml + "space", "preserve"));
                    xElement.Add(new XElement("value", text));
                    //// ReSharper disable once PossibleNullReferenceException
                    this.Document.Root!.Add(xElement);
                    this.Save();
                }
            }
        }

        internal IEnumerable<ResxFile> CultureSpecific()
        {
            foreach (var cultureResx in Directory.EnumerateFiles(Path.GetDirectoryName(this.FileName)!, $"{Path.GetFileNameWithoutExtension(this.FileName)}.*.resx", SearchOption.TopDirectoryOnly))
            {
                var resxFile = Cache.AddOrUpdate(cultureResx, s => Create(s), (s, file) => Update(s, file));
                if (resxFile != null)
                {
                    yield return resxFile;
                }
            }
        }

        private static ResxFile? Update(string fileName, ResxFile? old)
        {
            if (old is null ||
                old.lastWriteTimeUtc != new FileInfo(fileName).LastWriteTimeUtc)
            {
                return Create(fileName);
            }

            return old;
        }

        private static ResxFile? Create(string fileName)
        {
            using var reader = new StreamReader(File.OpenRead(fileName), Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            var document = XDocument.Load(reader);
            if (document.Root != null)
            {
                return new ResxFile(fileName, document, reader.CurrentEncoding, File.GetLastWriteTimeUtc(fileName));
            }

            return null;
        }

        private void Save()
        {
            using var writer = new StreamWriter(File.Open(this.FileName, FileMode.Create, FileAccess.Write), this.Encoding);
            this.Document.Save(writer);
            this.lastWriteTimeUtc = File.GetLastWriteTimeUtc(this.FileName);
        }

        private bool TryGetDataElement(string key, [NotNullWhen(true)] out XElement? element)
        {
            if (this.Document.Root is { } root)
            {
                foreach (var candidate in root.Elements("data"))
                {
                    if (TryGetName(candidate, out var name) &&
                        name == key)
                    {
                        element = candidate;
                        return true;
                    }
                }
            }

            element = null;
            return false;
        }
    }
}
