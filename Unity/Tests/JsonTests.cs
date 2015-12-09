namespace CarbonCore.Tests.Unity
{
    using System.Collections.Generic;

    using CarbonCore.Tests.Unity.Data;
    using CarbonCore.Utils.Json;

    using NUnit.Framework;

    [TestFixture]
    public class JsonTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void SerializationOfDictionary()
        {
            var testData = new JsonTestClass
                               {
                                   KeyDictionary = new Dictionary<JsonTestSubClass, int>(),
                                   KeyValueDictionary = new Dictionary<JsonTestSubClass, JsonTestSubClass>(),
                                   ValueDictionary = new Dictionary<string, JsonTestSubClass>()
                               };

            testData.KeyDictionary.Add(new JsonTestSubClass { String = "First entry" }, 500);
            testData.KeyDictionary.Add(new JsonTestSubClass { String = "Another entry" }, 10);

            testData.ValueDictionary.Add("123", new JsonTestSubClass { Float = 15.0f });
            testData.KeyValueDictionary.Add(
                new JsonTestSubClass { Int = 55 },
                new JsonTestSubClass { String = "Combination entry" });

            string data = JsonExtensions.SaveToData(testData);
            JsonTestClass loadedInstance = JsonExtensions.LoadFromData<JsonTestClass>(data);
            Assert.AreEqual(loadedInstance.KeyDictionary.Count, testData.KeyDictionary.Count);
            Assert.AreEqual(loadedInstance.KeyValueDictionary.Count, testData.KeyValueDictionary.Count);
            Assert.AreEqual(loadedInstance.ValueDictionary.Count, testData.ValueDictionary.Count);

            string data2 = JsonExtensions.SaveToData(loadedInstance);
            Assert.AreEqual(data2, data);
        }

        [Test]
        public void SerializationOfEmptyDictionary()
        {
            var testData = new JsonTestClass
                               {
                                   KeyDictionary = new Dictionary<JsonTestSubClass, int>(),
                                   KeyValueDictionary = new Dictionary<JsonTestSubClass, JsonTestSubClass>(),
                                   ValueDictionary = new Dictionary<string, JsonTestSubClass>()
                               };

            string data = JsonExtensions.SaveToData(testData);
            JsonTestClass loadedInstance = JsonExtensions.LoadFromData<JsonTestClass>(data);
            Assert.AreEqual(loadedInstance.KeyDictionary.Count, testData.KeyDictionary.Count);
            Assert.AreEqual(loadedInstance.KeyValueDictionary.Count, testData.KeyValueDictionary.Count);
            Assert.AreEqual(loadedInstance.ValueDictionary.Count, testData.ValueDictionary.Count);
        }

        [Test]
        public void SerializationOfNullDictionary()
        {
            var testData = new JsonTestClass
                               {
                                   KeyDictionary = new Dictionary<JsonTestSubClass, int>(),
                                   KeyValueDictionary = null,
                                   ValueDictionary = new Dictionary<string, JsonTestSubClass>()
                               };

            string data = JsonExtensions.SaveToData(testData);
            JsonTestClass loadedInstance = JsonExtensions.LoadFromData<JsonTestClass>(data);
            Assert.AreEqual(loadedInstance.KeyDictionary, testData.KeyDictionary);
            Assert.AreEqual(loadedInstance.KeyValueDictionary, testData.KeyValueDictionary);
            Assert.AreEqual(loadedInstance.ValueDictionary, testData.ValueDictionary);
        }
    }
}
