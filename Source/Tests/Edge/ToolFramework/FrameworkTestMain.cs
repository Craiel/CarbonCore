namespace CarbonCore.Tests.Edge.ToolFramework
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;

    using CarbonCore.Tests.Edge.Contracts;
    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Logic.Actions;
    using CarbonCore.ToolFramework.Windows.Logic;
    using CarbonCore.Utils.Contracts.IoC;

    using NUnit.Framework;

    public class FrameworkTestMain : WindowApplicationBase, IFrameworkTestMain
    {
        private const int TestCycles = 100;

        private const string ToolName = "FrameworkTest";

        private readonly Random random = new Random((int)DateTime.Now.Ticks);

        private readonly IList<int> cyclesTested;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FrameworkTestMain(IFactory factory)
            : base(factory)
        {
            this.cyclesTested = new List<int>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Name
        {
            get
            {
                return ToolName;
            }
        }
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void StartupInitializeCustomActions(IList<IToolAction> target)
        {
            // Create 2 non-dispatched actions
            IToolAction action = RelayToolAction.Create(this.TestAction);
            target.Add(action);

            for (int i = 0; i < 4; i++)
            {
                action = RelayToolAction.Create(this.TestAction);
                target.Add(action);
            }

            for (int i = 0; i < 5; i++)
            {
                action = RelayToolAction.Create(this.TestAction);
                action.CanCancel = true;
                action.Order = 5;
                target.Add(action);
            }

            // Create one dispatched action that will run after the others
            action = RelayToolAction.Create(this.TestAction2);
            Assert.Fail("Dispatcher is currently not implemented!");
            /*action.Dispatcher = Application.Current.Dispatcher;
            action.Order = 10;
            target.Add(action);*/
        }

        protected override Window DoInitializeMainWindow()
        {
            return new FrameworkTestMainWindow();
        }
        
        protected override void StartupInitializeLogic(IToolAction toolAction, CancellationToken cancellationToken)
        {
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void TestAction(IToolAction action, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId + " TestAction");
            action.ProgressMax = TestCycles;
            for (int i = 0; i < TestCycles; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    System.Diagnostics.Debug.WriteLine("Cancelling action1!");
                    return;
                }

                action.Progress = i;
                action.ProgressMessage = string.Format("Test {0} of {1}", i + 1, TestCycles);
                Thread.Sleep(this.random.Next(5, 25));
            }

            lock (this.cyclesTested)
            {
                this.cyclesTested.Add(TestCycles);
            }

            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId + " TestAction - Done");
        }

        private void TestAction2(IToolAction action, CancellationToken cancellationToken)
        {
            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId + " TestAction2");
            Assert.Fail("Dispatcher is currently not implemented!");

            /*Assert.AreEqual(action.Dispatcher, Application.Current.Dispatcher, "Dispatched test must be in the main dispatcher");
            Assert.AreEqual(10, this.cyclesTested.Count);
            
            action.ProgressMax = TestCycles;
            for (int i = 0; i < TestCycles; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    System.Diagnostics.Debug.WriteLine("Cancelling action2!");
                    return;
                }

                action.Progress = i;
                action.ProgressMessage = string.Format("Dispatched Test {0} of {1}", i + 1, TestCycles);
                Thread.Sleep(this.random.Next(5, 15));
            }

            lock (this.cyclesTested)
            {
                this.cyclesTested.Add(TestCycles);
            }

            System.Diagnostics.Debug.WriteLine(Thread.CurrentThread.ManagedThreadId + " TestAction2 - Done");*/
        }
    }
}
