namespace CarbonCore.Unity.Utils.Contracts.BehaviorTree
{
    using CarbonCore.Unity.Utils.Logic.Enums;

    public interface IBehaviorTreeDecoratorNode : IBehaviorTreeNode
    {
        BehaviorTreeDecoratorMode Mode { get; set; }

        IBehaviorTreeNode Leaf { get; set; }

        int RepeatCount { get; set; }
    }
}
