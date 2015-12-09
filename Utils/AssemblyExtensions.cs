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
            return Assembly.GetAssembly(type).GetVersionCompat();
        }

        public static Version GetVersionCompat(this Assembly assembly)
        {
            return assembly.GetName().Version;
        }

        public static CarbonFile GetAssemblyFileCompat(this Assembly assembly)
        {
            return new CarbonFile(assembly.Location);
        }

        public static CarbonDirectory GetDirectory(this Assembly assembly)
        {
            CarbonFile file = assembly.GetAssemblyFileCompat();
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
                CarbonFile file = assembly.GetAssemblyFileCompat();
                if (file == null)
                {
                    continue;
                }

                results.Add(file);
            }

            return results;
        }

        public static string GetLocalizedResourcePath(string resourcePath, string assemblyRoot)
        {
            string localizedResourcePath = resourcePath;
            if (!string.IsNullOrEmpty(assemblyRoot))
            {
                localizedResourcePath = localizedResourcePath.Replace(assemblyRoot, string.Empty);
            }

            localizedResourcePath = localizedResourcePath.TrimStart(ResourceDelimiter);

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

            // Ignore internal resources (*.g.*)
            if (segments.Length > 0 && segments[0] == "g")
            {
                return null;
            }

            string file = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), segments) + localizedResourcePathSuffix;
            if (!string.IsNullOrEmpty(extension))
            {
                file = string.Concat(file, ".", extension);
            }

            return file;
        }

        public static byte[] LoadResource(this Assembly assembly, string resourcePath, out string resourceFileName, string assemblyRoot = "")
        {
            resourceFileName = GetLocalizedResourcePath(resourcePath, assemblyRoot);
            if (resourceFileName == null)
            {
                // The file was not valid for loading
                return null;
            }

            using (var stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (stream == null)
                {
                    System.Diagnostics.Trace.TraceError("Could not Open Resource Stream: {0}", resourcePath);
                    return null;
                }

                System.Diagnostics.Trace.Assert(stream != null);
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }

        public static IDictionary<string, byte[]> LoadResources(this Assembly assembly, string path = null)
        {
            IDictionary<string, byte[]> results = new Dictionary<string, byte[]>();

            string[] resources = assembly.GetManifestResourceNames();
            if (resources.Length <= 0)
            {
                System.Diagnostics.Trace.TraceWarning("No resource to load for {0}", assembly);
                return null;
            }

            string assemblyRoot = assembly.GetName().Name + '.';
            if (!string.IsNullOrEmpty(path))
            {
                assemblyRoot += path;
            }

            foreach (string resource in resources)
            {
                if (!resource.StartsWith(assemblyRoot))
                {
                    continue;
                }

                string file;
                byte[] data = LoadResource(assembly, resource, out file, assemblyRoot);
                if (data != null)
                {
                    results.Add(file, data);
                }
            }

            return results;
        }

        public static IList<CarbonFile> ExtractResources(this Assembly assembly, CarbonDirectory target, string path = null, bool replace = true)
        {
            CarbonDirectory location = assembly.GetDirectory();
            System.Diagnostics.Trace.Assert(location.Exists);


            target.Create();

            IDictionary<string, byte[]> resources = LoadResources(assembly, path);
            if (resources == null || resources.Count <= 0)
            {
                return null;
            }

            IList<CarbonFile> results = new List<CarbonFile>();
            foreach (string file in resources.Keys)
            {
                var targetFile = target.ToFile(file);
                targetFile.GetDirectory().Create();
                results.Add(targetFile);

                using (var writer = targetFile.OpenCreate())
                {
                    writer.Write(resources[file], 0, resources[file].Length);
                    System.Diagnostics.Trace.TraceInformation("Extracted {0} ({1})", targetFile, targetFile.Size);
                }
            }
            
            return results;
        }
    }
}
