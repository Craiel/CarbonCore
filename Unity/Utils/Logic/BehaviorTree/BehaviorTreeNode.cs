namespace CarbonCore.Utils.Unity.Logic.BehaviorTree
{
    using CarbonCore.Utils.Unity.Contracts.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public abstract class BehaviorTreeNode : IBehaviorTreeNode
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [JsonIgnore]
        public BehaviorTreeStatus Status { get; protected set; }

        public virtual void OnEnter(BehaviorTreeContext context)
        {
            this.Status = BehaviorTreeStatus.Undefined;
        }

        public void Execute(BehaviorTreeContext context)
        {
            ProfilerUtils.BeginSampleThreadsafe("BT_N: " + this.GetType().Name);

            this.DoExecute(context);

            ProfilerUtils.EndSampleThreadSafe();
        }

        public virtual void OnExit(BehaviorTreeContext context)
        {
        }

        protected abstract void DoExecute(BehaviorTreeContext context);
    }
}
