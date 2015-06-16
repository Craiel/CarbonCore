namespace CarbonCore.Utils.IoC
{
    using System;

    using Autofac;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    public class FactoryAutofac : IFactory
    {
        private readonly IComponentContext container;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FactoryAutofac(IComponentContext container)
        {
            this.container = container;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Resolve<T>()
        {
            return this.container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return this.container.Resolve(type);
        }
    }
}
