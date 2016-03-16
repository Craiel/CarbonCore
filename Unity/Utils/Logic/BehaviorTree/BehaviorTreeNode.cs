namespace CarbonCore.Unity.Utils.Logic.BehaviorTree
{
    using CarbonCore.Unity.Utils.Contracts.BehaviorTree;
    using CarbonCore.Unity.Utils.Logic.Enums;

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

        public abstract void Execute(BehaviorTreeContext context);

        public virtual void OnExit(BehaviorTreeContext context)
        {
        }
    }
}
