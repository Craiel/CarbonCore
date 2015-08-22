namespace CarbonCore.Utils.Compat.Contracts
{
    using CarbonCore.Utils.Compat.Diagnostics.Metrics;

    public interface ICarbonDiagnostics
    {
        MetricSection BeginMeasure();

        void TakeMeasure(MetricSection metric);

        void ResetMeasure(MetricSection metric);

        void RegisterLogContext(int id, string name);

        void UnregisterLogContext(int id);

        ILog GetLogContext(int id);

        void SetMute(int managedThreadId, bool mute);
    }
}
