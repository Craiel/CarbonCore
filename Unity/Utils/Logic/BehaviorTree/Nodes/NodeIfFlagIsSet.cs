namespace CarbonCore.Utils.Unity.Logic.BehaviorTree.Nodes
{
    using CarbonCore.Utils.Unity.Logic.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public class NodeIfFlagIsSet : BehaviorTreeLeafNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public NodeIfFlagIsSet()
        {
        }

        public NodeIfFlagIsSet(int key)
        {
            this.Key = key;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Key { get; set; }

        public override void Execute(BehaviorTreeContext context)
        {
            if (context.Get<bool>(this.Key))
            {
                this.Status = BehaviorTreeStatus.Succeeded;
                return;
            }

            this.Status = BehaviorTreeStatus.Failed;
        }
    }
}
