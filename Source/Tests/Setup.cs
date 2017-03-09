namespace CarbonCore.Tests
{
    using NLog;

    using NUnit.Framework;
    
    [SetUpFixture]
    public class Setup
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        [OneTimeSetUp]
        public void SetUp()
        {
            // This initializes diagnostic and sets the main thread context
            Logger.Info("Tests starting!");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }
    }
}
