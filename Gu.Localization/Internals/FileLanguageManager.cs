namespace Gu.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;

    [DebuggerDisplay(@"Assembly: {Assembly.GetName().Name} Languages: {string.Join("", "", Languages.Select(x=>x.TwoLetterISOLanguageName))}")]
    internal sealed class FileLanguageManager : ILanguageManager
    {
        private readonly Dictionary<CultureInfo, ResourceSet> _culturesAndResourceSets = new Dictionary<CultureInfo, ResourceSet>();

        private bool _disposed;

        internal FileLanguageManager(Type typeInAssembly)
            : this(typeInAssembly.Assembly)
        {
        }

        internal FileLanguageManager(Assembly assembly)
        {
            Assembly = assembly;
            var resourceFiles = GetResourceFiles(assembly);
            TryAddResource(assembly, CultureInfo.InvariantCulture);
            foreach (var file in resourceFiles)
            {
                var resourceAssy = Assembly.LoadFile(file.FullName);
                TryAddResource(resourceAssy,resourceAssy.GetName().CultureInfo);
            }
            Languages = _culturesAndResourceSets.Keys.Where(x => !string.IsNullOrEmpty(x.Name))
                                                .ToArray();
        }

        public Assembly Assembly { get; }

        public IReadOnlyList<CultureInfo> Languages { get; }

        public string Translate(CultureInfo culture, string key)
        {
            VerifyDisposed();
            ResourceSet set;
            if (_culturesAndResourceSets.TryGetValue(culture, out set))
            {
                return set.GetString(key);
            }
            if (!Equals(culture, CultureInfo.InvariantCulture))
            {
                return Translate(CultureInfo.InvariantCulture, key);
            }
            return string.Format(Properties.Resources.MissingTranslationFormat, key);
        }

        /// <summary>
        /// Make the class sealed when using this. 
        /// Call VerifyDisposed at the start of all public methods
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            foreach (var culturesAndResourceSet in _culturesAndResourceSets)
            {
                culturesAndResourceSet.Value.Dispose();
            }
            _culturesAndResourceSets.Clear();
            // Dispose some stuff now
        }

        private bool TryAddResource(Assembly resourceAssy, CultureInfo culture)
        {
            var manifestResourceNames = resourceAssy.GetManifestResourceNames();
            var resourceName =
                manifestResourceNames.SingleOrDefault(s => !s.EndsWith(".g.resources") && s.EndsWith(".resources"));
            if (resourceName == null)
            {
                return false;
            }
            using (var stream = resourceAssy.GetManifestResourceStream(resourceName))
            {
                var resourceSet = new ResourceSet(stream);
                _culturesAndResourceSets[culture] = resourceSet;
            }
            return true;
        }

        private void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(
                    GetType()
                        .FullName);
            }
        }

        private static IReadOnlyList<FileInfo> GetResourceFiles(Assembly assembly)
        {
            var directory = GetDirectory(assembly);
            var searchPattern = System.IO.Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name) + ".resources.dll";
            var resourceFiles = Directory.EnumerateFiles(directory.FullName, searchPattern, SearchOption.AllDirectories)
                                         .Select(x => new FileInfo(x))
                                         .ToArray();
            return resourceFiles;
        }

        private static DirectoryInfo GetDirectory(Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new Uri(codeBase, UriKind.Absolute);
            var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(uri.LocalPath));
            if (Directory.Exists(directory.FullName))
            {
                return directory;
            }
            return new DirectoryInfo(assembly.Location);
        }
    }
}