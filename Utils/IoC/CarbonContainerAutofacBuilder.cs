namespace CarbonCore.Utils.IoC
{
    using System;
    using System.Collections.Generic;

    using Autofac;

    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IoC;

    public static class CarbonContainerAutofacBuilder
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ICarbonContainer Build<T>()
            where T : ICarbonQuickModule
        {
            // Scan the module for dependencies
            IEnumerable<Type> dependencies = CarbonContainerBuilder.ScanModules(typeof(T), typeof(UtilsModule));

            var builder = new ContainerBuilder();
            foreach (Type moduleType in dependencies)
            {
                var quickModule = (ICarbonQuickModule)Activator.CreateInstance(moduleType);
                var autofacModule = new CarbonModuleAutofac(quickModule);
                builder.RegisterModule(autofacModule);
            }

            return new CarbonContainerAutofac(builder.Build());
        }
    }
}
