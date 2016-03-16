namespace CarbonCore.Unity.Utils.Logic
{
    public delegate void IntervalTriggerDelegate(float currentTime, IntervalTrigger trigger);

    public class IntervalTrigger
    {
        private readonly float interval;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public IntervalTrigger(float interval)
        {
            this.interval = interval;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event IntervalTriggerDelegate OnTrigger;

        public float LastTrigger { get; private set; }

        public long TriggerCount { get; private set; }

        public static IntervalTrigger Create(float interval, IntervalTriggerDelegate target)
        {
            var trigger = new IntervalTrigger(interval);
            trigger.OnTrigger += target;
            return trigger;
        }

        public void Update(float currentTime)
        {
            if (currentTime > this.LastTrigger + this.interval)
            {
                this.TriggerCount++;
                if (this.OnTrigger != null)
                {
                    this.OnTrigger(currentTime, this);
                }

                this.LastTrigger = currentTime;
            }
        }
    }
}
