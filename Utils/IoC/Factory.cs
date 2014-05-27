namespace CarbonCore.Utils.IoC
{
    using Autofac;

    using CarbonCore.Utils.Contracts.IoC;

    public class Factory : IFactory
    {
        private readonly IComponentContext container;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Factory(IComponentContext container)
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
    }
}
