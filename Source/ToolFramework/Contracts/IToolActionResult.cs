namespace CarbonCore.ToolFramework.Contracts
{
    using System;
    
    public interface IToolActionResult
    {
        DateTime StartTime { get; set; }
        TimeSpan ExecutionTime { get; set; }

        bool Success { get; set; }

        string Message { get; set; }

        Exception Exception { get; set; }
    }
}
