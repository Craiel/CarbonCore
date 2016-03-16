namespace CarbonCore.Unity.Utils.Logic.BehaviorTree
{
    using System;

    using CarbonCore.Unity.Utils.Contracts.BehaviorTree;
    using CarbonCore.Unity.Utils.Logic.Enums;
    using CarbonCore.Utils.Diagnostics;

    public class BehaviorTreeDecoratorNode : BehaviorTreeNode, IBehaviorTreeDecoratorNode
    {
        private static int MaxRepeatsUntil = 100;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BehaviorTreeDecoratorNode()
        {
        }

        public BehaviorTreeDecoratorNode(IBehaviorTreeNode node, BehaviorTreeDecoratorMode mode)
        {
            this.Leaf = node;
            this.Mode = mode;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BehaviorTreeDecoratorMode Mode { get; set; }

        public IBehaviorTreeNode Leaf { get; set; }

        public int RepeatCount { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoExecute(BehaviorTreeContext context)
        {
            Diagnostic.Assert(this.Leaf != null, "No leaf in decorator!");

            switch (this.Mode)
            {
                    case BehaviorTreeDecoratorMode.Inverter:
                    {
                        this.Leaf.Execute(context);
                        this.Status = this.Leaf.Status == BehaviorTreeStatus.Failed ? BehaviorTreeStatus.Succeeded : BehaviorTreeStatus.Failed;
                        break;
                    }

                    case BehaviorTreeDecoratorMode.RepeatUntilFail:
                    {
                        int abortCounter = 0;
                        while (this.Leaf.Status != BehaviorTreeStatus.Failed)
                        {
                            this.Leaf.Execute(context);
                            abortCounter++;
                            if (abortCounter > MaxRepeatsUntil)
                            {
                                throw new Exception("RepeatUntilFail did not finish in time");
                            }
                        }

                        break;
                    }

                    case BehaviorTreeDecoratorMode.RepeatUntilSucceed:
                    {
                        int abortCounter = 0;
                        while (this.Leaf.Status != BehaviorTreeStatus.Succeeded)
                        {
                            this.Leaf.Execute(context);
                            abortCounter++;
                            if (abortCounter > MaxRepeatsUntil)
                            {
                                throw new Exception("RepeatUntilSucceed did not finish in time");
                            }
                        }

                        break;
                    }

                    case BehaviorTreeDecoratorMode.Repeater:
                    {
                        Diagnostic.Assert(this.RepeatCount > 0, "No repetition count supplied");
                        for (var i = 0; i < this.RepeatCount; i++)
                        {
                            this.Leaf.Execute(context);
                        }

                        this.Status = BehaviorTreeStatus.Succeeded;
                        break;
                    }

                    case BehaviorTreeDecoratorMode.Failer:
                    {
                        this.Leaf.Execute(context);
                        this.Status = BehaviorTreeStatus.Failed;
                        break;
                    }

                    case BehaviorTreeDecoratorMode.Succeeder:
                    {
                        this.Leaf.Execute(context);
                        this.Status = BehaviorTreeStatus.Succeeded;
                        break;
                    }
            }
        }
    }
}
