namespace CarbonCore.Utils.Unity.Contracts.BehaviorTree
{
    using CarbonCore.Utils.Unity.Logic.Enums;

    public interface IBehaviorTreeDecoratorNode : IBehaviorTreeNode
    {
        BehaviorTreeDecoratorMode Mode { get; set; }

        IBehaviorTreeNode Leaf { get; set; }

        int RepeatCount { get; set; }
    }
}
