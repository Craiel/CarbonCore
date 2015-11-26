namespace CarbonCore.Utils.Unity.Logic
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
