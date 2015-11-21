namespace Gu.Localization
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Resources;
    using Gu.Localization.Internals;

    [DebuggerDisplay(@"Assembly: {Assembly.GetName().Name} Languages: {string.Join("", "", Languages.Select(x=>x.TwoLetterISOLanguageName))}")]
    internal class FileLanguageManager : ILanguageManager
    {
        private static readonly ConcurrentDictionary<Assembly, ILanguageManager> Cache = new ConcurrentDictionary<Assembly, ILanguageManager>(AssemblyComparer.Default);
        private readonly Dictionary<CultureInfo, ResourceSet> _culturesAndResourceSets = new Dictionary<CultureInfo, ResourceSet>(CultureInfoComparer.Default);
        private bool _disposed;

        static FileLanguageManager()
        {
            if (AppDomain.CurrentDomain.IsDesignTime())
            {
                Factory = new DesigntimeLanguageManagerFactory();
            }
            else if (AppDomain.CurrentDomain.IsDebug())
            {
                Factory = new DebugLanguageManagerFactory();
            }
            else
            {
                Factory = new FileLanguageManagerFactory();
            }
        }

        protected FileLanguageManager(Assembly assembly, IReadOnlyList<FileInfo> files)
        {
            Assembly = assembly;
            ResourceFiles = files;
            TryAddResource(assembly, CultureInfo.InvariantCulture);
            foreach (var file in ResourceFiles)
            {
                var resourceAssy = Assembly.Load(File.ReadAllBytes(file.FullName));
                TryAddResource(resourceAssy, resourceAssy.GetName().CultureInfo);
            }
            Languages = _culturesAndResourceSets.Keys.Where(x => !string.IsNullOrEmpty(x.Name))
                                                .ToArray();
            Translator.AllCulturesInner.UnionWith(Languages);
        }

        public static ILanguageManagerFactory Factory { get; }

        public IReadOnlyList<FileInfo> ResourceFiles { get; }

        public Assembly Assembly { get; }

        public IReadOnlyList<CultureInfo> Languages { get; }

        public string Translate(CultureInfo culture, string key)
        {
            VerifyDisposed();
            ResourceSet set;
            if (_culturesAndResourceSets.TryGetValue(culture, out set))
            {
                var translated = set.GetString(key);
                if (!string.IsNullOrEmpty(translated))
                {
                    return translated;
                }
            }

            if (!Equals(culture, CultureInfo.InvariantCulture))
            {
                var translated = Translate(CultureInfo.InvariantCulture, key);
                if (!string.IsNullOrEmpty(translated))
                {
                    return translated;
                }
            }
            return string.Format(Properties.Resources.MissingTranslationFormat, key);
        }

        /// <summary>
        /// Dispose(true); //I am calling you from Dispose, it's safe
        /// GC.SuppressFinalize(this); //Hey, GC: don't bother calling finalize later
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;
            Dispose(true);
        }

        /// <summary>
        /// Protected implementation of Dispose pattern. 
        /// </summary>
        /// <param name="disposing">true: safe to free managed resources</param>
        protected virtual void Dispose(bool disposing)
        {

            if (disposing)
            {
                foreach (var culturesAndResourceSet in _culturesAndResourceSets)
                {
                    culturesAndResourceSet.Value.Dispose();
                }
                _culturesAndResourceSets.Clear();
            }
        }

        protected void VerifyDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
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
            using (var reader = new ResourceReader(resourceAssy.GetManifestResourceStream(resourceName)))
            {
                var resourceSet = new ResourceSet(reader);
                _culturesAndResourceSets[culture] = resourceSet;
            }
            return true;
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

        internal class FileLanguageManagerFactory : ILanguageManagerFactory
        {
            private static readonly ConcurrentDictionary<Assembly, FileInfo> FileCache = new ConcurrentDictionary<Assembly, FileInfo>(AssemblyComparer.Default);
            protected FileInfo DesignTimeFile(Assembly assembly)
            {
                return FileCache.GetOrAdd(assembly, CreateDeigntimeFileInfo);
            }

            public ILanguageManager GetOrCreate(Type typeInAssembly)
            {
                return GetOrCreate(typeInAssembly.Assembly);
            }

            public ILanguageManager GetOrCreate(Assembly assembly)
            {
                ILanguageManager result;
                if (Cache.TryGetValue(assembly, out result))
                {
                    return result;
                }
                var resourceFiles = GetResourceFiles(assembly);
                result = Cache.GetOrAdd(assembly, a => new FileLanguageManager(a, resourceFiles));
                Translator.AllCulturesInner.UnionWith(result.Languages);
                var assemblyAndLanguages = new AssemblyAndLanguages(assembly, result.Languages, resourceFiles);
                Translator.AllAssembliesAndLanguagesInner.Add(assemblyAndLanguages);
                return result;
            }

            protected virtual IReadOnlyList<FileInfo> GetResourceFiles(Assembly assembly)
            {
                var directory = GetDirectory(assembly);
                var searchPattern = $"{System.IO.Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name)}.resources.dll";
                var resourceFiles = Directory.EnumerateFiles(directory.FullName, searchPattern, SearchOption.AllDirectories)
                                             .Select(x => new FileInfo(x))
                                             .ToArray();
                return resourceFiles;
            }

            private FileInfo CreateDeigntimeFileInfo(Assembly assembly)
            {
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var designtimeDirectory = System.IO.Path.Combine(localAppData, GetType().Assembly.ManifestModule.Name);
                try
                {
                    if (!Directory.Exists(designtimeDirectory))
                    {
                        Directory.CreateDirectory(designtimeDirectory);
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
                return new FileInfo(System.IO.Path.Combine(designtimeDirectory, assembly.ManifestModule.Name + ".designtime"));
            }
        }

        private class DesigntimeLanguageManagerFactory : FileLanguageManagerFactory
        {
            private static readonly FileInfo[] EmptyFiles = new FileInfo[0];

            protected override IReadOnlyList<FileInfo> GetResourceFiles(Assembly assembly)
            {
                var designTimeFile = DesignTimeFile(assembly);
                if (File.Exists(designTimeFile.FullName))
                {
                    try
                    {
                        var resourceFiles = File.ReadAllLines(designTimeFile.FullName).Select(x => new FileInfo(x)).ToArray();
                        return resourceFiles;
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch
                    {
                    }
                }
                return EmptyFiles;
            }
        }

        private class DebugLanguageManagerFactory : FileLanguageManagerFactory
        {
            protected override IReadOnlyList<FileInfo> GetResourceFiles(Assembly assembly)
            {
                var resourceFiles = base.GetResourceFiles(assembly);
                try
                {
                    var designTimeFile = DesignTimeFile(assembly);
                    File.Delete(designTimeFile.FullName);
                    if (resourceFiles.Any())
                    {
                        File.WriteAllLines(designTimeFile.FullName, resourceFiles.Select(x => x.FullName));
                    }
                    return resourceFiles;
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch
                {
                }
                return resourceFiles;
            }
        }
    }
}