namespace CarbonCore.GrammarParser.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.PostProcessing;

    public interface ILine
    {
        IReadOnlyCollection<string> PrecedingContent { get; }
        IReadOnlyCollection<string> SubsequentContent { get; }

        string Current { get; }
        string Next { get; }
        string Previous { get; set; }
        string TrimmedCurrent { get; }

        string Processed { get; set; }

        int LineNumber { get; }
        int ProcessedLineNumber { get; set; }

        LineUpdateMode UpdateMode { get; set; }

        int ContentBegin { get; set; }
        int ContentEnd { get; set; }
    }
}
