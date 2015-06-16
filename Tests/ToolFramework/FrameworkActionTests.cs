namespace CarbonCore.Tests.ToolFramework
{
    using CarbonCore.Tests.Contracts;
    using CarbonCore.Tests.IoC;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [TestFixture, RequiresSTA]
    public class FrameworkActionTests
    {
        private ICarbonContainer container;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = CarbonContainerAutofacBuilder.Build<ToolFrameworkTestModule>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GeneralWiringTest()
        {
            var main = this.container.Resolve<IFrameworkTestMain>();
            main.Start();
        }
    }
}
