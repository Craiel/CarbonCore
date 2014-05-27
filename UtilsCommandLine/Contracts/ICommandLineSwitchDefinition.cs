namespace CarbonCore.UtilsCommandLine.Contracts
{
    public delegate void CommandLineSwitchSetDelegate(string argument);

    public interface ICommandLineSwitchDefinition
    {
        CommandLineSwitchSetDelegate Set { get; }

        string Long { get; }
        string Short { get; }

        string Description { get; set; }

        bool Required { get; set; }
        bool AllowMultiple { get; set; }
        bool RequireArgument { get; set; }
    }
}
