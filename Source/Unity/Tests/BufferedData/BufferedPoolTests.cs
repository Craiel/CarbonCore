namespace CarbonCore.Unity.Tests.BufferedData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    
    using CarbonCore.Unity.Tests.Data;
    using CarbonCore.Unity.Utils.Contracts.BufferedData;
    using CarbonCore.Unity.Utils.Logic.BufferedData;
    using CarbonCore.Unity.Utils.Logic.BufferedData.Commands;

    using NUnit.Framework;

    [TestFixture]
    public class BufferedPoolTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void BasicDataPoolTest()
        {
            var pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(new BufferedDataPoolSettings { MaxDatasetCount = 2 });

            Assert.AreEqual(pool.CurrentCommand, pool.LatestCommand);

            using (DataSnapshot snapshot = pool.GetData())
            {
                Assert.IsNotNull(snapshot);
                Assert.IsTrue(snapshot.IsAlive);

                IList<BufferTestEntry> testData = snapshot.Target.GetInstances<BufferTestEntry>();
                Assert.IsNull(testData);
            }

            // Write a new instance into the pool
            pool.Enqueue(new BufferCommandWriteDiscreteInstance<BufferTestEntry>());
            pool.Commit();
            pool.Update();

            // We still have 1 command pending for the other dataset
            Assert.Less(pool.CurrentCommand, pool.LatestCommand);
            pool.Update();

            // Now all our commands should have been applied to all datasets
            Assert.AreEqual(pool.CurrentCommand, pool.LatestCommand);

            BufferTestEntry buffer1Instance;
            using (DataSnapshot snapshot = pool.GetData())
            {
                buffer1Instance = snapshot.Target.GetInstance<BufferTestEntry>();
                Assert.IsNotNull(buffer1Instance);
            }

            this.AdvanceBuffers(pool);

            BufferTestEntry buffer2Instance;
            using (DataSnapshot snapshot = pool.GetData())
            {
                buffer2Instance = snapshot.Target.GetInstance<BufferTestEntry>();
                Assert.IsNotNull(buffer2Instance);
            }

            // Check if we have the "same" entry but different instances of it (per buffer)
            Assert.AreEqual(buffer1Instance.TestInt, buffer2Instance.TestInt);
            buffer1Instance.TestInt = 5;
            Assert.AreNotEqual(buffer1Instance, buffer2Instance);
        }

        [Test]
        public void PoolSettingsTest()
        {
            var pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(new BufferedDataPoolSettings { MaxDatasetCount = 4 });
            Assert.AreEqual(4, pool.DatasetCount);

            pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(new BufferedDataPoolSettings { MaxDatasetCount = 3, DedicatedDatasets = 1 });
            Assert.AreEqual(3, pool.DatasetCount);
            Assert.AreEqual(1, pool.DedicatedDatasetCount);

            pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(new BufferedDataPoolSettings { MaxDatasetCount = 5, DedicatedDatasets = 2, UseDynamicDatasetAllocation = true });
            Assert.AreEqual(1, pool.DatasetCount); 
        }

        [Test]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void DedicatedBufferTest()
        {
            var pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(new BufferedDataPoolSettings { MaxDatasetCount = 2, DedicatedDatasets = 1 });
            Assert.AreEqual(2, pool.DatasetCount);
            Assert.AreEqual(1, pool.DedicatedDatasetCount);

            // Add a test entry
            pool.Enqueue(new BufferCommandWriteDiscreteInstance<BufferTestEntry>());
            this.AdvanceBuffers(pool);

            using (DataSnapshot snapshot = pool.GetDedicatedData())
            {
                pool.Enqueue(new BufferCommandWriteDiscreteInstance<BufferTestEntry>());

                //Assert.That(this.AdvanceBuffers(pool), Throws.TypeOf<InvalidOperationException>(), "Adding a command and forcing a swap should fail while we hold a lock on a dedicated buffer");
            }
        }

        [Test]
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void LockAndSwapTestStatic()
        {
            var pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(new BufferedDataPoolSettings { MaxDatasetCount = 4 });

            this.AdvanceBuffers(pool);
            Assert.AreEqual(1, pool.ActiveDataset);

            using (DataSnapshot lockOnDataset1 = pool.GetData())
            {
                this.AdvanceBuffers(pool);
                Assert.AreEqual(2, pool.ActiveDataset);
                using (DataSnapshot lockOnDataset2 = pool.GetData())
                {
                    // Cycling the buffers will now only bring us 0 and 3
                    this.AdvanceAndConfirmActiveDataset(pool, 3);
                    this.AdvanceAndConfirmActiveDataset(pool, 0);
                    this.AdvanceAndConfirmActiveDataset(pool, 3);
                }
            }
        }

        //[Test]
        public void LockAndSwapTestDynamic()
        {
            const int MinDatasetCount = 2;
            const int MaxDatasetCount = 4;

            var pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(
                new BufferedDataPoolSettings
                    {
                        MinDatasetCount = MinDatasetCount,
                        MaxDatasetCount = MaxDatasetCount,
                        UseDynamicDatasetAllocation = true
                    });

            this.AdvanceBuffers(pool);
            Assert.AreEqual(1, pool.ActiveDataset);

            int expectedDatasetCount = MinDatasetCount;
            Assert.AreEqual(expectedDatasetCount, pool.DatasetCount);
            IList<DataSnapshot> datasetLocks = new List<DataSnapshot>();
            for (var i = 0; i < MinDatasetCount; i++)
            {
                // Get a lock on all buffers
                datasetLocks.Add(pool.GetData());
                this.AdvanceBuffers(pool);
            }

            // We should now have more datasets in the pool
            expectedDatasetCount++;
            Assert.AreEqual(expectedDatasetCount, pool.DatasetCount);

            // Now fill up to max buffers - 1
            for (var i = 0; i < MaxDatasetCount - MinDatasetCount - 1; i++)
            {
                datasetLocks.Add(pool.GetData());
                this.AdvanceBuffers(pool);
            }

            expectedDatasetCount++;
            Assert.AreEqual(expectedDatasetCount, pool.DatasetCount);

            // Now we release all the buffers, this should bring us back to the min buffer count
            foreach (DataSnapshot snapshot in datasetLocks)
            {
                snapshot.Dispose();
            }

            datasetLocks.Clear();

            // Call update to trigger the buffer release
            pool.Update();

            Assert.AreEqual(MinDatasetCount, pool.DatasetCount);
        }

        [Test]
        public void DataConsistencyTest()
        {
            var pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(new BufferedDataPoolSettings { MaxDatasetCount = 3 });

            // Write an instance into the pool
            pool.Enqueue(new BufferCommandWriteDiscreteInstance<BufferTestEntry>());
            this.AdvanceBuffers(pool);

            // Update the instance with a proper command
            pool.Enqueue(new BufferTestEntryUpdateCommand(5));
            this.AdvanceBuffers(pool);

            using (DataSnapshot snapshot = pool.GetData())
            {
                var entry = snapshot.Target.GetInstance<BufferTestEntry>();
                Assert.AreEqual(5, entry.TestInt);
            }
        }

        [Test]
        public void DataConsistencyTestDynamic()
        {
            const int MinDatasetCount = 2;
            const int MaxDatasetCount = 4;

            var pool = BufferedDataTestSetup.Factory.Resolve<IBufferedDataPool>();
            pool.Initialize(new BufferedDataPoolSettings { MinDatasetCount = MinDatasetCount, MaxDatasetCount = MaxDatasetCount, UseDynamicDatasetAllocation = true });

            // Write an instance into the pool
            pool.Enqueue(new BufferCommandWriteDiscreteInstance<BufferTestEntry>());
            this.AdvanceBuffers(pool);

            pool.Enqueue(new BufferTestEntryUpdateCommand(5));
            this.AdvanceBuffers(pool);

            // Get a lock on all of the default datasets
            IList<DataSnapshot> datasetLocks = new List<DataSnapshot>();
            for (var i = 0; i < MinDatasetCount; i++)
            {
                // Get a lock on all buffers
                datasetLocks.Add(pool.GetData());
                this.AdvanceBuffers(pool);
            }

            // Now we ensure we are on a newly created buffer
            Assert.AreEqual(2, pool.ActiveDataset);
            using (DataSnapshot snapshot = pool.GetData())
            {
                // See if the buffer has the same data as the ones that existed before
                var instance = snapshot.Target.GetInstance<BufferTestEntry>();
                Assert.IsNotNull(instance);
                Assert.AreEqual(5, instance.TestInt);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void AdvanceBuffers(IBufferedDataPool pool)
        {
            pool.Commit();
            pool.Update();
        }

        private void AdvanceAndConfirmActiveDataset(IBufferedDataPool pool, int dataset)
        {
            this.AdvanceBuffers(pool);
            Assert.AreEqual(dataset, pool.ActiveDataset);
        }
    }
}
