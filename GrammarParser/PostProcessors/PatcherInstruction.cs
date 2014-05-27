namespace CarbonCore.GrammarParser.PostProcessors
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.Patching;
    using CarbonCore.GrammarParser.PostProcessing;

    public class PatcherInstruction : ProcessingInstruction
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public PatcherInstruction(string fileName, IList<string> data, IList<Patch> patches)
            : base(fileName, data)
        {
            this.Patches = patches;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<Patch> Patches { get; private set; }
    }
}
