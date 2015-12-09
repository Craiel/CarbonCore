namespace D3Theory.Console
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Utils;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Json;
    using CarbonCore.Utils.Edge.CommandLine.Contracts;

    using D3Theory.Console.Contracts;
    using D3Theory.Contracts;
    using D3Theory.Data;
    using D3Theory.Logic;
    using D3Theory.Logic.Modules;

    using Newtonsoft.Json;

    public class Main : IMain
    {
        private const string DefaultDataDirectory = "Data";
        private const string DefaultSimulationFile = "default";

        private const string ExtensionSimulation = ".sim";
        private const string ExtensionResult = ".result";
        private const string ExtensionResultTable = ".csv";

        private readonly IMainData data;

        private readonly ICommandLineArguments arguments;

        private readonly IList<Simulation> simulations;
        private readonly IList<CarbonFile> simulationFiles;

        private bool saveAsTable;
        private bool randomValues;

        private CarbonDirectory dataPath;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Main(IFactory factory)
        {
            this.data = factory.Resolve<IMainData>();
            this.simulations = new List<Simulation>();
            this.simulationFiles = new List<CarbonFile>();

            this.arguments = factory.Resolve<ICommandLineArguments>();
            
            this.RegisterCommandLineArguments();

            this.dataPath = new CarbonDirectory(string.Empty);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Simulate()
        {
            if (!this.arguments.ParseCommandLineArguments())
            {
                this.arguments.PrintArgumentUse();
                return;
            }

            foreach (CarbonFile file in this.simulationFiles)
            {
                var simulation = JsonExtensions.LoadFromFile<Simulation>(file, false);
                simulation.File = file;
                this.simulations.Add(simulation);
            }

            if (this.dataPath.IsNull)
            {
                this.dataPath = RuntimeInfo.Path.ToDirectory(DefaultDataDirectory);
            }

            if (this.simulations.Count <= 0)
            {
                Simulation defaultSimulation = Defaults.GetDefaultSimulation();
                defaultSimulation.File = RuntimeInfo.Path.ToFile(DefaultSimulationFile + ExtensionSimulation);
                this.simulations.Add(defaultSimulation);
                JsonExtensions.SaveToFile(defaultSimulation.File, defaultSimulation, false, Formatting.Indented);
            }
            
            this.data.Load(this.dataPath);

            foreach (Simulation simulation in this.simulations)
            {
                SimulationStats results = this.Simulate(simulation);
                if (results == null)
                {
                    continue;
                }

                if (this.saveAsTable)
                {
                    CarbonFile resultFile = simulation.File.ChangeExtension(string.Concat(simulation.File.Extension, ExtensionResult, ExtensionResultTable));
                    using (FileStream stream = resultFile.OpenCreate())
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.Write(results.ExportAsText());
                        }
                    }

                    resultFile = simulation.File.ChangeExtension(string.Concat(simulation.File.Extension, ExtensionResult, ".Sets", ExtensionResultTable));
                    using (FileStream stream = resultFile.OpenCreate())
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            writer.Write(results.ExportSetsAsTable());
                        }
                    }
                }
                else
                {
                    CarbonFile resultFile = simulation.File.ChangeExtension(simulation.File.Extension + ExtensionResult);
                    JsonExtensions.SaveToFile(resultFile, results, false, Formatting.Indented);
                }
            }

            this.data.Save(this.dataPath);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void RegisterCommandLineArguments()
        {
            ICommandLineSwitchDefinition definition = this.arguments.Define("d", "dataPath", x => this.dataPath = new CarbonDirectory(x));
            definition.Description = "The path where the data is located";

            definition = this.arguments.Define("s", "simulationFile", x => this.simulationFiles.Add(new CarbonFile(x)));
            definition.RequireArgument = true;
            definition.AllowMultiple = true;
            definition.Description = "The simulation to run";

            definition = this.arguments.Define("r", "randomValues", x => this.randomValues = true);
            definition.Description = "Uses a seeded random instead of fixed giving different results every run";

            definition = this.arguments.Define("t", "saveTable", x => this.saveAsTable = true);
            definition.Description = "Save as table in text format instead of Json report";
        }

        private SimulationStats Simulate(Simulation simulation)
        {
            System.Diagnostics.Trace.TraceInformation("Starting simulation for " + simulation.Class);
            
            // Create the struct and pass it to the logic
            ISimulationData simulationData = new SimulationData(this.data, simulation, this.randomValues);

            // Todo: choose which modules to use
            IList<ISimulationModule> modules = new List<ISimulationModule>
                                                   {
                                                       new SimulationBasicAttack()
                                                   };

            if (simulationData.Class.Skills != null)
            {
                foreach (D3Skill skill in simulationData.Class.Skills)
                {
                    modules.Add(new SimulationBasicSkill(skill.Name));
                }
            }

            foreach (ISimulationModule module in modules)
            {
                simulationData.ClearTargets();
                simulationData.CurrentTime = 0.0f;
                SimulationSampleSet sampleSet = module.Simulate(simulationData);
                if (sampleSet == null)
                {
                    System.Diagnostics.Trace.TraceWarning("No sample returned for {0}", module.Name);
                    continue;
                }

                simulationData.Stats.SampleSets.Add(sampleSet);
            }

            return simulationData.Stats;
        }
    }
}
