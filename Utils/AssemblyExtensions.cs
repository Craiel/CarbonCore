namespace CarbonCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    using CarbonCore.Utils.IO;

    public static class AssemblyExtensions
    {
        private const string Delimiter = ".";

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Version GetVersion(Type type)
        {
            return Assembly.GetAssembly(type).GetVersion();
        }

        public static Version GetVersion(this Assembly assembly)
        {
            if (assembly.IsDynamic)
            {
                return null;
            }

            return assembly.GetName().Version;
        }

        public static CarbonFile GetAssemblyFile(this Assembly assembly)
        {
            // Dynamic assemblies have no location
            if (assembly.IsDynamic)
            {
                return null;
            }

            return new CarbonFile(assembly.Location);
        }

        public static CarbonDirectory GetDirectory(this Assembly assembly)
        {
            CarbonFile file = assembly.GetAssemblyFile();
            return file == null ? null : file.GetDirectory();
        }

        public static IList<CarbonFile> GetLoadedAssemblyFiles(AppDomain domain = null)
        {
            if (domain == null)
            {
                domain = AppDomain.CurrentDomain;
            }

            Assembly[] assemblies = domain.GetAssemblies();
            if (assemblies.Length <= 0)
            {
                System.Diagnostics.Trace.TraceWarning("Could not locate any assemblies in domain {0}", domain);
                return null;
            }

            IList<CarbonFile> results = new List<CarbonFile>(assemblies.Length);
            foreach (Assembly assembly in assemblies)
            {
                CarbonFile file = assembly.GetAssemblyFile();
                if (file == null)
                {
                    continue;
                }

                results.Add(file);
            }

            return results;
        }

        public static IList<CarbonFile> ExtractResources(this Assembly assembly, CarbonDirectory target, string path = null, bool replace = true)
        {
            CarbonDirectory location = assembly.GetDirectory();
            System.Diagnostics.Trace.Assert(location.Exists);

            string[] resources = assembly.GetManifestResourceNames();
            if (resources.Length <= 0)
            {
                System.Diagnostics.Trace.TraceWarning("No resource to extract for {0} with path {1}", assembly, path);
                return null;
            }

            string assemblyRoot = assembly.GetName().Name + '.';
            target.Create();
            IList<CarbonFile> results = new List<CarbonFile>();
            foreach (string resource in resources)
            {
                string localizedResourcePath = resource.Replace(assemblyRoot, string.Empty);

                // Ignore internal resources
                if (localizedResourcePath.StartsWith("g."))
                {
                    continue;
                }

                // Filter out resources we don't want to extract
                if (string.IsNullOrEmpty(path) || !localizedResourcePath.StartsWith(path))
                {
                    continue;
                }

                // Files with *.*.*.ext are translated to *\*\*.ext
                var internalFile = new CarbonFile(resource);
                string internalFilePath = internalFile.FileNameWithoutExtension.Replace(Delimiter, System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture));
                internalFile = new CarbonFile(internalFilePath + internalFile.Extension);

                var targetFile = target.ToFile(internalFile);
                targetFile.GetDirectory().Create();
                using (var stream = assembly.GetManifestResourceStream(resource))
                {
                    System.Diagnostics.Trace.Assert(stream != null);
                    using (var writer = targetFile.OpenCreate())
                    {
                        stream.CopyTo(writer);
                    }
                }

                results.Add(targetFile);
            }

            return results;
        }
    }
}
