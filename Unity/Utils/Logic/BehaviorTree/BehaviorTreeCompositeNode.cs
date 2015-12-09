namespace CarbonCore.Utils.Unity.Logic.BehaviorTree
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Unity.Contracts.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public class BehaviorTreeCompositeNode : BehaviorTreeNode, IBehaviorTreeCompositeNode
    {
        private static readonly Random CompositeRandom = new Random((int)DateTime.Now.Ticks);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BehaviorTreeCompositeNode()
        {
        }

        public BehaviorTreeCompositeNode(BehaviorTreeCompositeMode mode)
        {
            this.Mode = mode;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public BehaviorTreeCompositeMode Mode { get; set; }

        public bool IsRandom { get; set; }

        public List<IBehaviorTreeNode> Children { get; set; }

        public void AddChild(IBehaviorTreeNode node)
        {
            if (this.Children == null)
            {
                this.Children = new List<IBehaviorTreeNode>();
            }

            this.Children.Add(node);
        }

        public void RemoveChild(IBehaviorTreeNode node)
        {
            this.Children.Remove(node);
        }

        public override void Execute(BehaviorTreeContext context)
        {
            Diagnostic.Assert(this.Children != null, "Empty Composite node in Tree");

            this.Status = this.ProcessChildren(context);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private BehaviorTreeStatus ProcessChildren(BehaviorTreeContext context)
        {
            // the default status depends on the mode we are trying to go for
            BehaviorTreeStatus resultStatus = this.Mode == BehaviorTreeCompositeMode.Selector
                                                  ? BehaviorTreeStatus.Failed
                                                  : BehaviorTreeStatus.Succeeded;

            var nodes = new List<IBehaviorTreeNode>(this.Children);
            while (nodes.Count > 0)
            {
                int index = 0;
                if (this.IsRandom)
                {
                    index = CompositeRandom.Next(nodes.Count);
                }

                IBehaviorTreeNode node = nodes[index];
                nodes.RemoveAt(index);

                node.OnEnter(context);
                node.Execute(context);
                node.OnExit(context);

                /*if (node.GetType() != typeof(BehaviorTreeCompositeNode))
                {
                    Diagnostic.Info(
                        "BT.Execute: {0} -> {1}",
                        node.GetType().Name,
                        node.Status);
                }*/

                // In sequence all nodes have to succeed
                switch (this.Mode)
                {
                    case BehaviorTreeCompositeMode.Sequence:
                        {
                            if (node.Status == BehaviorTreeStatus.Failed
                                || node.Status == BehaviorTreeStatus.Running)
                            {
                                // All nodes have to succeed for sequence
                                return node.Status;
                            }

                            break;
                        }

                    case BehaviorTreeCompositeMode.Selector:
                        {
                            // Any node needs to succeed for a selector to be valid
                            if (node.Status == BehaviorTreeStatus.Succeeded)
                            {
                                return node.Status;
                            }

                            // Try the next node
                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException(this.Mode.ToString());
                        }
                }
            }

            return resultStatus;
        }
    }
}
