namespace CarbonCore.Applications.CrystalBuild.CSharp
{
    using CarbonCore.Applications.CrystalBuild.CSharp.Contracts;
    using CarbonCore.Applications.CrystalBuild.CSharp.Data;
    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Json;

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
