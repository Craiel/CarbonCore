namespace CarbonCore.GrammarParser.Contracts.PostProcessing
{
    public interface IProcessLineData
    {
        string Name { get; set; }

        TX GetLineData<TX>() where TX : class, ILine;
    }
}
