namespace Assets.Scripts.Tests
{
    using Assets.Scripts.Enums;
    using Assets.Scripts.Systems;
    using Assets.Scripts.Tests.BehaviorTree;
    using Assets.Scripts.Tests.General;
    using Assets.Scripts.Tests.JsonTests;

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

            this.DictionaryEntryCount = 100;
            this.DictionaryLookupsPerInterval = 10;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public int TestSampleCount;

        [SerializeField]
        public float TestInterval;

        [SerializeField]
        public int DictionaryEntryCount;

        [SerializeField]
        public int DictionaryLookupsPerInterval;

        [SerializeField]
        public bool DictionaryRandomLookup;
        
        public bool EnableGeneralTests { get; set; }

        public bool EnableBehaviorTreeTests { get; set; }

        public bool EnableDictionaryTests { get; set; }

        public bool EnableJsonTests { get; set; }

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
                this.EnableGeneralTests = false;
            }

            if (this.EnableBehaviorTreeTests)
            {
                BehaviorTreeTest.Run();
                this.EnableBehaviorTreeTests = false;
            }

            if (this.EnableDictionaryTests)
            {
                DictionaryTest.Run(this.DictionaryEntryCount, this.DictionaryLookupsPerInterval, this.DictionaryRandomLookup);
                this.EnableDictionaryTests = false;
            }

            if (this.EnableJsonTests)
            {
                JsonTest.Run();
                this.EnableJsonTests = false;
            }
        }
    }
}
