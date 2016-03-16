namespace CarbonCore.Tests.ContentServices.Data
{
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.Utils;

    public static class DataTestData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static DataTestEntry TestEntry = new DataTestEntry
        {
            TestString = "test me",
            TestBool = true,
            TestInt = 12345,
            TestFloat = 2.1f,
            TestLong = 999888777666555,
            ByteArray = new byte[] { 20, 50, 20, 50, 10 }
        };

        public static DataTestEntry2 TestEntry2 = new DataTestEntry2
        {
            OtherTestString = "another test",
            OtherTestBool = true,
            OtherTestFloat = 5.1f,
            OtherTestLong = 523
        };

        public static DataTestEntry FullTestEntry = new DataTestEntry
        {
            TestString = "This is a full cascaded complex test class!",
            TestBool = true,
            TestInt = 54321,
            TestFloat = 99.987654321f,
            TestLong = 12999888777666555,
            Enum = TestEnum.Second,
            ByteArray = new byte[] { 50, 50, 100, 100, 10, 2 },
            CascadedEntry = new DataTestEntry2 { Id = "Cascaded!", OtherTestBool = true, OtherTestFloat = 123.456f },
            SimpleCollection = new List<int> { 50, 50, 100, 100, 10, 2 },
            SimpleDictionary = new Dictionary<string, float>
                                    {
                                        { "First", 20.0f },
                                        { "Second", 19.0f },
                                        { "Third", 1.0f }
                                    },
            CascadingCollection = new List<DataTestEntry2>
                                    {
                                        new DataTestEntry2 { Id = "Test Entry 2" },
                                        new DataTestEntry2 { Id = "Another type of entry", OtherTestFloat = 99.0f }
                                    },
            CascadingDictionary = new Dictionary<int, DataTestEntry2>
                                    {
                                        { 0, new DataTestEntry2 { Id = "0" } },
                                        { 1, new DataTestEntry2 { Id = "1", OtherTestLong = 99 } },
                                        { 50, new DataTestEntry2 { Id = "Third", OtherTestString = "Still the third..." } }
                                    }
        };

        public static SyncTestEntry SyncTestEntry;

        public static SyncTestEntry2 SyncTestEntry2 = new SyncTestEntry2
                                                          {
                                                              Id = {Value = "Test Entry 2"},
                                                              OtherTestFloat = new Sync<float>(55.545f),
                                                              OtherTestLong = new Sync<long>(-99999999987654321),
                                                              OtherTestString = {Value = "Custom Strings are kewl!"}
                                                          };

        static DataTestData()
        {
            SyncTestEntry = new SyncTestEntry
        {
            TestString = { Value = "This is a full cascaded complex test class!" },
            TestBool = new Sync<bool>(true),
            TestInt = new Sync<int>(54321),
            TestFloat = new Sync<float>(99.987654321f),
            TestLong = new Sync<long>(12999888777666555),
            Enum = new Sync<TestEnum>(TestEnum.Third),
            ByteArray = { Value = new byte[] { 50, 50, 100, 100, 10, 2 } },
            CascadedEntry = { Value = new SyncTestEntry2 { Id = { Value = "Cascaded!" }, OtherTestBool = new Sync<bool>(true), OtherTestFloat = new Sync<float>(123.456f) } },
            };

            SyncTestEntry.SimpleCollection.AddRange(new List<int> { 50, 50, 100, 100, 10, 2 });
            SyncTestEntry.SimpleDictionary.Add("First", 20);
            SyncTestEntry.SimpleDictionary.Add("Second", 19);
            SyncTestEntry.SimpleDictionary.Add("Third", 1);
            SyncTestEntry.CascadingCollection.AddRange(new List<SyncTestEntry2>
                                    {
                                        new SyncTestEntry2 { Id = {Value = "Test Entry 2" }},
                                        new SyncTestEntry2 { Id = {Value = "Another type of entry"}, OtherTestFloat = new Sync<float>(99.0f) }
                                    });
            SyncTestEntry.CascadingDictionary.Add(0, new SyncTestEntry2 { Id = {Value = "0"}});
            SyncTestEntry.CascadingDictionary.Add(1, new SyncTestEntry2 { Id = {Value = "1"}, OtherTestLong = { Value = 99 } });
            SyncTestEntry.CascadingDictionary.Add(50, new SyncTestEntry2 { Id = { Value = "Third"}, OtherTestString = {Value = "Still the third..."} });
        }
    }
}
