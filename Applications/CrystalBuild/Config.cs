namespace CarbonCore.Applications.CrystalBuild
{
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.Json;

    using CrystalBuild.Contracts;

    public class Config : JsonConfig<BuildConfig>, IConfig
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool Load(CarbonFile file)
        {
            bool result = base.Load(file);

            // Set the project root
            this.Current.ProjectRoot = file.GetDirectory();
            if (this.Current.ProjectRoot == null || this.Current.ProjectRoot.IsNull)
            {
                this.Current.ProjectRoot = RuntimeInfo.WorkingDirectory;
            }

            return result;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override BuildConfig GetDefault()
        {
            return new BuildConfig
                       {
                           Name = Constants.DefaultProjectName,
                           Templates = new[] { new CarbonDirectoryFilter(Constants.DataTemplateDirectory, Constants.FilterTemplates) },
                           Sources = new[] { new CarbonDirectoryFilter(Constants.SourceDirectory, Constants.FilterSource) },
                           Data = new[] { new CarbonDirectoryFilter(Constants.DataDirectory, Constants.FilterData) },
                           StyleSheets = new[] {new CarbonDirectoryFilter(Constants.DataCssDirectory, Constants.FilterStyleSheet) },
                           Contents = new[] { new CarbonDirectoryFilter(Constants.ContentDirectory, Constants.FilterContent) },
                           Images = new[] { new CarbonDirectoryFilter(Constants.DataImagesDirectory, Constants.FilterImages) },
                           SourceTarget = Constants.OutputDirectory.ToFile(Constants.DefaultProjectTarget),
                           TemplateTarget = Constants.SourceDataGeneratedDirectory.ToFile(Constants.DefaultTemplateTarget),
                           DataTarget = Constants.SourceDataGeneratedDirectory.ToFile(Constants.DefaultDataTarget),
                           StyleSheetTarget = Constants.OutputDirectory.ToFile(Constants.DefaultStyleSheetTarget),
                           ImageRoot = Constants.ContentDirectory.ToDirectory(Constants.DefaultImageRoot),
                           ContentTarget = Constants.OutputDirectory
                       };
        }
    }
}
