namespace CarbonCore.Utils.IoC
{
    using System;

    using CarbonCore.Utils.Contracts.IoC;

    // Quick configuration for the basic needs
    public class CarbonQuickBinding : ICarbonQuickBinding
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Type Interface { get; set; }
        public Type Implementation { get; private set; }

        public object Instance { get; private set; }

        public bool IsSingleton { get; private set; }
        public bool IsAlwaysUnique { get; private set; }

        public ICarbonQuickBinding For<T>()
        {
            this.Interface = typeof(T);
            return this;
        }

        public ICarbonQuickBinding Use<T>()
        {
            this.Implementation = typeof(T);
            return this;
        }

        public ICarbonQuickBinding Use<T>(T instance)
        {
            this.Instance = instance;
            return this;
        }

        public ICarbonQuickBinding Singleton()
        {
            this.IsSingleton = true;
            return this;
        }

        public ICarbonQuickBinding AlwaysUnique()
        {
            this.IsAlwaysUnique = true;
            return this;
        }
    }
}
