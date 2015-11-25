namespace CarbonCore.Utils.Unity.Logic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Unity.Contracts;

    // https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
    public class HeapPriorityQueue<T> : IPriorityQueue<T>
        where T : PriorityQueueNode
    {
        private T[] nodes;
        private long lifetimeNodeQueueCount;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public HeapPriorityQueue(int maxNodes)
        {
            Diagnostic.Assert(maxNodes > 0);

            this.nodes = new T[maxNodes + 1];
            this.lifetimeNodeQueueCount = 0;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Count { get; private set; }

        public int MaxSize
        {
            get
            {
                return this.nodes.Length - 1;
            }
        }

        public T First
        {
            get
            {
                return this.nodes[1];
            }
        }

        public void Clear()
        {
            Array.Clear(this.nodes, 1, this.Count);
            this.Count = 0;
        }

        public bool Contains(T node)
        {
            return this.nodes[node.QueueIndex] == node;
        }

        public void Enqueue(T node, double priority)
        {
            node.Priority = priority;
            this.Count++;
            this.nodes[this.Count] = node;
            node.QueueIndex = this.Count;
            node.InsertionIndex = this.lifetimeNodeQueueCount++;
            this.CascadeUp(this.nodes[this.Count]);
        }

        public T Dequeue()
        {
            T returnMe = this.nodes[1];
            this.Remove(returnMe);
            return returnMe;
        }

        public void Resize(int maxNodes)
        {
            T[] newArray = new T[maxNodes + 1];
            int highestIndexToCopy = Math.Min(maxNodes, this.Count);
            for (int i = 1; i <= highestIndexToCopy; i++)
            {
                newArray[i] = this.nodes[i];
            }

            this.nodes = newArray;
        }
        
        public void UpdatePriority(T node, double priority)
        {
            node.Priority = priority;
            this.OnNodeUpdated(node);
        }

        public void Remove(T node)
        {
            if (this.Count <= 1)
            {
                this.nodes[1] = null;
                this.Count = 0;
                return;
            }

            bool wasSwapped = false;
            T formerLastNode = this.nodes[this.Count];
            if (node.QueueIndex != this.Count)
            {
                this.Swap(node, formerLastNode);
                wasSwapped = true;
            }

            this.Count--;
            this.nodes[node.QueueIndex] = null;

            if (wasSwapped)
            {
                this.OnNodeUpdated(formerLastNode);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 1; i <= this.Count; i++)
            {
                yield return this.nodes[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool IsValidQueue()
        {
            for (int i = 1; i < this.nodes.Length; i++)
            {
                if (this.nodes[i] != null)
                {
                    int childLeftIndex = 2 * i;
                    if (childLeftIndex < this.nodes.Length && this.nodes[childLeftIndex] != null
                        && this.HasHigherPriority(this.nodes[childLeftIndex], this.nodes[i]))
                    {
                        return false;
                    }

                    int childRightIndex = childLeftIndex + 1;
                    if (childRightIndex < this.nodes.Length && this.nodes[childRightIndex] != null
                        && this.HasHigherPriority(this.nodes[childRightIndex], this.nodes[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Swap(T node1, T node2)
        {
            this.nodes[node1.QueueIndex] = node2;
            this.nodes[node2.QueueIndex] = node1;

            int temp = node1.QueueIndex;
            node1.QueueIndex = node2.QueueIndex;
            node2.QueueIndex = temp;
        }

        private void CascadeUp(T node)
        {
            int parent = node.QueueIndex / 2;
            while (parent >= 1)
            {
                T parentNode = this.nodes[parent];
                if (this.HasHigherPriority(parentNode, node))
                {
                    break;
                }

                this.Swap(node, parentNode);

                parent = node.QueueIndex / 2;
            }
        }

        private void CascadeDown(T node)
        {
            int finalQueueIndex = node.QueueIndex;
            while (true)
            {
                var newParent = node;
                int childLeftIndex = 2 * finalQueueIndex;

                if (childLeftIndex > this.Count)
                {
                    node.QueueIndex = finalQueueIndex;
                    this.nodes[finalQueueIndex] = node;
                    break;
                }

                T childLeft = this.nodes[childLeftIndex];
                if (this.HasHigherPriority(childLeft, newParent))
                {
                    newParent = childLeft;
                }

                int childRightIndex = childLeftIndex + 1;
                if (childRightIndex <= this.Count)
                {
                    T childRight = this.nodes[childRightIndex];
                    if (this.HasHigherPriority(childRight, newParent))
                    {
                        newParent = childRight;
                    }
                }

                if (newParent != node)
                {
                    this.nodes[finalQueueIndex] = newParent;

                    int temp = newParent.QueueIndex;
                    newParent.QueueIndex = finalQueueIndex;
                    finalQueueIndex = temp;
                }
                else
                {
                    node.QueueIndex = finalQueueIndex;
                    this.nodes[finalQueueIndex] = node;
                    break;
                }
            }
        }

        private void OnNodeUpdated(T node)
        {
            int parentIndex = node.QueueIndex / 2;
            T parentNode = this.nodes[parentIndex];

            if (parentIndex > 0 && this.HasHigherPriority(node, parentNode))
            {
                this.CascadeUp(node);
            }
            else
            {
                this.CascadeDown(node);
            }
        }

        private bool HasHigherPriority(T higher, T lower)
        {
            return higher.Priority < lower.Priority
                   || (Math.Abs(higher.Priority - lower.Priority) < float.Epsilon && higher.InsertionIndex < lower.InsertionIndex);
        }
    }
}
