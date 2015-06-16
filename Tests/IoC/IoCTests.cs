namespace CarbonCore.Tests.IoC
{
    using CarbonCore.Tests.Contracts;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class QuickBindingTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void AutofacContainerBuild()
        {
            ICarbonContainer container = CarbonContainerAutofacBuilder.Build<ToolFrameworkTestModule>();
            this.TestBuiltContainer(container);
        }

        [Test]
        public void QuickContainerBuild()
        {
            ICarbonContainer container = CarbonContainerBuilder.BuildQuick<ToolFrameworkTestModule>();
            this.TestBuiltContainer(container);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void TestBuiltContainer(ICarbonContainer container)
        {
            Assert.NotNull(container);

            IFactory factory = container.Resolve<IFactory>();
            Assert.NotNull(factory);

            // Check a factory resolve with singleton binding
            IFrameworkTestMain mainOne = factory.Resolve<IFrameworkTestMain>();
            IFrameworkTestMain mainTwo = factory.Resolve<IFrameworkTestMain>();
            Assert.NotNull(mainOne);
            Assert.NotNull(mainTwo);
            Assert.AreEqual(mainOne, mainTwo);

            // Check a factory resolve without singleton
            IFrameworkTestMainViewModel vmOne = factory.Resolve<IFrameworkTestMainViewModel>();
            IFrameworkTestMainViewModel vmTwo = factory.Resolve<IFrameworkTestMainViewModel>();
            Assert.NotNull(vmOne);
            Assert.NotNull(vmTwo);
            Assert.AreNotEqual(vmOne, vmTwo);
        }
    }
}
