namespace CarbonCore.Tests.ContentServices
{
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers;

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
                this.AssertStreamPos(stream, 1, ref streamPosition);

                BooleanSerializer.Instance.Serialize(stream, null);
                this.AssertStreamPos(stream, 1, ref streamPosition);

                BooleanSerializer.Instance.Serialize(stream, (bool?)TestBool);
                this.AssertStreamPos(stream, 1, ref streamPosition);

                ByteArraySerializer.Instance.Serialize(stream, TestByteArray);
                this.AssertStreamPos(stream, 6, ref streamPosition);

                ByteArraySerializer.Instance.Serialize(stream, null);
                this.AssertStreamPos(stream, 1, ref streamPosition);

                DoubleSerializer.Instance.Serialize(stream, TestDouble);
                this.AssertStreamPos(stream, 9, ref streamPosition);

                DoubleSerializer.Instance.Serialize(stream, null);
                this.AssertStreamPos(stream, 1, ref streamPosition);

                DoubleSerializer.Instance.Serialize(stream, (double?)TestDouble);
                this.AssertStreamPos(stream, 9, ref streamPosition);

                FloatSerializer.Instance.Serialize(stream, TestFloat);
                this.AssertStreamPos(stream, 5, ref streamPosition);

                FloatSerializer.Instance.Serialize(stream, null);
                this.AssertStreamPos(stream, 1, ref streamPosition);

                FloatSerializer.Instance.Serialize(stream, (float?)TestFloat);
                this.AssertStreamPos(stream, 5, ref streamPosition);

                Int32Serializer.Instance.Serialize(stream, TestInt);
                this.AssertStreamPos(stream, 5, ref streamPosition);

                Int32Serializer.Instance.Serialize(stream, null);
                this.AssertStreamPos(stream, 1, ref streamPosition);

                Int32Serializer.Instance.Serialize(stream, (int?)TestInt);
                this.AssertStreamPos(stream, 5, ref streamPosition);

                Int64Serializer.Instance.Serialize(stream, TestLong);
                this.AssertStreamPos(stream, 9, ref streamPosition);

                Int64Serializer.Instance.Serialize(stream, null);
                this.AssertStreamPos(stream, 1, ref streamPosition);

                Int64Serializer.Instance.Serialize(stream, (long?)TestLong);
                this.AssertStreamPos(stream, 9, ref streamPosition);

                StringSerializer.Instance.Serialize(stream, TestString);
                this.AssertStreamPos(stream, 32, ref streamPosition);

                StringSerializer.Instance.Serialize(stream, null);
                this.AssertStreamPos(stream, 1, ref streamPosition);

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
            Sync<List<int>> simpleCollection = new List<int> { 5, 10, 15, 99, 2999 };

            Sync<List<ISyncEntry>> cascadedCollection = new List<ISyncEntry>
                                                             {
                                                                 DataTestData.SyncTestEntry2,
                                                                 DataTestData.SyncTestEntry2,
                                                                 new SyncTestEntry2
                                                                     {
                                                                         Id = "Last entry",
                                                                         OtherTestFloat =
                                                                             -15.15f
                                                                     }
                                                             };

            byte[] data;
            using (var stream = new MemoryStream())
            {
                long position = stream.Position;
                NativeSerialization.SerializeList<int>(
                    stream,
                    simpleCollection.IsChanged,
                    simpleCollection.Value,
                    Int32Serializer.Instance.Serialize);
                this.AssertStreamPos(stream, 29, ref position);

                NativeSerialization.SerializeList(
                    stream,
                    cascadedCollection.IsChanged,
                    cascadedCollection.Value,
                    (targetStream, value) => value.Save(targetStream));
                this.AssertStreamPos(stream, 154, ref position);

                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            Assert.AreEqual(183, data.Length);

            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                long position = stream.Position;
                
                SyncList<IList<int>, int> restoredSimpleCollection = NativeSerialization.DeserializeList<int>(
                    stream,
                    null, 
                    () => new List<int>(),
                    Int32Serializer.Instance.Deserialize);
                Assert.NotNull(restoredSimpleCollection);
                this.AssertListEquals(simpleCollection.Value, restoredSimpleCollection);
                this.AssertStreamPos(stream, 29, ref position);

                SyncList<IList<ISyncEntry>, ISyncEntry> restoredCascadeCollection =
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
                this.AssertListEquals(cascadedCollection.Value, restoredCascadeCollection);

                this.AssertStreamPos(stream, 154, ref position);
            }
        }

        [Test]
        public void DictionarySerialization()
        {
            SyncDictionary<Dictionary<string, float>, string, float> simpleDictionary = new Dictionary<string, float>
                                   {
                                       { "First", 20.0f },
                                       { "Second", 19.0f },
                                       { "Third", 1.0f }
                                   };

            SyncDictionary<Dictionary<int, SyncTestEntry2>, int, SyncTestEntry2> cascadingDictionary = new Dictionary<int, SyncTestEntry2>
                                    {
                                        { 0, new SyncTestEntry2 { Id = "0" } },
                                        { 1, new SyncTestEntry2 { Id = "1", OtherTestLong = 99 } },
                                        { 50, new SyncTestEntry2 { Id = "Third", OtherTestString = "Still the third..." } }
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
                this.AssertStreamPos(stream, 44, ref position);

                NativeSerialization.SerializeDictionary(
                    stream,
                    cascadingDictionary.IsChanged,
                    cascadingDictionary.Value,
                    Int32Serializer.Instance.Serialize,
                    (targetStream, value) => value.Save(targetStream));
                this.AssertStreamPos(stream, 86, ref position);

                data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
            }

            Assert.AreEqual(130, data.Length);

            using (var stream = new MemoryStream(data))
            {
                stream.Seek(0, SeekOrigin.Begin);
                long position = stream.Position;

                SyncDictionary<IDictionary<string, float>, string, float> restoredSimpleDictionary = NativeSerialization.DeserializeDictionary<string, float>(
                    stream,
                    null,
                    () => new Dictionary<string, float>(), 
                    StringSerializer.Instance.Deserialize,
                    FloatSerializer.Instance.Deserialize);
                Assert.NotNull(restoredSimpleDictionary);
                this.AssertDictionaryEquals(simpleDictionary.Value, restoredSimpleDictionary);
                this.AssertStreamPos(stream, 44, ref position);

                SyncDictionary<IDictionary<int, SyncTestEntry2>, int, SyncTestEntry2> restoredCascadeDictionary =
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
                this.AssertDictionaryEquals(cascadingDictionary.Value, restoredCascadeDictionary);

                this.AssertStreamPos(stream, 86, ref position);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void AssertStreamPos(Stream stream, long expectedCount, ref long lastPosition)
        {
            Assert.AreEqual(expectedCount, stream.Position - lastPosition);
            lastPosition = stream.Position;
        }

        private void AssertListEquals<T>(IList<T> first, IList<T> second)
        {
            Assert.AreEqual(first.Count, second.Count);
            for (var i = 0; i < first.Count; i++)
            {
                this.AssertInstanceEquals(first[i], second[i]);
            }
        }

        private void AssertDictionaryEquals<T, TN>(IDictionary<T, TN> first, IDictionary<T, TN> second)
        {
            Assert.AreEqual(first.Count, second.Count);

            IList<T> firstKeys = new List<T>(first.Keys);
            IList<T> secondKeys = new List<T>(second.Keys);
            this.AssertListEquals(firstKeys, secondKeys);

            IList<TN> firstValues = new List<TN>(first.Values);
            IList<TN> secondValues = new List<TN>(second.Values);
            this.AssertListEquals(firstValues, secondValues);
        }

        private void AssertInstanceEquals<T>(T first, T second)
        {
            if ((typeof(T).IsClass || typeof(T).IsInterface) && typeof(T) != typeof(string))
            {
                IList<PropertyInfo> properties = first.GetType().GetProperties();
                foreach (PropertyInfo info in properties)
                {
                    var firstValue = info.GetValue(first);
                    var secondValue = info.GetValue(second);
                    Assert.AreEqual(firstValue, secondValue);
                }
            }
            else
            {
                Assert.AreEqual(first, second);
            }
        }
    }
}
