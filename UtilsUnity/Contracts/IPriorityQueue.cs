namespace CarbonCore.Utils.Unity.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Unity.Logic;

    public interface IPriorityQueue<T> : IEnumerable<T>
        where T : PriorityQueueNode
    {
        T First { get; }

        int Count { get; }

        int MaxSize { get; }

        void Remove(T node);

        void UpdatePriority(T node, double priority);

        void Enqueue(T node, double priority);

        T Dequeue();

        void Clear();

        bool Contains(T node);

        void Resize(int maxNodes);
    }
}
