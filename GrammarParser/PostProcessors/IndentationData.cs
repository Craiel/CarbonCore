namespace CarbonCore.GrammarParser.PostProcessors
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.PostProcessing;

    public class IndentationData : ProcessingData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Level { get; set; }

        public HashSet<char> IncreaseAt { get; set; }
        public HashSet<char> DecreaseAt { get; set; }

        public char IndentationChar { get; set; }

        public byte IndentationAmount { get; set; }
    }
}
