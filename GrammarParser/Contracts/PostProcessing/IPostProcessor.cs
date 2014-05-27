namespace CarbonCore.GrammarParser.Contracts.PostProcessing
{
    using System.Collections.Generic;

    using CarbonCore.GrammarParser.PostProcessing;

    public interface IPostProcessor
    {
        IList<string> Process(ProcessingInstruction instruction);
    }
}
