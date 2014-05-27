namespace CarbonCore.Utils.Diagnostics
{
    using System;

    public class ProgressRegion : IDisposable
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ProgressRegion(string name, int progressMax)
        {
            this.Name = name;
            this.ProgressMax = progressMax;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public int Progress { get; set; }
        public int ProgressMax { get; set; }

        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("ProgressRegionEnd: " + this.Name);
        }

        public void Update()
        {
            // Todo
        }
    }
}
