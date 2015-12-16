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
        private float lastInterval;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TestController()
        {
            this.TestSampleCount = 10;
            this.TestInterval = 1.0f;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public int TestSampleCount;

        [SerializeField]
        public float TestInterval;

        public bool EnableGeneralTests { get; set; }

        public bool EnableBehaviorTreeTests { get; set; }

        public override void Awake()
        {
            this.RegisterInController(SceneController.Instance, SceneRootCategory.System, true);

            base.Awake();
        }

        public void Update()
        {
            if (Time.time < this.lastInterval + this.TestInterval)
            {
                return;
            }

            this.lastInterval = Time.time;
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
