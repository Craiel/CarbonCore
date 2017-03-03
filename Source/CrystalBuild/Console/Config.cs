namespace CarbonCore.CrystalBuild.Console
{
    using Contracts;

    using Data;

    using Utils;
    using Utils.IO;
    using Utils.Json;

    public class Config : JsonConfig<CSharpBuildConfig>, IConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool Load(CarbonFile file)
        {
            bool result = base.Load(file);

            // Set the project root
            this.Current.WorkingDirectory = file.GetDirectory();
            if (this.Current.WorkingDirectory == null || this.Current.WorkingDirectory.IsNull)
            {
                this.Current.WorkingDirectory = RuntimeInfo.WorkingDirectory;
            }

            return result;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override CSharpBuildConfig GetDefault()
        {
            return new CSharpBuildConfig();
        }
    }
}
