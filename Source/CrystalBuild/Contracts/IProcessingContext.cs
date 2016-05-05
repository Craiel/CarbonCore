namespace CarbonCore.CrystalBuild.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.Utils.IO;

    public interface IProcessingContext
    {
        string Name { get; set; }
        
        CarbonDirectory Root { get; set; }

        IReadOnlyCollection<string> Warnings { get; }

        IReadOnlyCollection<string> Errors { get; }

        void AddWarning(string message, params object[] args);

        void AddError(string message, params object[] args);
    }
}
