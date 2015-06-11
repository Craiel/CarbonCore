namespace CarbonCore.Utils.Compat.Threading
{
    using CarbonCore.Utils.Compat.Contracts;

    public class ThreadQueuePayload : IThreadQueueOperationPayload
    {
        public object Data { get; set; }
    }
}
