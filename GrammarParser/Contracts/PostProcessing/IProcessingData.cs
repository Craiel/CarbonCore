namespace CarbonCore.GrammarParser.Contracts.PostProcessing
{
    using System.Collections.Generic;

    public interface IProcessingData
    {
        IList<string> Processed { get; }
    }
}
