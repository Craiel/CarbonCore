namespace CarbonCore.Tests.ContentServices
{
    public static class ContentTestData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ContentTestEntry TestEntry = new ContentTestEntry
        {
            TestString = "test me",
            TestBool = true,
            TestFloat = 2.1f,
            TestLong = 999888777666555,
            TestByteArray =
                new byte[] { 20, 50, 20, 50, 10 }
        };
    }
}
