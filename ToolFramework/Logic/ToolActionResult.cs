namespace CarbonCore.ToolFramework.Logic
{
    using System;

    using CarbonCore.ToolFramework.Contracts;

    public class ToolActionResult : IToolActionResult
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public DateTime StartTime { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
