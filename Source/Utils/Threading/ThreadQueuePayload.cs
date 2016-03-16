namespace CarbonCore.Utils.Threading
{
    using CarbonCore.Utils.Contracts;

    public class ThreadQueuePayload : IThreadQueueOperationPayload
    {
        public object Data { get; set; }
    }
}
