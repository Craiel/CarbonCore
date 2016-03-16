namespace CarbonCore.Unity.Utils.Logic.BehaviorTree
{
    using CarbonCore.Unity.Utils.Contracts.BehaviorTree;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Json;

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
            ProfilerUtils.BeginSampleThreadsafe("BT: " + this.GetType().Name);

            this.Root.Execute(context);

            ProfilerUtils.EndSampleThreadSafe();
        }
    }
}
