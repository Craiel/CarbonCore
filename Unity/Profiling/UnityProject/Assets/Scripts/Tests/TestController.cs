namespace Assets.Scripts.Tests
{
    using Assets.Scripts.Enums;
    using Assets.Scripts.Systems;
    using Assets.Scripts.Tests.BehaviorTree;
    using Assets.Scripts.Tests.General;

    using CarbonCore.Utils.Unity.Logic;

    using UnityEngine;

    public class TestController : UnitySingletonBehavior<TestController>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TestController()
        {
            this.TestSampleCount = 100;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public int TestSampleCount;

        public bool EnableGeneralTests { get; set; }

        public bool EnableBehaviorTreeTests { get; set; }

        public override void Awake()
        {
            this.RegisterInController(SceneController.Instance, SceneRootCategory.System, true);

            base.Awake();
        }

        public void Update()
        {
            for (var i = 0; i < this.TestSampleCount; i++)
            {
                this.DoRunTests();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoRunTests()
        {
            if (this.EnableGeneralTests)
            {
                GeneralTest.Run();
            }

            if (this.EnableBehaviorTreeTests)
            {
                BehaviorTreeTest.Run();
            }
        }
    }
}
