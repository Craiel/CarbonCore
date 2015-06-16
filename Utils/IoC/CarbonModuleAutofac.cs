namespace CarbonCore.Utils.IoC
{
    using System.IO;

    using Autofac;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    public class CarbonModuleAutofac : Module
    {
        private readonly ICarbonQuickModule inner;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonModuleAutofac(ICarbonQuickModule source)
        {
            this.inner = source;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ICarbonQuickBinding For<T>()
        {
            return this.inner.For<T>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register our quick bindings
            foreach (ICarbonQuickBinding binding in this.inner.GetQuickBindings())
            {
                if (binding.Interface == null || binding.Implementation == null)
                {
                    throw new InvalidDataException("QuickBinding is invalid, missing interface or implementation");
                }

                var registration = builder.RegisterType(binding.Implementation).As(binding.Interface);
                if (binding.IsSingleton)
                {
                    registration.SingleInstance();
                }
            }
        }
    }
}
