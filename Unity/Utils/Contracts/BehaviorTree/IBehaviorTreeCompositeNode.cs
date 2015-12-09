namespace CarbonCore.Utils.Unity.Contracts.BehaviorTree
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Unity.Logic.Enums;

    public interface IBehaviorTreeCompositeNode : IBehaviorTreeNode
    {
        BehaviorTreeCompositeMode Mode { get; set; }

        bool IsRandom { get; set; }

        List<IBehaviorTreeNode> Children { get; set; }
    }
}
