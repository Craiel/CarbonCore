﻿namespace CarbonCore.Tests.ContentServices
{
    using System;

    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.Utils.Compat.Diagnostics;

    using NUnit.Framework;

    [TestFixture]
    public class DataEntryTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GeneralTests()
        {
            DataTestData.TestEntry.Id = 123;

            var clone = (DataTestEntry)DataTestData.TestEntry.Clone();
            Assert.AreNotEqual(clone, DataTestData.TestEntry, "Clone must be different instance of original entry");
            Assert.IsNull(clone.Id, "Clone id must be null");

            var clone2 = (DataTestEntry)clone.Clone();
            Assert.AreEqual(clone, clone2, "Direct clone with a non-set id must match");

            clone2.ByteArray = null;
            Assert.AreEqual(clone, clone2, "Direct clone must still match after equality ignore property set to null");

            clone2.TestString = "Another test clone";
            Assert.AreNotEqual(clone, clone2, "Clones must mis-match after changing data");
        }
        
        [Test]
        public void HashCodeTests()
        {
            // Custom GetHashCode()
            var customHashCode = (DataTestEntry)DataTestData.TestEntry.Clone();
            int originalHash = customHashCode.GetHashCode();

            customHashCode.TestFloat += 25.0f;
            Assert.AreEqual(originalHash, customHashCode.GetHashCode(), "Custom hash override should stay consistent even with property change");

            // Default GetHashCode
            var clone = (DataTestEntry2)DataTestData.TestEntry2.Clone();
            originalHash = clone.GetHashCode();

            float originalFloat = clone.OtherTestFloat;

            clone.OtherTestFloat += 15.0f;
            Assert.AreNotEqual(originalHash, clone.GetHashCode(), "Hashes should mismatch after property change");

            clone.OtherTestFloat = originalFloat;
            Assert.AreEqual(originalHash, clone.GetHashCode(), "Hash should revert to original");

            // Restore the data from the original
            clone.CopyFrom(DataTestData.TestEntry2);
            Assert.AreEqual(clone.GetHashCode(), DataTestData.TestEntry2.GetHashCode(), "Hashes of objects with the same properties should match");

            var firstEntry = new DataTestEntry2();
            var secondEntry = new DataTestEntry2();
            Assert.AreEqual(firstEntry.GetHashCode(), secondEntry.GetHashCode());
            Assert.AreEqual(firstEntry, secondEntry);

            // DataTestEntry3 is configured to use the original GetHashCode function even though
            //  it inherites from DataTestEntry2
            var thirdEntry = new DataTestEntry3();
            Assert.AreNotEqual(firstEntry.GetHashCode(), thirdEntry.GetHashCode());
            Assert.AreNotEqual(firstEntry, thirdEntry);
        }

        [Test]
        public void CloneTest()
        {
            var original = DataTestData.FullTestEntry;
            var clone = (DataTestEntry)original.Clone();
            
            // Check the deep clone
            Assert.AreEqual(6, clone.SimpleCollection.Count);
            Assert.AreEqual(3, clone.SimpleDictionary.Count);

            Assert.AreEqual(3, clone.CascadingCollection.Count);
            Assert.AreEqual(3, clone.CascadingDictionary.Count);

            Assert.AreEqual(clone.CascadedEntry, original.CascadedEntry);

            // Check general equality
            Assert.AreEqual(clone, original);

            clone.CascadedEntry.OtherTestLong = 998;
            Assert.AreNotEqual(clone.CascadedEntry.OtherTestLong, original.CascadedEntry.OtherTestLong);

            clone.SimpleDictionary.Add("Fourth", -5.0f);
            Assert.AreNotEqual(clone.SimpleDictionary.Count, original.SimpleDictionary.Count);
        }

        [Test]
        public void SerializationTests()
        {
            var clone = (DataTestEntry)DataTestData.FullTestEntry.Clone();

            // Test basic Json serialization
            byte[] jsonData = DataEntrySerialization.Save(clone);
            Assert.AreEqual(1223, jsonData.Length, "Serialization should return data");

            DataTestEntry restored = DataEntrySerialization.Load<DataTestEntry>(jsonData);
            Assert.NotNull(restored);

            // Test compact serialization
            byte[] compact = DataEntrySerialization.CompactSave(clone);
            Assert.AreEqual(363, compact.Length);
            Assert.Less(compact.Length, jsonData.Length, "Compact serialization should be smaller");

            DataTestEntry restoredCompact = DataEntrySerialization.CompactLoad<DataTestEntry>(compact);
            Assert.NotNull(restoredCompact);

            // Test native serialization
            var combinedClone = (SyncTestEntryCombined)DataTestData.CombinedTestEntry.Clone();

            byte[] native = DataEntrySerialization.NativeSave(combinedClone);
            Assert.AreEqual(363, native.Length);
            Assert.Less(compact.Length, jsonData.Length, "Native serialization should be smaller");

            DataTestEntry restoredNative = new DataTestEntry();
            DataEntrySerialization.NativeLoad(restoredNative, native);
        }

        [Test]
        public void SyncSerializationTest()
        {
            // Test Sync serialization
            byte[] native = DataEntrySerialization.SyncSave(DataTestData.SyncTestEntry);
            Assert.AreEqual(59, native.Length);

            SyncTestEntry restored = new SyncTestEntry();
            DataEntrySerialization.SyncLoad(restored, native);

            byte[] restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(native.Length, restoredData.Length);

            restored.ResetSyncState();
            restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(6, restoredData.Length);

            restored.TestFloat.Value = 15.0f;
            restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(11, restoredData.Length);
        }

        [Test]
        public void SerializationPerformanceTests()
        {
            int cycles = 5000;
            var clone = (DataTestEntry)DataTestData.FullTestEntry.Clone();

            long totalData = 0;
            using (new ProfileRegion("DataEntry.JsonSerialization"))
            {
                var metric = Metrics.BeginMetric();

                for (var i = 0; i < cycles; i++)
                {
                    byte[] data = DataEntrySerialization.Save(clone);
                    Assert.Greater(data.Length, 0);
                    totalData += data.Length;
                    DataEntrySerialization.Load<DataTestEntry>(data);
                    Metrics.TakeMeasure(metric);
                }

                Metrics.TraceMeasure(metric, "DataEntry.JsonSerialization");
            }

            System.Diagnostics.Trace.TraceInformation("JSON Serialized {0} data, average: {1}", totalData, totalData / cycles);

            GC.Collect();

            totalData = 0;
            using (new ProfileRegion("DataEntry.CompactSerialization"))
            {
                var metric = Metrics.BeginMetric();

                for (var i = 0; i < cycles; i++)
                {
                    byte[] data = DataEntrySerialization.CompactSave(clone);
                    Assert.Greater(data.Length, 0);
                    totalData += data.Length;
                    DataEntrySerialization.CompactLoad<DataTestEntry>(data);
                    Metrics.TakeMeasure(metric);
                }

                Metrics.TraceMeasure(metric, "DataEntry.CompactSerialization");
            }

            System.Diagnostics.Trace.TraceInformation("Compact Serialized {0} data, average: {1}", totalData, totalData / cycles);

            GC.Collect();

            totalData = 0;
            using (new ProfileRegion("DataEntry.SyncSerialization"))
            {
                var metric = Metrics.BeginMetric();

                for (var i = 0; i < cycles; i++)
                {
                    byte[] data = DataEntrySerialization.SyncSave(DataTestData.SyncTestEntry);
                    Assert.Greater(data.Length, 0);
                    totalData += data.Length;

                    SyncTestEntry restoredSync = new SyncTestEntry();
                    DataEntrySerialization.SyncLoad(restoredSync, data);
                    Metrics.TakeMeasure(metric);
                }

                Metrics.TraceMeasure(metric, "DataEntry.SyncSerialization");
            }

            System.Diagnostics.Trace.TraceInformation("Sync Serialized {0} data, average: {1}", totalData, totalData / cycles);

            Profiler.TraceProfilerStatistics();
        }
    }
}
