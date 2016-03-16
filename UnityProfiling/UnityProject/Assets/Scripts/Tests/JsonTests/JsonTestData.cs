namespace Assets.Scripts.Tests.JsonTests
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    using UnityEngine;

    using Random = System.Random;

    [DataEntry(UseDefaultEquality = true)]
    public class JsonTestData : DataEntry
    {
        private static readonly Random Rand = new Random();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string StringValue { get; set; }

        public int IntValue { get; set; }

        public bool BoolValue { get; set; }

        public float[] FloatArray { get; set; }

        public List<int> IntList { get; set; }

        public Vector3 Vector { get; set; }

        public Quaternion Quaternion { get; set; }

        public JsonTestDataChild Child { get; set; }

        [JsonConverter(typeof(JsonDictionaryConverter<int, JsonTestDataChild>))]
        public Dictionary<int, JsonTestDataChild> ChildDictionary { get; set; }

        public void RandomizeContent(char stringChar)
        {
            this.StringValue = new string(stringChar, Rand.Next(5, 25));
            this.IntValue = Rand.Next(5, 25);
            this.BoolValue = Rand.Next(0, 2) == 1;
            this.FloatArray = new float[Rand.Next(10, 50)];
            for (var i = 0; i < this.FloatArray.Length; i++)
            {
                this.FloatArray[i] = (float)Rand.NextDouble();
            }

            int count = Rand.Next(10, 50);
            this.IntList = new List<int>();
            for (var i = 0; i < count; i++)
            {
                this.IntList.Add(Rand.Next());
            }

            this.Vector = new Vector3((float)Rand.NextDouble(), (float)Rand.NextDouble(), (float)Rand.NextDouble());
            this.Quaternion = Quaternion.AngleAxis((float)Rand.NextDouble(), this.Vector);

            this.Child = new JsonTestDataChild { ParentString = this.StringValue };

            this.ChildDictionary = new Dictionary<int, JsonTestDataChild>();
            count = Rand.Next(2, 8);
            for (var i = 0; i < count; i++)
            {
                var child = new JsonTestDataChild { ParentString = this.StringValue };
                this.ChildDictionary.Add(child.Id, child);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            throw new InvalidOperationException();
        }
    }
}
