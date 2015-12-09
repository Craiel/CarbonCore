namespace CarbonCore.Tests.Unity.Data
{
    public static class DataTestData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static DataTestEntry TestEntry = new DataTestEntry
        {
            TestString = "test me",
            TestBool = true,
            TestFloat = 2.1f,
            TestLong = 999888777666555,
            TestByteArray =
                new byte[] { 20, 50, 20, 50, 10 }
        };

        public static DataTestEntry2 TestEntry2 = new DataTestEntry2
        {
            OtherTestString = "another test",
            OtherTestBool = true,
            OtherTestFloat = 5.1f,
            OtherTestLong = 523,
        };
    }
}
