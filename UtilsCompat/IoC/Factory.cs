namespace CarbonCore.Utils.Compat.IoC
{
    using System;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    public class Factory : IFactory
    {
        private readonly ICarbonContainer container;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Factory(ICarbonContainer container)
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
