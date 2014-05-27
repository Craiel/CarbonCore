namespace CarbonCore.Utils.IoC
{
    using System;

    using CarbonCore.Utils.Contracts.IoC;

    // Quick configuration for the basic needs
    internal class CarbonQuickBinding : ICarbonQuickBinding
    {
        internal Type Interface { get; set; }
        internal Type Implementation { get; set; }
        internal bool IsSingleton { get; set; }

        public ICarbonQuickBinding Use<T>()
        {
            this.Implementation = typeof(T);
            return this;
        }

        public ICarbonQuickBinding Singleton()
        {
            this.IsSingleton = true;
            return this;
        }
    }
}
