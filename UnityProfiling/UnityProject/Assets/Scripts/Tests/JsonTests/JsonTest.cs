namespace Assets.Scripts.Tests.JsonTests
{
    using System.Linq;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Json;
    using CarbonCore.Utils.Unity.Logic.Json;

    using Newtonsoft.Json;

    using UnityEngine.Assertions;

    public class JsonTest
    {
        public static readonly JsonConverter[] JsonConverters =
            {
                new Vector3ConverterSmall(), new QuaternionConverterSmall(), new ColorConverter()
            };

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Run()
        {
            var firstObject = new JsonTestData();
            var secondObject = new JsonTestData();

            firstObject.RandomizeContent('a');
            secondObject.RandomizeContent('b');

            string firstStringData = JsonExtensions.SaveToData(firstObject, Formatting.None, JsonConverters);
            string firstStringData2 = JsonExtensions.SaveToData(firstObject, Formatting.None, JsonConverters);
            
            byte[] firstByteData = JsonExtensions.SaveToByte(firstObject, false, Formatting.None, JsonConverters);
            byte[] firstByteData2 = JsonExtensions.SaveToByte(firstObject, false, Formatting.None, JsonConverters);

            // Check that serializing multiple times results in the same data
            Assert.AreEqual(firstStringData, firstStringData2);
            Assert.IsTrue(firstByteData.SequenceEqual(firstByteData2));

            string secondStringData = JsonExtensions.SaveToData(secondObject, Formatting.None, JsonConverters);
            byte[] secondByteData = JsonExtensions.SaveToByte(secondObject, false, Formatting.None, JsonConverters);

            // Check that we serialized different data
            Assert.AreNotEqual(firstStringData, secondStringData);
            Assert.IsFalse(firstByteData.SequenceEqual(secondByteData));

            Diagnostic.Info("Serialized A: {0} str, {1} bytes", firstStringData.Length, firstByteData.Length);
            Diagnostic.Info("Serialized B: {0} str, {1} bytes", secondStringData.Length, secondByteData.Length);

            // Deserialize
            JsonTestData firstDeserialized = JsonExtensions.LoadFromData<JsonTestData>(firstStringData, JsonConverters);
            JsonTestData secondDeserialized = JsonExtensions.LoadFromByte<JsonTestData>(secondByteData, false, JsonConverters);
            
            Assert.IsTrue(firstObject.IntList.SequenceEqual(firstDeserialized.IntList));
            Assert.IsTrue(firstObject.FloatArray.SequenceEqual(firstDeserialized.FloatArray));
            Assert.IsTrue(firstObject.ChildDictionary.SequenceEqual(firstDeserialized.ChildDictionary));

            Assert.AreNotEqual(firstDeserialized, secondDeserialized);
        }
    }
}
