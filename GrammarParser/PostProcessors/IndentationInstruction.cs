namespace CarbonCore.GrammarParser.PostProcessors
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.PostProcessing;

    public class IndentationInstruction : ProcessingInstruction
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public IndentationInstruction(string fileName, IList<string> data)
            : base(fileName, data)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public char[] IncreaseAt { get; set; }
        public char[] DecreaseAt { get; set; }

        public char IndentationChar { get; set; }

        public byte IndentationAmount { get; set; }
    }
}
