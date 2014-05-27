namespace CarbonCore.UtilsCommandLine
{
    using System.Collections.Generic;

    using CarbonCore.UtilsCommandLine.Contracts;

    public class CommandLineSwitch : ICommandLineSwitch
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Switch { get; set; }

        public IList<string> Arguments { get; set; }
        
        public override int GetHashCode()
        {
            return this.Switch.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var typed = obj as CommandLineSwitch;
            if (typed == null)
            {
                return false;
            }

            return this.Switch.Equals(typed.Switch);
        }
    }
}
