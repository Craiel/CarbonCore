namespace CarbonCore.Utils.Compat.IoC
{
    using System;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    public abstract class Factory : IFactory
    {
        private readonly ICarbonContainer container;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected Factory(ICarbonContainer container)
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
