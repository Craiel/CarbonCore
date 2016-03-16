namespace CarbonCore.Unity.Tests.Data
{
    using System.Threading;

    using CarbonCore.Unity.Utils.Contracts;

    public class ReferenceEntry : IRefCountedObject
    {
        private int referenceCount;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int IntValue { get; set; }

        public int RefCount()
        {
            return this.referenceCount;
        }

        public void RefCountIncrease()
        {
            Interlocked.Increment(ref this.referenceCount);
        }

        public void RefCountDecrease()
        {
            Interlocked.Decrement(ref this.referenceCount);
        }
    }
}
