namespace CarbonCore.Tests.Edge.ToolFramework
{
    using System.Threading;
    
    using CarbonCore.Tests.Edge.IoC;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Edge.IoC;

    using NUnit.Framework;

    [TestFixture]
    [Apartment(ApartmentState.STA)]
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
    }
}
