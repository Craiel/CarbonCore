namespace CarbonCore.UtilsCommandLine.Contracts
{
    using System.Collections.Generic;

    public interface ICommandLineSwitch
    {
        string Switch { get; set; }

        IList<string> Arguments { get; set; }
    }
}
