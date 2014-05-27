namespace CarbonCore.ToolFramework.Contracts
{
    using System;

    public interface IToolBase : IDisposable
    {
        string Title { get; }

        string Name { get; }

        Version Version { get; }

        void Start();
    }
}
