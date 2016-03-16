namespace CarbonCore.Unity.Utils.IoC
{
    using CarbonCore.Unity.Utils.Contracts.BehaviorTree;
    using CarbonCore.Unity.Utils.Contracts.BufferedData;
    using CarbonCore.Unity.Utils.Contracts.TaskPool;
    using CarbonCore.Unity.Utils.Logic.BehaviorTree;
    using CarbonCore.Unity.Utils.Logic.BufferedData;
    using CarbonCore.Unity.Utils.Logic.TaskPoolLogic;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    public class UtilsUnityModule : CarbonQuickModule
    {
        public UtilsUnityModule()
        {
            this.For<IBehaviorTree>().Use<BehaviorTree>();
            this.For<ITaskPool>().Use<TaskPool>();

            this.For<IBufferedDataPool>().Use<BufferedDataPool>();
            this.For<IBufferedDataSetInternal>().Use<BufferedDataset>();
        }
    }
}