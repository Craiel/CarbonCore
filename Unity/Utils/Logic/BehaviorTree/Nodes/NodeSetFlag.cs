namespace CarbonCore.Unity.Utils.Logic.BehaviorTree.Nodes
{
    using CarbonCore.Unity.Utils.Logic.Enums;

    public class NodeSetFlag : BehaviorTreeLeafNode
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public NodeSetFlag()
        {
        }

        public NodeSetFlag(int key, bool value = true)
        {
            this.Key = key;
            this.Value = value;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Key { get; set; }

        public bool Value { get; set; }

        public override void Execute(BehaviorTreeContext context)
        {
            context.SetVariable(this.Key, this.Value);
            this.Status = BehaviorTreeStatus.Succeeded;
        }
    }
}
