namespace CarbonCore.Utils.Unity.IoC
{
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.Unity.Contracts.BehaviorTree;
    using CarbonCore.Utils.Unity.Contracts.BufferedData;
    using CarbonCore.Utils.Unity.Contracts.TaskPool;
    using CarbonCore.Utils.Unity.Logic.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.BufferedData;
    using CarbonCore.Utils.Unity.Logic.TaskPoolLogic;

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