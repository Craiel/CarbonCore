namespace CarbonCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using CarbonCore.Utils.IO;

    public static class AssemblyExtensions
    {
        private const char ResourceDelimiter = '.';

        private const string ResourceLocalIndicator = @"__.";

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
            if (!string.IsNullOrEmpty(path))
            {
                assemblyRoot += path;
            }

            target.Create();
            IList<CarbonFile> results = new List<CarbonFile>();
            foreach (string resource in resources)
            {
                if (!resource.StartsWith(assemblyRoot))
                {
                    continue;
                }

                string localizedResourcePath = resource.Replace(assemblyRoot, string.Empty).TrimStart(ResourceDelimiter);

                // Ignore internal resources
                if (localizedResourcePath.StartsWith("g."))
                {
                    continue;
                }

                // Check if we have a local indicator in the resource path
                string localizedResourcePathSuffix = string.Empty;
                int localIndicatorIndex = localizedResourcePath.IndexOf(ResourceLocalIndicator, StringComparison.Ordinal);
                if (localizedResourcePath.Contains(ResourceLocalIndicator))
                {
                    localizedResourcePathSuffix = localizedResourcePath.Substring(localIndicatorIndex + ResourceLocalIndicator.Length, localizedResourcePath.Length - localIndicatorIndex - ResourceLocalIndicator.Length);
                    localizedResourcePath = string.Empty;
                }

                // Files with *.*.*.ext are translated to *\*\*.ext
                string[] segments = localizedResourcePath.Split(new[] { ResourceDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                string extension = string.Empty;
                if (segments.Length < 2 || segments[segments.Length - 1].Length > 5)
                {
                    extension = string.Empty;
                }
                else if (segments.Length > 0)
                {
                    extension = segments[segments.Length - 1];
                    segments = segments.Take(segments.Length - 1).ToArray();
                }

                string file = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), segments) + localizedResourcePathSuffix;
                if (!string.IsNullOrEmpty(extension))
                {
                    file = string.Concat(file, ".", extension);
                }

                var targetFile = target.ToFile(file);

                targetFile.GetDirectory().Create();
                using (var stream = assembly.GetManifestResourceStream(resource))
                {
                    System.Diagnostics.Trace.Assert(stream != null);
                    using (var writer = targetFile.OpenCreate())
                    {
                        stream.CopyTo(writer);
                        System.Diagnostics.Trace.TraceInformation("Extracted {0} to {1} ({2})", resource, targetFile, targetFile.Size);
                    }
                }

                results.Add(targetFile);
            }

            return results;
        }
    }
}
