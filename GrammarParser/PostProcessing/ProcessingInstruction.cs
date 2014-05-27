namespace CarbonCore.GrammarParser.PostProcessing
{
    using System.Collections.Generic;

    public class ProcessingInstruction
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ProcessingInstruction(string fileName, IList<string> data)
        {
            this.FileName = fileName;
            this.Data = data;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName { get; set; }

        public IList<string> Data { get; set; } 
    }
}
