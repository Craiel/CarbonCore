namespace CarbonCore.Utils.Unity.Logic.BehaviorTree
{
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Json;
    using CarbonCore.Utils.Unity.Contracts.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic;

    public class BehaviorTree : EngineComponent, IBehaviorTree
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IBehaviorTreeNode Root { get; set; }

        public virtual void Load(string data)
        {
            this.Root = JsonExtensions.LoadFromData<IBehaviorTreeNode>(data);
        }

        public virtual string Save()
        {
            Diagnostic.Assert(this.Root != null);

            return JsonExtensions.SaveToData(this.Root);
        }

        public virtual void Execute(BehaviorTreeContext context)
        {
            this.Root.Execute(context);
        }
    }
}
