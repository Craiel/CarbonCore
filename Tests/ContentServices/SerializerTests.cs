namespace CarbonCore.Tests.ContentServices
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers;

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
        public void ListSerialization()
        {
            SyncList<List<int>, int> simpleCollection = new SyncList<List<int>, int>();
            simpleCollection.Value = new List<int> { 5, 10, 15, 99, 2999 };

            SyncList<List<ISyncEntry>, ISyncEntry> cascadedCollection = new SyncList<List<ISyncEntry>, ISyncEntry>();
            cascadedCollection.Value = new List<ISyncEntry>
                                                             {
                                                                 DataTestData.SyncTestEntry2,
                                                                 DataTestData.SyncTestEntry2,
                                                                 new SyncTestEntry2
                                                                     {
                                                                         Id = {Value = "Last entry"},
                                                                         OtherTestFloat =
                                                                             -15.15f
                                                                     }
                                                             };

            byte[] data;
            using (var stream = new MemoryStream())
            {
                long position = stream.Position;
                NativeSerialization.SerializeList(
                    stream,
                    simpleCollection.IsChanged,
                    simpleCollection.Value,
                    Int32Serializer.Instance.Serialize);
                TestUtils.AssertStreamPos(stream, 29, ref position);

                NativeSerialization.SerializeList(
                    stream,
                    cascadedCollection.IsChanged,
                    cascadedCollection.Value,
                    (targetStream, value) => value.Save(targetStream));
                TestUtils.AssertStreamPos(stream, 154, ref position);

                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            Assert.AreEqual(183, data.Length);

            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                long position = stream.Position;

                var restoredSimpleCollection = new SyncList<IList<int>, int>();
                restoredSimpleCollection.Value = NativeSerialization.DeserializeList<int>(
                    stream,
                    null, 
                    () => new List<int>(),
                    Int32Serializer.Instance.Deserialize);
                Assert.NotNull(restoredSimpleCollection);
                TestUtils.AssertListEquals(simpleCollection.Value, restoredSimpleCollection);
                TestUtils.AssertStreamPos(stream, 29, ref position);

                var restoredCascadeCollection = new SyncList<IList<ISyncEntry>, ISyncEntry>();
                restoredCascadeCollection.Value =
                    NativeSerialization.DeserializeList<ISyncEntry>(
                        stream,
                        null,
                        () => new List<ISyncEntry>(),
                        source =>
                            {
                                var entry = new SyncTestEntry2();
                                entry.Load(source);
                                return entry;
                            });
                Assert.NotNull(restoredCascadeCollection);
                TestUtils.AssertListEquals(cascadedCollection.Value, restoredCascadeCollection);

                TestUtils.AssertStreamPos(stream, 154, ref position);
            }
        }

        [Test]
        public void DictionarySerialization()
        {
            var simpleDictionary = new SyncDictionary<Dictionary<string, float>, string, float>();
            simpleDictionary.Value = new Dictionary<string, float>
                                   {
                                       { "First", 20.0f },
                                       { "Second", 19.0f },
                                       { "Third", 1.0f }
                                   };

            var cascadingDictionary = new SyncDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
            cascadingDictionary.Value = new Dictionary<int, SyncTestEntry2>
                                    {
                                        { 0, new SyncTestEntry2 { Id = {Value = "0"} } },
                                        { 1, new SyncTestEntry2 { Id = {Value = "1"}, OtherTestLong = 99 } },
                                        { 50, new SyncTestEntry2 { Id = {Value = "Third"}, OtherTestString = {Value = "Still the third..."} } }
                                    };

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
                TestUtils.AssertStreamPos(stream, 44, ref position);

                NativeSerialization.SerializeDictionary(
                    stream,
                    cascadingDictionary.IsChanged,
                    cascadingDictionary.Value,
                    Int32Serializer.Instance.Serialize,
                    (targetStream, value) => value.Save(targetStream));
                TestUtils.AssertStreamPos(stream, 86, ref position);

                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            Assert.AreEqual(130, data.Length);

            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                long position = stream.Position;

                var restoredSimpleDictionary = new SyncDictionary<IDictionary<string, float>, string, float>();
                restoredSimpleDictionary.Value = NativeSerialization.DeserializeDictionary<string, float>(
                    stream,
                    null,
                    () => new Dictionary<string, float>(), 
                    StringSerializer.Instance.Deserialize,
                    FloatSerializer.Instance.Deserialize);
                Assert.NotNull(restoredSimpleDictionary);
                TestUtils.AssertDictionaryEquals(simpleDictionary.Value, restoredSimpleDictionary);
                TestUtils.AssertStreamPos(stream, 44, ref position);

                var restoredCascadeDictionary = new SyncDictionary<IDictionary<int, SyncTestEntry2>, int, SyncTestEntry2>();
                restoredCascadeDictionary.Value =
                    NativeSerialization.DeserializeDictionary<int, SyncTestEntry2>(
                        stream,
                        null,
                        () => new Dictionary<int, SyncTestEntry2>(),
                        Int32Serializer.Instance.Deserialize,
                        source =>
                        {
                            var entry = new SyncTestEntry2();
                            entry.Load(source);
                            return entry;
                        });
                Assert.NotNull(restoredCascadeDictionary);
                TestUtils.AssertDictionaryEquals(cascadingDictionary.Value, restoredCascadeDictionary);

                TestUtils.AssertStreamPos(stream, 86, ref position);
            }
        }
    }
}
