namespace CarbonCore.Unity.Utils.Contracts.BehaviorTree
{
    using CarbonCore.Unity.Utils.Logic.BehaviorTree;

    public interface IBehaviorTree : IEngineComponent
    {
        IBehaviorTreeNode Root { get; set; }

        void Load(string data);
        string Save();

        void Execute(BehaviorTreeContext context);
    }
}
