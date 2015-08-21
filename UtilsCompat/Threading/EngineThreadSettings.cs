namespace CarbonCore.Utils.Compat.Threading
{
    public class EngineThreadSettings
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EngineThreadSettings()
        {
        }

        public EngineThreadSettings(int frameRate, int frameRateMeasureInterval = 2)
        {
            this.TargetFrameRate = frameRate;
            this.FrameRateMeasureInterval = frameRateMeasureInterval;
            this.ThrottleFrameRate = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool ThrottleFrameRate { get; set; }

        public int TargetFrameRate { get; set; }

        public int FrameRateMeasureInterval { get; set; }
    }
}
