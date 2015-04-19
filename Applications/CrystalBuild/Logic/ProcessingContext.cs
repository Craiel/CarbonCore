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
        public ProcessingContext()
        {
            this.warnings = new List<string>();
            this.errors = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsDebug { get; set; }

        public CarbonFile Template { get; set; }

        public CarbonDirectory Root { get; set; }

        public IReadOnlyCollection<string> Warnings
        {
            get
            {
                return this.warnings.AsReadOnly();
            }
        }

        public IReadOnlyCollection<string> Errors
        {
            get
            {
                return this.errors.AsReadOnly();
            }
        }

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
