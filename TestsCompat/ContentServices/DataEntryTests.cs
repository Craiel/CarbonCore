﻿namespace CarbonCore.Tests.ContentServices
{
    using System;
    using System.Linq;

    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;
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
            Assert.AreEqual(clone, DataTestData.TestEntry, "Clone must be equal to original entry (custom equality)");

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

            Assert.AreEqual(2, clone.CascadingCollection.Count);
            Assert.AreEqual(3, clone.CascadingDictionary.Count);

            Assert.AreEqual(clone.CascadedEntry, original.CascadedEntry);

            // Check general equality
            Assert.AreNotEqual(clone, original);

            clone.CascadedEntry.OtherTestLong = 998;
            Assert.AreNotEqual(clone.CascadedEntry.OtherTestLong, original.CascadedEntry.OtherTestLong);

            clone.SimpleDictionary.Add("Fourth", -5.0f);
            Assert.AreNotEqual(clone.SimpleDictionary.Count, original.SimpleDictionary.Count);
        }

        [Test]
        public void CompactSerializationTest()
        {
            // Test compact serialization
            byte[] compact = DataEntrySerialization.CompactSave(DataTestData.FullTestEntry);
            Assert.AreEqual(332, compact.Length);

            DataTestEntry restoredCompact = DataEntrySerialization.CompactLoad<DataTestEntry>(compact);
            Assert.NotNull(restoredCompact);

            byte[] compact2 = DataEntrySerialization.CompactSave(restoredCompact);
            Assert.AreEqual(compact.Length, compact2.Length);

            // Try another instance
            compact = DataEntrySerialization.CompactSave(DataTestData.TestEntry);
            Assert.AreEqual(46, compact.Length);

            restoredCompact = DataEntrySerialization.CompactLoad<DataTestEntry>(compact);
            Assert.NotNull(restoredCompact);

            compact2 = DataEntrySerialization.CompactSave(restoredCompact);
            Assert.AreEqual(compact.Length, compact2.Length);
        }

        [Test]
        public void JsonSerializationTest()
        {
            // Test basic Json serialization
            byte[] jsonData = DataEntrySerialization.Save(DataTestData.FullTestEntry);
            Assert.AreEqual(1112, jsonData.Length, "Serialization should return data");

            DataTestEntry restored = DataEntrySerialization.Load<DataTestEntry>(jsonData);
            Assert.NotNull(restored);

            byte[] jsonData2 = DataEntrySerialization.Save(restored);
            Assert.AreEqual(jsonData.Length, jsonData2.Length);
        }

        [Test]
        public void SyncSerializationTest()
        {
            // Mark everything as changed first so we get accurate results
            DataTestData.SyncTestEntry.ResetChangeState(true);

            // Test Sync serialization
            byte[] native = DataEntrySerialization.SyncSave(DataTestData.SyncTestEntry);
            Assert.AreEqual(345, native.Length);

            // Reset the change state and test again
            DataTestData.SyncTestEntry.ResetChangeState();
            native = DataEntrySerialization.SyncSave(DataTestData.SyncTestEntry);
            Assert.AreEqual(13, native.Length);

            // Test saving with ignoring the change state
            native = DataEntrySerialization.SyncSave(DataTestData.SyncTestEntry, true);
            Assert.AreEqual(345, native.Length);

            SyncTestEntry restored = new SyncTestEntry();
            SyncTestEntry restored2 = new SyncTestEntry();
            DataEntrySerialization.SyncLoad(restored, native);
            DataEntrySerialization.SyncLoad(restored2, native);
            TestUtils.AssertInstanceEquals(DataTestData.SyncTestEntry, restored);
            TestUtils.AssertInstanceEquals(restored, restored2);

            byte[] restoredData = DataEntrySerialization.SyncSave(restored, true);
            Assert.IsTrue(native.SequenceEqual(restoredData));

            restored.ResetChangeState();
            restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(13, restoredData.Length);

            // Modify simple types
            restored.TestFloat.Value = 15.0f;
            restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(18, restoredData.Length);

            DataEntrySerialization.SyncLoad(restored2, restoredData);
            TestUtils.AssertInstanceEquals(restored, restored2);

            // Modify collections
            restored.SimpleCollection.Add(1001);
            restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(56, restoredData.Length);

            DataEntrySerialization.SyncLoad(restored2, restoredData);
            Assert.IsTrue(restored.SimpleCollection.SequenceEqual(restored2.SimpleCollection));

            // Add and modify cascading entries
            restored.CascadingCollection.Add(new SyncTestEntry2());
            restored.CascadingCollection[0].OtherTestFloat.Value = -10.0f;
            restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(134, restoredData.Length);

            DataEntrySerialization.SyncLoad(restored2, restoredData);
            Assert.IsTrue(restored.CascadingCollection.SequenceEqual(restored2.CascadingCollection));

            // Modify Dictionaries
            restored.SimpleDictionary.Add("TestDict", 123);
            restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(193, restoredData.Length);

            DataEntrySerialization.SyncLoad(restored2, restoredData);
            Assert.IsTrue(restored.SimpleDictionary.SequenceEqual(restored2.SimpleDictionary));

            // Add and modify cascading entries
            restored.CascadingDictionary.Add(1001, new SyncTestEntry2());
            restored.CascadingDictionary[0].OtherTestFloat.Value = -10.0f;
            restoredData = DataEntrySerialization.SyncSave(restored);
            Assert.AreEqual(297, restoredData.Length);

            DataEntrySerialization.SyncLoad(restored2, restoredData);
            Assert.IsTrue(restored.CascadingDictionary.SequenceEqual(restored2.CascadingDictionary));
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
