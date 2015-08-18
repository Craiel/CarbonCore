namespace CarbonCore.Tests.ContentServices
{
    using System;

    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    public class SyncTestEntry : SyncEntry
    {
        public SyncContent<int?> Id { get; protected set; }

        public SyncContent<int> TestInt { get; protected set; }

        public SyncContent<long> TestLong { get; protected set; }

        public SyncContent<float> TestFloat { get; protected set; }

        public SyncContent<bool> TestBool { get; protected set; }

        public SyncContent<byte[]> ByteArray { get; protected set; }

        public SyncContent<string> TestString { get; protected set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return Tuple.Create(this.Id).GetHashCode();
        }
    }
}
