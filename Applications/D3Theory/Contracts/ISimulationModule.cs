namespace D3Theory.Contracts
{
    using D3Theory.Data;

    public interface ISimulationModule
    {
        string Name { get; }

        SimulationSampleSet Simulate(ISimulationData data);
    }
}
