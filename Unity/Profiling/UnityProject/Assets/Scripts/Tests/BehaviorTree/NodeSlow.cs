namespace Assets.Scripts.Tests.BehaviorTree
{
    using System.Collections.Generic;

    using Assets.Scripts.Game;

    using CarbonCore.Utils.Unity.Logic.BehaviorTree;

    public class NodeSlow : BehaviorTreeNode
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Execute(BehaviorTreeContext context)
        {
            var system = context.Get<GameSystem>();
            var list = context.Get<IList<int>>();

            for (var i = 0; i < 1000; i++)
            {
            }
        }
    }
}
