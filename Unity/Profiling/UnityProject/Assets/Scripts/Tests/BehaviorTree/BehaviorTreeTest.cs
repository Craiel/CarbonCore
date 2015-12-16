namespace Assets.Scripts.Tests.BehaviorTree
{
    using System.Collections.Generic;

    using Assets.Scripts.Game;

    using CarbonCore.Utils.Unity.Logic.BehaviorTree;

    public static class BehaviorTreeTest
    {
        private static readonly BehaviorTree Tree;

        private static readonly BehaviorTreeContext Context;

        static BehaviorTreeTest()
        {
            Tree = new BehaviorTree { Root = TestTrees.DefaultTestTree() };

            Context = new BehaviorTreeContext();
        }
        
        public static void Run()
        {
            PrepareContext(Context);

            Tree.Execute(Context);
        }

        private static void PrepareContext(BehaviorTreeContext context)
        {
            context.Clear();
            context.Set("Some Random String");
            context.Set(GameSystem.Instance);
            context.Set(1);
            context.SetVariable(1, "First");
            context.SetVariable("second", true);
            context.Set(new List<int>());
        }
    }
}
