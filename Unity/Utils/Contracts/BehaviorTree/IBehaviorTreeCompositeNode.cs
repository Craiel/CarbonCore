namespace CarbonCore.Unity.Utils.Contracts.BehaviorTree
{
    using System.Collections.Generic;

    using CarbonCore.Unity.Utils.Logic.Enums;

    public interface IBehaviorTreeCompositeNode : IBehaviorTreeNode
    {
        BehaviorTreeCompositeMode Mode { get; set; }

        bool IsRandom { get; set; }

        List<IBehaviorTreeNode> Children { get; set; }
    }
}
