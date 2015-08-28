namespace CarbonCore.Tests.Compat.ContentServices
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers;
    using CarbonCore.Tests.ContentServices;
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

        private static readonly byte[] TestByteArray = { 20, 30, 5 };

        private const string TestString = "This is a simple test string!";

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

                BooleanSerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                BooleanSerializer.Instance.Serialize(stream, (bool?)TestBool);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                ByteArraySerializer.Instance.Serialize(stream, TestByteArray);
                TestUtils.AssertStreamPos(stream, 6, ref streamPosition);

                ByteArraySerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                DoubleSerializer.Instance.Serialize(stream, TestDouble);
                TestUtils.AssertStreamPos(stream, 9, ref streamPosition);

                DoubleSerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                DoubleSerializer.Instance.Serialize(stream, (double?)TestDouble);
                TestUtils.AssertStreamPos(stream, 9, ref streamPosition);

                FloatSerializer.Instance.Serialize(stream, TestFloat);
                TestUtils.AssertStreamPos(stream, 5, ref streamPosition);

                FloatSerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                FloatSerializer.Instance.Serialize(stream, (float?)TestFloat);
                TestUtils.AssertStreamPos(stream, 5, ref streamPosition);

                Int32Serializer.Instance.Serialize(stream, TestInt);
                TestUtils.AssertStreamPos(stream, 5, ref streamPosition);

                Int32Serializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                Int32Serializer.Instance.Serialize(stream, (int?)TestInt);
                TestUtils.AssertStreamPos(stream, 5, ref streamPosition);

                Int64Serializer.Instance.Serialize(stream, TestLong);
                TestUtils.AssertStreamPos(stream, 9, ref streamPosition);

                Int64Serializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                Int64Serializer.Instance.Serialize(stream, (long?)TestLong);
                TestUtils.AssertStreamPos(stream, 9, ref streamPosition);

                StringSerializer.Instance.Serialize(stream, TestString);
                TestUtils.AssertStreamPos(stream, 32, ref streamPosition);

                StringSerializer.Instance.Serialize(stream, null);
                TestUtils.AssertStreamPos(stream, 1, ref streamPosition);

                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            Assert.AreEqual(103, data.Length);

            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);

                Assert.AreEqual(TestBool, BooleanSerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, BooleanSerializer.Instance.Deserialize(stream));
                Assert.AreEqual((bool?)TestBool, BooleanSerializer.Instance.Deserialize(stream));
                
                Assert.AreEqual(TestByteArray, ByteArraySerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, ByteArraySerializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestDouble, DoubleSerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, DoubleSerializer.Instance.Deserialize(stream));
                Assert.AreEqual((double?)TestDouble, DoubleSerializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestFloat, FloatSerializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, FloatSerializer.Instance.Deserialize(stream));
                Assert.AreEqual((double?)TestFloat, FloatSerializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestInt, Int32Serializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, Int32Serializer.Instance.Deserialize(stream));
                Assert.AreEqual((double?)TestInt, Int32Serializer.Instance.Deserialize(stream));

                Assert.AreEqual(TestLong, Int64Serializer.Instance.Deserialize(stream));
                Assert.AreEqual(null, Int64Serializer.Instance.Deserialize(stream));
                Assert.AreEqual((double?)TestLong, Int64Serializer.Instance.Deserialize(stream));

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
            var simpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>();
            simpleDictionary.Add("First", 20);
            simpleDictionary.Add("Second", 19);
            simpleDictionary.Add("Third", 1);

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
            const int FirstTestSize = 29;
            const int SecondTestSize = 77;

            SyncList<List<int>, int> simpleCollection = new SyncList<List<int>, int>();
            simpleCollection.AddRange(new List<int> { 5, 10, 15, 99, 2999 });

            SyncList<List<ISyncEntry>, ISyncEntry> cascadedCollection = new SyncList<List<ISyncEntry>, ISyncEntry>();
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

                NativeSerialization.SerializeList(
                    stream,
                    cascadedCollection.IsChanged,
                    cascadedCollection.Value,
                    (targetStream, value) => value.Save(targetStream));
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
                NativeSerialization.DeserializeList<int>(
                    stream,
                    restoredSimpleCollection.Value,
                    Int32Serializer.Instance.Deserialize);
                Assert.NotNull(restoredSimpleCollection);
                TestUtils.AssertListEquals(simpleCollection.Value, restoredSimpleCollection);
                TestUtils.AssertStreamPos(stream, FirstTestSize, ref position);

                var restoredCascadeCollection = new SyncList<List<ISyncEntry>, ISyncEntry>();
                NativeSerialization.DeserializeList<ISyncEntry>(
                        stream,
                        restoredCascadeCollection.Value,
                        source =>
                            {
                                var entry = new SyncTestEntry2();
                                entry.Load(source);
                                return entry;
                            });
                Assert.NotNull(restoredCascadeCollection);
                TestUtils.AssertListEquals(cascadedCollection.Value, restoredCascadeCollection);

                TestUtils.AssertStreamPos(stream, SecondTestSize, ref position);
            }
        }

        [Test]
        public void DictionarySerialization()
        {
            const int FirstTestSize = 44;
            const int SecondTestSize = 86;

            var simpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>();
            simpleDictionary.Add("First", 20);
            simpleDictionary.Add("Second", 19);
            simpleDictionary.Add("Third", 1);

            var cascadingDictionary = new SyncDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
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

                NativeSerialization.SerializeDictionary(
                    stream,
                    cascadingDictionary.IsChanged,
                    cascadingDictionary.Value,
                    Int32Serializer.Instance.Serialize,
                    (targetStream, value) => value.Save(targetStream));
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
                NativeSerialization.DeserializeDictionary<string, float>(
                    stream,
                    restoredSimpleDictionary.Value,
                    StringSerializer.Instance.Deserialize,
                    FloatSerializer.Instance.Deserialize);
                Assert.NotNull(restoredSimpleDictionary);
                TestUtils.AssertDictionaryEquals(simpleDictionary.Value, restoredSimpleDictionary);
                TestUtils.AssertStreamPos(stream, FirstTestSize, ref position);

                var restoredCascadeDictionary = new SyncDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
                NativeSerialization.DeserializeDictionary<int, SyncTestEntry2>(
                        stream,
                        restoredCascadeDictionary.Value,
                        Int32Serializer.Instance.Deserialize,
                        source =>
                        {
                            var entry = new SyncTestEntry2();
                            entry.Load(source);
                            return entry;
                        });
                Assert.NotNull(restoredCascadeDictionary);
                TestUtils.AssertDictionaryEquals(cascadingDictionary.Value, restoredCascadeDictionary);

                TestUtils.AssertStreamPos(stream, SecondTestSize, ref position);
            }
        }
    }
}
