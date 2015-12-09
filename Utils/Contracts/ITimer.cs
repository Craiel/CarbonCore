namespace CarbonCore.Utils.Contracts
{
    public interface ITimer
    {
        float TimeModifier { get; set; }

        long ElapsedTime { get; }
        long ActualElapsedTime { get; }
        long TimeLostToPause { get; }

        bool IsPaused { get; }

        void Reset();
        void Pause();
        void Resume();
        void Update();
    }
}
