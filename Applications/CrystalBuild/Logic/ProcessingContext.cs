namespace CarbonCore.Applications.CrystalBuild.Logic
{
    using System.Collections.Generic;

    using CarbonCore.Utils.IO;

    public class ProcessingContext
    {
        private readonly List<string> warnings;

        private readonly List<string> errors;
 
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ProcessingContext(ProcessingCache cache, BuildTargetPlatform targetPlatform)
        {
            this.warnings = new List<string>();
            this.errors = new List<string>();

            this.Cache = cache;
            this.TargetPlatform = targetPlatform;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public bool IsDebug { get; set; }

        public bool ExportSourceAsModule { get; set; }

        public ProcessingCache Cache { get; private set; }
        
        public CarbonDirectory Root { get; set; }

        public BuildTargetPlatform TargetPlatform { get; private set; }

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
