namespace CarbonCore.Utils.Edge
{
    using System;
    using System.Reflection;
    
    public static class AssemblyExtensions
    {
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
    }
}
