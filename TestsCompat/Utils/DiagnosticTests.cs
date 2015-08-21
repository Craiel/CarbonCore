namespace CarbonCore.Tests.Compat.Utils
{
    using System;
    using System.Threading;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class DiagnosticTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void TestThreadedTracing()
        {
            var first = new EngineThread(this.FirstThreadMain, "First Thread", new EngineThreadSettings(2));
            first.Start();

            var second = new EngineThread(this.SecondThreadMain, "Second Thread", new EngineThreadSettings(10));
            second.Start();

            Diagnostic.Error("Test Error message");

            Thread.Sleep(TimeSpan.FromSeconds(2));

            first.Shutdown();
            second.Shutdown();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool FirstThreadMain(EngineTime time)
        {
            Diagnostic.Info("Info");
            return true;
        }

        private bool SecondThreadMain(EngineTime time)
        {
            Diagnostic.Warning("Warning!!");
            return true;
        }
    }
}
