namespace CarbonCore.Modules.D3Theory
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Modules.D3Theory.Contracts;
    using CarbonCore.Modules.D3Theory.Data;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    public class MainData : IMainData
    {
        private const string ExtensionClass = ".class";

        private static readonly CarbonFile FileGeneric = new CarbonFile("d3.generic");

        private readonly List<D3Class> classes;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public MainData()
        {
            this.classes = new List<D3Class>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public D3Generic Generic { get; private set; }

        public IReadOnlyCollection<D3Class> Classes
        {
            get
            {
                return this.classes.AsReadOnly();
            }
        }

        public void Load(CarbonDirectory path)
        {
            if (!path.IsNull && path.Exists)
            {
                if (this.LoadFromPath(path))
                {
                    return;
                }
            }

            System.Diagnostics.Trace.TraceWarning("Could not load data from {0}, using defaults!", path);
            this.LoadFromDefaults();
        }

        public void Save(CarbonDirectory path)
        {
            path.Create();
            CarbonFile target = path.ToFile(FileGeneric);
            JsonExtensions.SaveToFile(target, this.Generic, false, Formatting.Indented);

            foreach (D3Class @class in this.classes)
            {
                CarbonFile classTarget = path.ToFile(@class.Name + ExtensionClass);
                JsonExtensions.SaveToFile(classTarget, @class, false, Formatting.Indented);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ClearData()
        {
            this.classes.Clear();
        }

        private void LoadFromDefaults()
        {
            this.ClearData();

            this.Generic = Defaults.GetDefaultGeneric();

            foreach (D3Class @class in Defaults.GetDefaultClasses())
            {
                this.classes.Add(@class);
            }
        }

        private bool LoadFromPath(CarbonDirectory path)
        {
            this.ClearData();

            this.Generic = JsonExtensions.LoadFromFile<D3Generic>(path.ToFile(FileGeneric), false);

            CarbonFileResult[] files = path.GetFiles("*" + ExtensionClass);
            foreach (CarbonFileResult file in files)
            {
                try
                {
                    var @class = JsonExtensions.LoadFromFile<D3Class>(file.Absolute, false);
                    this.classes.Add(@class);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Trace.TraceError("Could not load class definition from {0}: {1}", file, e);
                    return false;
                }
            }

            return true;
        }
    }
}
