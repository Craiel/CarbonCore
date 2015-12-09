namespace CarbonCore.Tests.Edge.ToolFramework
{
    using CarbonCore.Tests.Edge.Contracts;
    using CarbonCore.Tests.Edge.IoC;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Edge.IoC;

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
