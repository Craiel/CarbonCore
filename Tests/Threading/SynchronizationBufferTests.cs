namespace CarbonCore.Tests.Threading
{
    using CarbonCore.Utils.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class SynchronizationBufferTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void TestReadWrite()
        {
            SynchronizationBuffer<DataHolder> buffer = new SynchronizationBuffer<DataHolder>();

            var write = buffer.AquireWriteLock();
            write.Data.Value = 0xBADF00D;
            buffer.ReleaseLock(write);

            var read = buffer.AquireReadLock();
            Assert.AreEqual(0xBADF00D, read.Data.Value);
            buffer.ReleaseLock(read);
        }

        [Test]
        public void TestWriteRelevance()
        {
            SynchronizationBuffer<DataHolder> buffer = new SynchronizationBuffer<DataHolder>();

            var write = buffer.AquireWriteLock();
            write.Data.Value = 1;
            buffer.ReleaseLock(write);

            write = buffer.AquireWriteLock();
            write.Data.Value = 2;
            buffer.ReleaseLock(write);

            var read = buffer.AquireReadLock();
            Assert.AreEqual(2, read.Data.Value);
            buffer.ReleaseLock(read);
        }

        [Test]
        public void TestWriteLock()
        {
            SynchronizationBuffer<DataHolder> buffer = new SynchronizationBuffer<DataHolder>();

            var write = buffer.AquireWriteLock();
            write.Data.Value = 1;
            buffer.ReleaseLock(write);

            write = buffer.AquireWriteLock();
            write.Data.Value = 2;

            var read = buffer.AquireReadLock();
            Assert.AreEqual(1, read.Data.Value);
            buffer.ReleaseLock(read);

            buffer.ReleaseLock(write);

            read = buffer.AquireReadLock();
            Assert.AreEqual(2, read.Data.Value);
            buffer.ReleaseLock(read);
        }

        private class DataHolder
        {
            public int Value { get; set; }
        }
    }
}
