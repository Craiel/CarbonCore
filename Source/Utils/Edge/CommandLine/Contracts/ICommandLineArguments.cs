namespace CarbonCore.Utils.Edge.CommandLine.Contracts
{
    using System.Collections.Generic;

    public interface ICommandLineArguments
    {
        IReadOnlyCollection<ICommandLineSwitch> ActiveSwitches { get; }

        bool ParseArguments(string arguments);
        bool ParseCommandLineArguments();
        
        bool HasSwitch(string key);

        ICommandLineSwitch GetSwitch(string key);

        ICommandLineSwitchDefinition Define(string shortString, CommandLineSwitchSetDelegate action);
        ICommandLineSwitchDefinition Define(string shortString, string longString, CommandLineSwitchSetDelegate action);

        void ClearDefines();

        void PrintArgumentUse();
    }
}
