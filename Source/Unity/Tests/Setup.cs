namespace CarbonCore.Unity.Tests
{
    using NLog;

    using NUnit.Framework;

    [SetUpFixture]
    public class Setup
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [SetUp]
        public void SetUp()
        {
            // This initializes diagnostic and sets the main thread context
            Logger.Info("Tests starting!");
        }
    }
}
