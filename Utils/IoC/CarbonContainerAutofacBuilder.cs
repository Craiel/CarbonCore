namespace CarbonCore.Utils.IoC
{
    using System;
    using System.Collections.Generic;

    using Autofac;
    using Autofac.Builder;
    using Autofac.Core;

    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IoC;

    public class CarbonContainerAutofacBuilder : CarbonContainerBuilder
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ICarbonContainer Build<T>(ContainerBuildOptions options = ContainerBuildOptions.None)
            where T : IModule
        {
            // Scan the module for dependencies
            IEnumerable<Type> dependencies = this.ScanModules(typeof(T), typeof(UtilsModule));

            var builder = new ContainerBuilder();
            foreach (Type moduleType in dependencies)
            {
                builder.RegisterModule((IModule)Activator.CreateInstance(moduleType));
            }

            return new CarbonContainerAutofac(builder.Build());
        }
    }
}
