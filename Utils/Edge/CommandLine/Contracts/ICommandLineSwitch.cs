namespace CarbonCore.Utils.Edge.CommandLine.Contracts
{
    using System.Collections.Generic;

    public interface ICommandLineSwitch
    {
        string Switch { get; set; }

        IList<string> Arguments { get; set; }
    }
}
