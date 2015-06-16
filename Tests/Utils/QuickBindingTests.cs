namespace CarbonCore.Tests.Utils
{
    using CarbonCore.Tests.IoC;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class QuickBindingTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void TestContainerBuild()
        {
            ICarbonContainer container = CarbonContainerBuilder.BuildQuick<ToolFrameworkTestModule>();
            Assert.NotNull(container);
        }
    }
}
