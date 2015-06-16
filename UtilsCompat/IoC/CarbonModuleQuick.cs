namespace CarbonCore.Utils.Compat.IoC
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    public class CarbonModuleQuick
    {
        private readonly List<ICarbonQuickBinding> bindings;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonModuleQuick()
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
    }
}
