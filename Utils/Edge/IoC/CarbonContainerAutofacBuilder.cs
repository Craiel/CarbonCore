namespace CarbonCore.Utils.Edge.IoC
{
    using System;
    using System.Collections.Generic;

    using Autofac;

    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IoC;

    public static class CarbonContainerAutofacBuilder
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ICarbonContainer Build<T>()
            where T : ICarbonQuickModule
        {
            // Scan the module for dependencies
            IEnumerable<Type> dependencies = CarbonContainerBuilder.ScanModules(
                typeof(T),
                typeof(UtilsModule),
                typeof(UtilsEdgeModule));

            var builder = new ContainerBuilder();
            foreach (Type moduleType in dependencies)
            {
                var quickModule = (ICarbonQuickModule)Activator.CreateInstance(moduleType);
                var autofacModule = new CarbonModuleAutofac(quickModule);
                builder.RegisterModule(autofacModule);
            }

            CarbonContainerAutofac container = new CarbonContainerAutofac(builder.Build());

            // Inject the container as ICarbonContainer for factory resolve
            builder = new ContainerBuilder();
            builder.RegisterInstance(container).As<ICarbonContainer>();
            builder.Update(container.Inner);

            return container;
        }
    }
}
