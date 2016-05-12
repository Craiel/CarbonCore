namespace CarbonCore.ToolFramework.Console.Contracts
{
    using System;

    public interface IConsoleApplicationBase : IDisposable
    {
        string Name { get; }

        Version Version { get; }

        int ExitCode { get; }

        void Start();
    }
}
