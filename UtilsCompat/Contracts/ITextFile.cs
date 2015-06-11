namespace CarbonCore.Utils.Compat.Contracts
{
    using System;

    using CarbonCore.Utils.Compat.IO;

    public interface ITextFile : IDisposable
    {
        CarbonFile File { get; set; }

        void Write(string value);
        void WriteLine(string line);
        void Clear();
        void Close();
    }
}
