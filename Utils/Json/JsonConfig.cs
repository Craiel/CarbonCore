namespace CarbonCore.Utils.Json
{
    using System;

    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;

    using Newtonsoft.Json;
    
    public class JsonConfig<T> : IJsonConfig<T>
        where T : class
    {
        private CarbonFile configFile;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Current { get; set; }

        public virtual bool Load(CarbonFile file)
        {
            this.configFile = file;
            return this.LoadConfig(this.configFile);
        }

        public virtual bool Save(CarbonFile file = null)
        {
            CarbonFile targetFile = file ?? this.configFile;
            System.Diagnostics.Trace.Assert(targetFile != null);

            try
            {
                JsonExtensions.SaveToFile(targetFile, this.Current, false, Formatting.Indented);
                return true;
            }
            catch (Exception e)
            {
                Diagnostic.Exception(e);
                System.Diagnostics.Trace.TraceError("Could not save config to {0}", file);
                return false;
            }
        }

        public virtual void Reset()
        {
            this.Current = this.GetDefault();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual T GetDefault()
        {
            return Activator.CreateInstance<T>();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool LoadConfig(CarbonFile file)
        {
            if (file.Exists)
            {
                this.Current = JsonExtensions.LoadFromFile<T>(file, false);
            }

            if (this.Current == null)
            {
                System.Diagnostics.Trace.TraceError("Config is invalid, resetting to default");
                this.Current = this.GetDefault();

                JsonExtensions.SaveToFile(file, this.Current, false, Formatting.Indented);
                return false;
            }

            return true;
        }
    }
}
