namespace CarbonCore.Tests.ToolFramework
{
    using Autofac;

    using CarbonCore.Tests.Contracts;
    using CarbonCore.Tests.IoC;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [TestFixture, RequiresSTA]
    public class FrameworkActionTests
    {
        private IContainer container;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = new CarbonContainerAutofacBuilder().Build<ToolFrameworkTestModule>() as IContainer;
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
