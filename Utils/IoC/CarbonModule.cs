namespace CarbonCore.Utils.IoC
{
    using System.Collections.Generic;
    using System.IO;

    using Autofac;

    using CarbonCore.Utils.Contracts.IoC;

    public abstract class CarbonModule : Module, ICarbonModule
    {
        private readonly List<CarbonQuickBinding> bindings;
        
        protected CarbonModule()
        {
            this.bindings = new List<CarbonQuickBinding>();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ICarbonQuickBinding For<T>()
        {
            var binding = new CarbonQuickBinding { Interface = typeof(T) };
            this.bindings.Add(binding);
            return binding;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register our quick bindings
            foreach (CarbonQuickBinding binding in this.bindings)
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
