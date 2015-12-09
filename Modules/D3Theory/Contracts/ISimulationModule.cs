namespace CarbonCore.Modules.D3Theory.Contracts
{
    using CarbonCore.Modules.D3Theory.Data;

    public interface ISimulationModule
    {
        string Name { get; }

        SimulationSampleSet Simulate(ISimulationData data);
    }
}
