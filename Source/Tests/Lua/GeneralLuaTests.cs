namespace CarbonCore.Tests.Lua
{
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.Lua.Contracts;
    using CarbonCore.Utils.Lua.IoC;

    using NUnit.Framework;

    public class GeneralLuaTests
    {
        private ICarbonContainer container;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = CarbonContainerBuilder.BuildQuick<UtilsLuaModule>();
        }

        [TearDown]
        public void TearDown()
        {
            this.container.Dispose();
        }

        [Test]
        public void LuaInitializeTest()
        {
            var runtime = this.container.Resolve<ILuaRuntime>();

            Assert.IsNotNull(runtime);

            runtime.Execute(@"print ""Hello World""");
        }
    }
}
