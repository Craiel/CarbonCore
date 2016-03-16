namespace CarbonCore.Utils.Edge.CommandLine
{
    using CarbonCore.Utils.Edge.CommandLine.Contracts;

    public class CommandLineSwitchDefinition : ICommandLineSwitchDefinition
    {
        public CommandLineSwitchDefinition(string shortSwitch, string longSwitch, CommandLineSwitchSetDelegate action)
        {
            this.Short = shortSwitch;
            this.Long = longSwitch;
            this.Set = action;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CommandLineSwitchSetDelegate Set { get; private set; }

        public string Long { get; private set; }

        public string Short { get; private set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool AllowMultiple { get; set; }

        public bool RequireArgument { get; set; }
    }
}
