namespace CarbonCore.ToolFramework.Windows.Contracts
{
    using System;

    public interface IWindowApplicationBase : IDisposable
    {
        string Name { get; }
        
        Version Version { get; }

        void Start();
    }
}
