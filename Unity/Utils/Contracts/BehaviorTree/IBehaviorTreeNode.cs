namespace CarbonCore.Unity.Utils.Contracts.BehaviorTree
{
    using CarbonCore.Unity.Utils.Logic.BehaviorTree;
    using CarbonCore.Unity.Utils.Logic.Enums;

    public interface IBehaviorTreeNode
    {
        BehaviorTreeStatus Status { get; }

        void OnEnter(BehaviorTreeContext context);
        void Execute(BehaviorTreeContext context);
        void OnExit(BehaviorTreeContext context);
    }
}
