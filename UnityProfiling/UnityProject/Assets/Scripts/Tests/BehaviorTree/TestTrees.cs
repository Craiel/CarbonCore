namespace Assets.Scripts.Tests.BehaviorTree
{
    using CarbonCore.Utils.Unity.Logic.BehaviorTree;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public static class TestTrees
    {
        public static BehaviorTreeNode DefaultTestTree()
        {
            var node = new BehaviorTreeCompositeNode(BehaviorTreeCompositeMode.Sequence);
            node.AddChild(FirstSubNode());
            node.AddChild(SecondSubNode());
            node.AddChild(ThirdSubNode());

            return node;
        }

        public static BehaviorTreeNode FirstSubNode()
        {
            var node = new NodeCustomComposite();
            node.AddChild(new NodeSlow());
            node.AddChild(new NodeFast());

            return node;
        }

        public static BehaviorTreeNode SecondSubNode()
        {
            var node = new NodeCompositeMulti();
            node.AddChild(new NodeFail());
            node.AddChild(new NodeFast());

            return node;
        }

        public static BehaviorTreeNode ThirdSubNode()
        {
            var node = new BehaviorTreeCompositeNode(mode: BehaviorTreeCompositeMode.Sequence);
            node.AddChild(new BehaviorTreeDecoratorNode(new NodeFail(), BehaviorTreeDecoratorMode.Inverter));
            node.AddChild(new BehaviorTreeDecoratorNode(new NodeFail(), BehaviorTreeDecoratorMode.Succeeder));
            node.AddChild(new BehaviorTreeDecoratorNode(new NodeFast(), BehaviorTreeDecoratorMode.Inverter));
            node.AddChild(new BehaviorTreeDecoratorNode(new NodeFast(), BehaviorTreeDecoratorMode.Failer));
            return node;
        }
    }
}
