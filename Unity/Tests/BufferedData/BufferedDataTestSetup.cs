namespace CarbonCore.Unity.Tests.BufferedData
{
    using CarbonCore.Unity.Utils.IoC;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [SetUpFixture]
    public class BufferedDataTestSetup
    {
        private ICarbonContainer container;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IFactory Factory { get; private set; }

        [OneTimeSetUp]
        public void SetUp()
        {
            this.container = CarbonContainerBuilder.BuildQuick<UtilsUnityModule>();

            Factory = this.container.Resolve<IFactory>();
        }
    }
}
