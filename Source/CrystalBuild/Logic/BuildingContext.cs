namespace CarbonCore.CrystalBuild.Logic
{
    using System.Collections.Generic;

    using CarbonCore.CrystalBuild.Contracts;
    using CarbonCore.Utils.IO;

    public class BuildingContext<T> : IProcessingContext
        where T : BuildingCache
    {
        private readonly List<string> warnings;

        private readonly List<string> errors;
 
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BuildingContext(T cache)
        {
            this.warnings = new List<string>();
            this.errors = new List<string>();

            this.Cache = cache;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }
        
        public T Cache { get; private set; }
        
        public CarbonDirectory Root { get; set; }
        
        public IReadOnlyCollection<string> Warnings => this.warnings.AsReadOnly();

        public IReadOnlyCollection<string> Errors => this.errors.AsReadOnly();

        public void AddWarning(string message, params object[] args)
        {
            this.warnings.Add(args != null ? string.Format(message, args) : message);
        }

        public void AddError(string message, params object[] args)
        {
            this.errors.Add(args != null ? string.Format(message, args) : message);
        }
    }
}
