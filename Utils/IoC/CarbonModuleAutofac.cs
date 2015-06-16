namespace CarbonCore.Utils.IoC
{
    using System.Collections.Generic;
    using System.IO;

    using Autofac;

    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IoC;

    public abstract class CarbonModuleAutofac : Module, ICarbonModule
    {
        private readonly List<ICarbonQuickBinding> bindings;
        
        protected CarbonModuleAutofac()
        {
            this.bindings = new List<ICarbonQuickBinding>();
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ICarbonQuickBinding For<T>()
        {
            var binding = new CarbonQuickBinding().For<T>();
            this.bindings.Add(binding);
            return binding;
        }

        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register our quick bindings
            foreach (ICarbonQuickBinding binding in this.bindings)
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
