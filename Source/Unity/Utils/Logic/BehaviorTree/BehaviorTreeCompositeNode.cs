﻿namespace CarbonCore.Unity.Utils.Logic.BehaviorTree
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Unity.Utils.Contracts.BehaviorTree;
    using CarbonCore.Unity.Utils.Logic.Enums;

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

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoExecute(BehaviorTreeContext context)
        {
            this.Status = this.IsRandom 
                ? this.ProcessChildrenRandom(context) 
                : this.ProcessChildren(context);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private BehaviorTreeStatus ProcessChildren(BehaviorTreeContext context)
        {
            ProfilerUtils.BeginSampleThreadsafe("ProcessChildren");

            if (this.Children == null)
            {
                // No children, just succeed
                return BehaviorTreeStatus.Succeeded;
            }

            // the default status depends on the mode we are trying to go for
            BehaviorTreeStatus resultStatus = this.Mode == BehaviorTreeCompositeMode.Selector
                                                  ? BehaviorTreeStatus.Failed
                                                  : BehaviorTreeStatus.Succeeded;

            for (var i = 0; i < this.Children.Count; i++)
            {
                BehaviorTreeStatus? nodeStatus = this.ExecuteNode(context, this.Children[i]);
                if (nodeStatus != null)
                {
                    ProfilerUtils.EndSampleThreadSafe();
                    return nodeStatus.Value;
                }
            }

            ProfilerUtils.EndSampleThreadSafe();
            return resultStatus;
        }

        private BehaviorTreeStatus ProcessChildrenRandom(BehaviorTreeContext context)
        {
            ProfilerUtils.BeginSampleThreadsafe("ProcessChildrenRandom");

            if (this.Children == null)
            {
                // No children, just succeed
                return BehaviorTreeStatus.Succeeded;
            }

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

                BehaviorTreeStatus? nodeStatus = this.ExecuteNode(context, node);
                if (nodeStatus != null)
                {
                    ProfilerUtils.EndSampleThreadSafe();
                    return nodeStatus.Value;
                }
            }

            ProfilerUtils.EndSampleThreadSafe();
            return resultStatus;
        }

        private BehaviorTreeStatus? ExecuteNode(BehaviorTreeContext context, IBehaviorTreeNode node)
        {
            node.OnEnter(context);
            node.Execute(context);
            node.OnExit(context);

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

            return null;
        }
    }
}