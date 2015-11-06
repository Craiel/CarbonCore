namespace CarbonCore.Tests.Compat.ContentServices
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers;
    using CarbonCore.Tests.Compat.ContentServices.Data;
    using CarbonCore.Utils.Compat;

    using NUnit.Framework;

    [TestFixture]
    public class SerializerTests
    {
        private const bool TestBool = true;

        private const double TestDouble = 54.321;

        private const float TestFloat = 123.45f;

        private const int TestInt = 987654321;

        private const long TestLong = 112233445566778899;

        private const string TestString = "This is a simple test string!";

        private static readonly byte[] TestByteArray = { 20, 30, 5, byte.MinValue, byte.MaxValue };
        
        private static readonly float[] TestFloatArray = { 15.2f, 9.999995f, 100000000001.2f, float.MinValue, float.MaxValue };

        private static readonly double[] TestDoubleArray = { 999999595959595595955521.2345d, -125.0f, 55153, double.MinValue, double.MaxValue };

        private static readonly int[] TestIntArray = { 500, -1000, 9321, int.MinValue, int.MaxValue };
        
        private static readonly long[] TestInt64Array = { 911111111111112, -5000, 123456, long.MinValue, long.MaxValue};

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void SimpleTypeSerialization()
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                long streamPosition = 0;

                BooleanSerializer.Instance.Serialize(stream, TestBool);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                BooleanSerializer.Instance.SerializeNullable(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                BooleanSerializer.Instance.SerializeNullable(stream, TestBool);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                ByteArraySerializer.Instance.Serialize(stream, TestByteArray);
                TestUtils.AssertStreamPos(stream, 8, ref streamPosition);

                ByteArraySerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                DoubleSerializer.Instance.Serialize(stream, TestDouble);
                TestUtils.AssertStreamPos(stream, 9, ref streamPosition);

                DoubleSerializer.Instance.SerializeNullable(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                DoubleSerializer.Instance.SerializeNullable(stream, TestDouble);
                TestUtils.AssertStreamPos(stream, 9, ref streamPosition);

                DoubleArraySerializer.Instance.Serialize(stream, TestDoubleArray);
                TestUtils.AssertStreamPos(stream, 45, ref streamPosition);

                DoubleArraySerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                FloatSerializer.Instance.Serialize(stream, TestFloat);
                TestUtils.AssertStreamPos(stream, 5, ref streamPosition);

                FloatSerializer.Instance.SerializeNullable(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                FloatSerializer.Instance.SerializeNullable(stream, TestFloat);
                TestUtils.AssertStreamPos(stream, 5, ref streamPosition);

                FloatArraySerializer.Instance.Serialize(stream, TestFloatArray);
                TestUtils.AssertStreamPos(stream, 25, ref streamPosition);

                FloatArraySerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                Int32Serializer.Instance.Serialize(stream, TestInt);
                TestUtils.AssertStreamPos(stream, 5, ref streamPosition);

                Int32Serializer.Instance.SerializeNullable(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                Int32Serializer.Instance.SerializeNullable(stream, TestInt);
                TestUtils.AssertStreamPos(stream, 5, ref streamPosition);

                Int32ArraySerializer.Instance.Serialize(stream, TestIntArray);
                TestUtils.AssertStreamPos(stream, 25, ref streamPosition);

                Int32ArraySerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                Int64Serializer.Instance.Serialize(stream, TestLong);
                TestUtils.AssertStreamPos(stream, 9, ref streamPosition);

                Int64Serializer.Instance.SerializeNullable(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                Int64Serializer.Instance.SerializeNullable(stream, TestLong);
                TestUtils.AssertStreamPos(stream, 9, ref streamPosition);

                Int64ArraySerializer.Instance.Serialize(stream, TestInt64Array);
                TestUtils.AssertStreamPos(stream, 45, ref streamPosition);

                Int64ArraySerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                StringSerializer.Instance.Serialize(stream, TestString);
                TestUtils.AssertStreamPos(stream, 32, ref streamPosition);

                StringSerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            Assert.AreEqual(249, data.Length);

            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);

                Assert.AreEqual(TestBool, BooleanSerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, BooleanSerializer.Instance.DeserializeNullable(stream));
                Assert.AreEqual((bool?)TestBool, BooleanSerializer.Instance.DeserializeNullable(stream));
                
                Assert.AreEqual(TestByteArray, ByteArraySerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, ByteArraySerializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestDouble, DoubleSerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, DoubleSerializer.Instance.DeserializeNullable(stream));
                Assert.AreEqual((double?)TestDouble, DoubleSerializer.Instance.DeserializeNullable(stream));

                Assert.AreEqual(TestDoubleArray, DoubleArraySerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, DoubleArraySerializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestFloat, FloatSerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, FloatSerializer.Instance.DeserializeNullable(stream));
                Assert.AreEqual((double?)TestFloat, FloatSerializer.Instance.DeserializeNullable(stream));

                Assert.AreEqual(TestFloatArray, FloatArraySerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, FloatArraySerializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestInt, Int32Serializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, Int32Serializer.Instance.DeserializeNullable(stream));
                Assert.AreEqual((double?)TestInt, Int32Serializer.Instance.DeserializeNullable(stream));

                Assert.AreEqual(TestIntArray, Int32ArraySerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, Int32ArraySerializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestLong, Int64Serializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, Int64Serializer.Instance.DeserializeNullable(stream));
                Assert.AreEqual((double?)TestLong, Int64Serializer.Instance.DeserializeNullable(stream));

                Assert.AreEqual(TestInt64Array, Int64ArraySerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, Int64ArraySerializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestString, StringSerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, StringSerializer.Instance.Deserialize(stream));

                Assert.AreEqual(stream.Position, stream.Length);
            }
        }

        [Test]
        public void ListIsChangedTests()
        {
            var simpleCollection = new SyncList<List<int>, int>();
            simpleCollection.AddRange(new List<int> { 5, 10, 15, 99, 2999 });

            var cascadedCollection = new SyncCascadeList<List<ISyncEntry>, ISyncEntry>();
            cascadedCollection.AddRange(
                new List<ISyncEntry>
                    {
                        DataTestData.SyncTestEntry2,
                        DataTestData.SyncTestEntry2,
                        new SyncTestEntry2
                            {
                                Id = { Value = "Last entry" },
                                OtherTestFloat = new Sync<float>(-15.15f)
                            }
                    });

            Assert.IsTrue(simpleCollection.IsChanged);
            Assert.IsTrue(cascadedCollection.IsChanged);

            simpleCollection.ResetChangeState();
            cascadedCollection.ResetChangeState();

            Assert.IsFalse(simpleCollection.IsChanged);
            Assert.IsFalse(cascadedCollection.IsChanged);

            // Addition
            simpleCollection.Add(5);
            Assert.IsTrue(simpleCollection.IsChanged);

            cascadedCollection.Add(new SyncTestEntry2());
            Assert.IsTrue(cascadedCollection.IsChanged);

            simpleCollection.ResetChangeState();
            cascadedCollection.ResetChangeState();

            // Indexers
            simpleCollection[0] = 15;
            Assert.IsTrue(simpleCollection.IsChanged);

            ((SyncTestEntry2)cascadedCollection[0]).OtherTestString.Value = "Changing a property in a cascaded collection";
            Assert.IsTrue(cascadedCollection.IsChanged);
        }

        [Test]
        public void DictionaryIsChangedTests()
        {
            var simpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>
                                       {
                                           { "First", 20 },
                                           { "Second", 19 },
                                           { "Third", 1 }
                                       };

            var cascadingDictionary = new SyncCascadeValueDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
            cascadingDictionary.Add(0, new SyncTestEntry2 { Id = { Value = "0" } });
            cascadingDictionary.Add(1, new SyncTestEntry2 { Id = { Value = "1" }, OtherTestLong = { Value = 99 } });
            cascadingDictionary.Add(50, new SyncTestEntry2 { Id = { Value = "Third" }, OtherTestString = { Value = "Still the third..." } });


            Assert.IsTrue(simpleDictionary.IsChanged);
            Assert.IsTrue(cascadingDictionary.IsChanged);

            simpleDictionary.ResetChangeState();
            cascadingDictionary.ResetChangeState();

            Assert.IsFalse(simpleDictionary.IsChanged);
            Assert.IsFalse(cascadingDictionary.IsChanged);

            // Addition
            simpleDictionary.Add("test", 15.5f);
            Assert.IsTrue(simpleDictionary.IsChanged);

            cascadingDictionary.Add(15, new SyncTestEntry2());
            Assert.IsTrue(cascadingDictionary.IsChanged);

            simpleDictionary.ResetChangeState();
            cascadingDictionary.ResetChangeState();

            // Indexers
            simpleDictionary["test"] = 125.9f;
            Assert.IsTrue(simpleDictionary.IsChanged);

            (cascadingDictionary[0]).OtherTestString.Value = "Changing a property in a cascaded collection";
            Assert.IsTrue(cascadingDictionary.IsChanged);
        }

        [Test]
        public void ListSerialization()
        {
            const int FirstTestSize = 28;
            const int SecondTestSize = 77;

            SyncList<List<int>, int> simpleCollection = new SyncList<List<int>, int>();
            simpleCollection.AddRange(new List<int> { 5, 10, 15, 99, 2999 });

            SyncCascadeList<List<ISyncEntry>, ISyncEntry> cascadedCollection = new SyncCascadeList<List<ISyncEntry>, ISyncEntry>();
            cascadedCollection.AddRange(
                new List<ISyncEntry>
                    {
                        new SyncTestEntry2
                            {
                                Id = { Value = "First entry" },
                            },
                        new SyncTestEntry2
                            {
                                Id = { Value = "Second entry" },
                                OtherTestBool =  new Sync<bool>(true)
                            },
                        new SyncTestEntry2
                            {
                                Id = { Value = "Last entry" },
                                OtherTestFloat = new Sync<float>(-15.15f)
                            }
                    });

            simpleCollection.ResetChangeState(true);
            cascadedCollection.ResetChangeState(true);

            byte[] data;
            using (var stream = new MemoryStream())
            {
                long position = stream.Position;
                NativeSerialization.SerializeList(
                    stream,
                    simpleCollection.IsChanged,
                    simpleCollection.Value,
                    Int32Serializer.Instance.Serialize);
                TestUtils.AssertStreamPos(stream, FirstTestSize, ref position);

                NativeSerialization.SerializeCascadeList(stream, cascadedCollection, false);
                TestUtils.AssertStreamPos(stream, SecondTestSize, ref position);

                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            Assert.AreEqual(FirstTestSize + SecondTestSize, data.Length);

            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                long position = stream.Position;

                var restoredSimpleCollection = new SyncList<List<int>, int>();
                NativeSerialization.DeserializeList(
                    stream,
                    restoredSimpleCollection.Value,
                    Int32Serializer.Instance.Deserialize);
                Assert.NotNull(restoredSimpleCollection);
                TestUtils.AssertListEquals(simpleCollection.Value, restoredSimpleCollection);
                TestUtils.AssertStreamPos(stream, FirstTestSize, ref position);

                var restoredCascadeCollection = new SyncCascadeList<List<ISyncEntry>, ISyncEntry>();
                NativeSerialization.DeserializeCascadeList(
                    stream,
                    restoredCascadeCollection,
                    () => new SyncTestEntry2());
                Assert.NotNull(restoredCascadeCollection);
                TestUtils.AssertListEquals(cascadedCollection.Value, restoredCascadeCollection);

                TestUtils.AssertStreamPos(stream, SecondTestSize, ref position);
            }

            cascadedCollection.ResetChangeState();
            ((SyncTestEntry2)cascadedCollection[0]).OtherTestString.Value = "modified";
            Assert.IsTrue(cascadedCollection.IsChanged);
            Assert.IsFalse(cascadedCollection.IsListChanged);

            int modifiedLength;
            int fullLength;
            using (var stream = new MemoryStream())
            {
                NativeSerialization.SerializeCascadeList(stream, cascadedCollection, false);
                modifiedLength = (int)stream.Length;
            }

            cascadedCollection.ResetChangeState(true);

            using (var stream = new MemoryStream())
            {
                NativeSerialization.SerializeCascadeList(stream, cascadedCollection, false);
                fullLength = (int)stream.Length;
            }

            Assert.Less(modifiedLength, fullLength);
        }

        [Test]
        public void DictionarySerialization()
        {
            const int FirstTestSize = 43;
            const int SecondTestSize = 86;

            var simpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>
                                       {
                                           { "First", 20 },
                                           { "Second", 19 },
                                           { "Third", 1 }
                                       };

            var cascadingDictionary = new SyncCascadeValueDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
            cascadingDictionary.Add(0, new SyncTestEntry2 { Id = { Value = "0" } });
            cascadingDictionary.Add(1, new SyncTestEntry2 { Id = { Value = "1" }, OtherTestLong = { Value = 99 } });
            cascadingDictionary.Add(50, new SyncTestEntry2 { Id = { Value = "Third" }, OtherTestString = { Value = "Still the third..." } });

            byte[] data;
            using (var stream = new MemoryStream())
            {
                long position = stream.Position;
                NativeSerialization.SerializeDictionary(
                    stream,
                    simpleDictionary.IsChanged,
                    simpleDictionary.Value,
                    StringSerializer.Instance.Serialize,
                    FloatSerializer.Instance.Serialize);
                TestUtils.AssertStreamPos(stream, FirstTestSize, ref position);

                NativeSerialization.SerializeCascadeValueDictionary(
                    stream,
                    cascadingDictionary,
                    Int32Serializer.Instance.Serialize,
                    false);
                TestUtils.AssertStreamPos(stream, SecondTestSize, ref position);

                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            Assert.AreEqual(FirstTestSize + SecondTestSize, data.Length);

            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                long position = stream.Position;

                var restoredSimpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>();
                NativeSerialization.DeserializeDictionary(
                    stream,
                    restoredSimpleDictionary.Value,
                    StringSerializer.Instance.Deserialize,
                    FloatSerializer.Instance.Deserialize);
                Assert.NotNull(restoredSimpleDictionary);
                TestUtils.AssertDictionaryEquals(simpleDictionary.Value, restoredSimpleDictionary);
                TestUtils.AssertStreamPos(stream, FirstTestSize, ref position);

                var restoredCascadeDictionary = new SyncCascadeValueDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
                NativeSerialization.DeserializeCascadeValueDictionary(
                        stream,
                        restoredCascadeDictionary, 
                        Int32Serializer.Instance.Deserialize,
                        () => new SyncTestEntry2());
                Assert.NotNull(restoredCascadeDictionary);
                TestUtils.AssertDictionaryEquals(cascadingDictionary.Value, restoredCascadeDictionary);

                TestUtils.AssertStreamPos(stream, SecondTestSize, ref position);
            }
        }
    }
}
