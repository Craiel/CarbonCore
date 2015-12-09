namespace CarbonCore.Utils.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.Utils.Contracts.IoC;

    public static class CarbonContainerBuilder
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ICarbonContainer BuildQuick<T>()
            where T : ICarbonQuickModule
        {
            // Create a new Quick Container
            var container = new CarbonQuickContainer();

            // Scan the module for dependencies
            IEnumerable<Type> dependencies = ScanModules(typeof(T), typeof(UtilsCompatModule));
            foreach (Type moduleType in dependencies)
            {
                container.RegisterModule((ICarbonQuickModule)Activator.CreateInstance(moduleType));
            }

            return container;
        }

        public static IEnumerable<Type> ScanModules(Type moduleType, params Type[] baseDependencies)
        {
            // Utils module is added by default
            IDictionary<Type, int> dependencies = new Dictionary<Type, int>();
            foreach (Type dependency in baseDependencies)
            {
                dependencies.Add(dependency, 0);
            }

            // We use a stack so module load happens in the registration order
            var pending = new Stack<Type>();
            pending.Push(moduleType);
            int currentOrder = 0;
            while (pending.Count > 0)
            {
                currentOrder += 1000;
                Type key = pending.Pop();
                System.Diagnostics.Debug.WriteLine(string.Format("[IoC] Initializing Module: {0}", key));

                if (dependencies.ContainsKey(key))
                {
                    currentOrder = dependencies[key] - 1;
                }
                else
                {
                    dependencies.Add(key, currentOrder--);
                }

                object[] dependencyAttributes = key.GetCustomAttributes(typeof(DependsOnModuleAttribute), true);
                foreach (DependsOnModuleAttribute attribute in dependencyAttributes)
                {
                    if (dependencies.ContainsKey(attribute.Type))
                    {
                        continue;
                    }

                    dependencies.Add(attribute.Type, currentOrder--);
                    pending.Push(attribute.Type);
                }
            }
            
            return from entry in dependencies orderby entry.Value ascending select entry.Key;
        }
    }
}
