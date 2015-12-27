namespace CarbonCore.Applications.MCSync.Launcher
{
    using CarbonCore.Applications.MCSync.Launcher.Contracts;
    using CarbonCore.Applications.MCSync.Launcher.Logic;
    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Json;

    public class Config : JsonConfig<LaunchConfig>, IConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool Load(CarbonFile file)
        {
            bool result = base.Load(file);

            // Set the project root
            if (this.Current.Root == null || this.Current.Root.IsNull)
            {
                this.Current.Root = RuntimeInfo.WorkingDirectory;
            }

            return result;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override LaunchConfig GetDefault()
        {
            return new LaunchConfig
            {
                Root = new CarbonDirectory(@"."),
                SourcePath = @"C:\Users\USERNAME\Google Drive\Minecraft\source",
                TargetPath = ".",
                Force = true,
                Server = false
            };
        }
    }
}
