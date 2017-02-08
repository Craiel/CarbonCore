namespace CarbonCore.Unity.Tests
{
    using CarbonCore.Utils.Diagnostics;

    using NUnit.Framework;

    [SetUpFixture]
    public class Setup
    {
        [SetUp]
        public void SetUp()
        {
            Diagnostic.RegisterThread("Test Main");
            
            // This initializes diagnostic and sets the main thread context
            Diagnostic.Info("Tests starting!");
        }
    }
}
