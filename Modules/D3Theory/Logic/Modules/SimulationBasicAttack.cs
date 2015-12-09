namespace CarbonCore.Modules.D3Theory.Logic.Modules
{
    using CarbonCore.Modules.D3Theory.Contracts;
    using CarbonCore.Modules.D3Theory.Data;

    public class SimulationBasicAttack : ISimulationModule
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name
        {
            get
            {
                return "BasicAttack";
            }
        }

        public SimulationSampleSet Simulate(ISimulationData data)
        {
            var set = new SimulationSampleSet { Name = this.Name };
            while (data.CurrentTime < data.MaxTime)
            {
                data.UpdateTargets(set);

                if (!SimulateShared.CanExecuteAction(data))
                {
                    break;
                }

                IEntity target = data.PickRandomTarget();
                SimulateShared.ApplyBaseAttack(data, set, target);
                SimulateShared.FinalizeAction(data, set);
            }

            set.UpdateDPS(data.MaxTime);
            return set;
        }
    }
}
