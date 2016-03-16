namespace CarbonCore.Unity.Utils.Data
{
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.Utils.Threading;

    public class EngineTimeSnapshot : SmartDataEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EngineTimeSnapshot()
        {
        }

        public EngineTimeSnapshot(EngineTime time)
        {
            this.Frame = time.Frame;

            this.Speed = time.Speed;

            this.Ticks = time.Ticks;

            this.FixedTicks = time.FixedTicks;

            this.TicksLostToPause = time.TicksLostToPause;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DataElement]
        public long Frame { get; set; }

        [DataElement]
        public float Speed { get; set; }

        [DataElement]
        public long Ticks { get; set; }
        
        [DataElement]
        public long FixedTicks { get; set; }
        
        [DataElement]
        public long TicksLostToPause { get; set; }

        public EngineTime GetTime()
        {
            return new EngineTime(this.Frame, this.Speed, this.Ticks, this.FixedTicks, this.TicksLostToPause);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return this.Frame.GetHashCode()
                ^ this.Speed.GetHashCode()
                ^ this.FixedTicks.GetHashCode()
                ^ this.TicksLostToPause.GetHashCode();
        }
    }
}
