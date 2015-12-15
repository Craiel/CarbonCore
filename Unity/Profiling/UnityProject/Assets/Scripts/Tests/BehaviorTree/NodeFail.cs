﻿namespace Assets.Scripts.Tests.BehaviorTree
{
    using System.Collections.Generic;

    using Assets.Scripts.Game;

    using CarbonCore.Utils.Unity.Logic.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public class NodeFail : BehaviorTreeNode
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Execute(BehaviorTreeContext context)
        {
            var system = context.Get<GameSystem>();
            var list = context.Get<IList<int>>();

            this.Status = BehaviorTreeStatus.Failed;
        }
    }
}