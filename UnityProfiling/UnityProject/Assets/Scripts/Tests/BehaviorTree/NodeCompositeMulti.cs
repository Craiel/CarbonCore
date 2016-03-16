namespace Assets.Scripts.Tests.BehaviorTree
{
    using CarbonCore.Utils.Unity.Logic.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public class NodeCompositeMulti : BehaviorTreeCompositeNode
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Execute(BehaviorTreeContext context)
        {
            for (var i = 0; i < 10; i++)
            {
                base.Execute(context);
            }

            this.Status = BehaviorTreeStatus.Succeeded;
        }
    }
}
