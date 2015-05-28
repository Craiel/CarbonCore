namespace CarbonCore.ToolFramework.Contracts
{
    using System;

    public interface IWindowApplicationBase : IDisposable
    {
        string Name { get; }
        
        Version Version { get; }

        void Start();
    }
}
