namespace CarbonCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using CarbonCore.Utils.IO;

    public static class AssemblyExtensions
    {
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
            if (file == null)
            {
                return null;
            }

            return new CarbonDirectory(file);
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
    }
}
