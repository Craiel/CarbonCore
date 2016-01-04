namespace CarbonCore.Utils.Threading
{
    public struct SynchronizationDataLock<T>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SynchronizationDataLock(T data, int id)
            : this()
        {
            this.Data = data;
            this.Id = id;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Data { get; private set; }
        public int Id { get; private set; }
    }
}
