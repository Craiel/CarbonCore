namespace Assets.Scripts.Tests
{
    using System.Collections.Generic;
    
    public static class DictionaryTest
    {
        private static readonly IDictionary<int, string> TestDictionary;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static DictionaryTest()
        {
            TestDictionary = new Dictionary<int, string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Run(int entryCount, int lookupCount, bool useRandomIndex)
        {
            if (TestDictionary.Count != entryCount)
            {
                RebuildDictionary(entryCount);
            }

            long successs = 0;
            for (var i = 0; i < lookupCount; i++)
            {
                if (useRandomIndex)
                {
                    int index = UnityEngine.Random.Range(0, TestDictionary.Count);
                    string value;
                    if (TestDictionary.TryGetValue(index, out value))
                    {
                        successs += value.Length;
                    }
                }
                else
                {
                    successs += TestDictionary[i].Length;
                }
            }
        }

        private static void RebuildDictionary(int count)
        {
            TestDictionary.Clear();
            for (var i = 0; i < count; i++)
            {
                TestDictionary.Add(i, string.Format("{0} Test Entry", i));
            }
        }
    }
}
