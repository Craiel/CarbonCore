namespace CarbonCore.ToolFramework.Contracts.ViewModels
{
    using System;
    using System.Diagnostics;

    using CarbonCore.Utils.Diagnostics;

    public interface ILogEntryViewModel : IBaseViewModel
    {
        DateTime Time { get; }

        TraceEventType Type { get; }

        string Source { get; }

        string Message { get; }

        void SetData(TraceEventData data);
    }
}
