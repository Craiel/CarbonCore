namespace CarbonCore.Utils.Unity.Contracts.BehaviorTree
{
    using CarbonCore.Utils.Unity.Contracts;
    using CarbonCore.Utils.Unity.Logic.BehaviorTree;

    public interface IBehaviorTree : IEngineComponent
    {
        IBehaviorTreeNode Root { get; set; }

        void Load(string data);
        string Save();

        void Execute(BehaviorTreeContext context);
    }
}
