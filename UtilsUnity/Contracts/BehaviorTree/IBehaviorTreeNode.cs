namespace CarbonCore.Utils.Unity.Contracts.BehaviorTree
{
    using CarbonCore.Utils.Unity.Logic.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public interface IBehaviorTreeNode
    {
        BehaviorTreeStatus Status { get; }

        void OnEnter(BehaviorTreeContext context);
        void Execute(BehaviorTreeContext context);
        void OnExit(BehaviorTreeContext context);
    }
}
