namespace CarbonCore.Processing.Contracts
{
    using System;
    using System.IO;

    public interface ICarbonResource : IDisposable
    {
        void Load(Stream source);
        long Save(Stream target);
    }
}
