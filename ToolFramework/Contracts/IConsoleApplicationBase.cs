namespace CarbonCore.ToolFramework.Contracts
{
    using System;

    public interface IConsoleApplicationBase : IDisposable
    {
        string Name { get; }

        Version Version { get; }

        void Start();
    }
}
