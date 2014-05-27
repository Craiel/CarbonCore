namespace CarbonCore.GrammarParser.PostProcessors
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.Patching;
    using CarbonCore.GrammarParser.PostProcessing;

    public class PatcherData : ProcessingData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public PatcherData()
        {
            this.Patches = new List<Patch>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<Patch> Patches { get; private set; }
    }
}
