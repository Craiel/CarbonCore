namespace CarbonCore.Utils.IoC
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Contracts.IoC;

    public class CarbonQuickModule : ICarbonQuickModule
    {
        private readonly List<ICarbonQuickBinding> bindings;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonQuickModule()
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

        public IList<ICarbonQuickBinding> GetQuickBindings()
        {
            return new List<ICarbonQuickBinding>(this.bindings);
        }
    }
}
