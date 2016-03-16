namespace Assets.Scripts.Tests.BehaviorTree
{
    using System.Collections.Generic;

    using Assets.Scripts.Game;

    using CarbonCore.Utils.Unity.Logic.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public class NodeCustomComposite : BehaviorTreeCompositeNode
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Execute(BehaviorTreeContext context)
        {
            context.Get<GameSystem>();
            context.Get<IList<int>>();

            base.Execute(context);

            this.Status = BehaviorTreeStatus.Succeeded;
        }
    }
}
