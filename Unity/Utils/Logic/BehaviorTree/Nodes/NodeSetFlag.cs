namespace CarbonCore.Utils.Unity.Logic.BehaviorTree.Nodes
{
    using CarbonCore.Utils.Unity.Logic.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoExecute(BehaviorTreeContext context)
        {
            context.SetVariable(this.Key, this.Value);
            this.Status = BehaviorTreeStatus.Succeeded;
        }
    }
}
