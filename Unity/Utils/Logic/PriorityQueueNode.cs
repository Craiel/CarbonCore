namespace CarbonCore.Utils.Unity.Logic
{
    public class PriorityQueueNode
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public double Priority { get; set; }

        public long InsertionIndex { get; set; }

        public int QueueIndex { get; set; }
    }
}
