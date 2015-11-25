namespace CarbonCore.Tests.Unity.BufferedData
{
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.Unity.IoC;

    using NUnit.Framework;

    [SetUpFixture]
    public class BufferedDataTestSetup
    {
        private ICarbonContainer container;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IFactory Factory { get; private set; }

        [SetUp]
        public void SetUp()
        {
            this.container = CarbonContainerBuilder.BuildQuick<UtilsUnityModule>();

            Factory = this.container.Resolve<IFactory>();
        }
    }
}
