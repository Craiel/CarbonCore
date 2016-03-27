namespace CarbonCore.Unity.Utils.Data
{
    using CarbonCore.Utils.Threading;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class EngineTimeSnapshot
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EngineTimeSnapshot()
        {
        }

        public EngineTimeSnapshot(EngineTimeSnapshot other)
        {
            this.Frame = other.Frame;
            this.Speed = other.Speed;
            this.Ticks = other.Ticks;
            this.FixedTicks = other.FixedTicks;
            this.TicksLostToPause = other.TicksLostToPause;
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
        public long Frame { get; set; }
        
        public float Speed { get; set; }
        
        public long Ticks { get; set; }
        
        public long FixedTicks { get; set; }
        
        public long TicksLostToPause { get; set; }

        public EngineTime GetTime()
        {
            return new EngineTime(this.Frame, this.Speed, this.Ticks, this.FixedTicks, this.TicksLostToPause);
        }
    }
}
