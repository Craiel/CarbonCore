namespace CarbonCore.Applications.CrystalBuild
{
    using CarbonCore.Utils.Compat.IO;

    public static class Constants
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public const int ObfuscationValue = 191;

        public const string DefaultProjectName = "Crystal";

        public const string DefaultProjectTarget = DefaultProjectName + Utils.Compat.Constants.ExtensionJavaScript;
        public const string DefaultTemplateTarget = "templates" + Utils.Compat.Constants.ExtensionJavaScript;
        public const string DefaultDataTarget = "data" + Utils.Compat.Constants.ExtensionJavaScript;
        public const string DefaultStyleSheetTarget = DefaultProjectTarget + Utils.Compat.Constants.ExtensionCss;
        public const string DefaultImageTemplate = "imagesTemplate" + Utils.Compat.Constants.ExtensionJavaScript;
        public const string DefaultImageTarget = "images" + Utils.Compat.Constants.ExtensionJavaScript;
        public const string DefaultImageRoot = @"data\www\images";

        public const string FilterSource = "*" + Utils.Compat.Constants.ExtensionJavaScript;
        public const string FilterTemplates = "*" + Utils.Compat.Constants.ExtensionHtml;
        public const string FilterData = "*" + Utils.Compat.Constants.ExtensionCsv;
        public const string FilterStyleSheet = "*" + Utils.Compat.Constants.ExtensionCss;
        public const string FilterContent = "*";
        public const string FilterImages = "*";
        
        public static readonly CarbonDirectory DataDirectory = new CarbonDirectory("data");
        public static readonly CarbonDirectory DataCssDirectory = DataDirectory.ToDirectory("css");
        public static readonly CarbonDirectory DataTemplateDirectory = DataDirectory.ToDirectory("templates");
        public static readonly CarbonDirectory SourceDirectory = new CarbonDirectory("src");
        public static readonly CarbonDirectory SourceDataDirectory = SourceDirectory.ToDirectory("data");
        public static readonly CarbonDirectory SourceDataGeneratedDirectory = SourceDataDirectory.ToDirectory("generated");
        public static readonly CarbonDirectory OutputDirectory = new CarbonDirectory("bin");
        public static readonly CarbonDirectory ContentDirectory = new CarbonDirectory("www");
        public static readonly CarbonDirectory DataImagesDirectory = ContentDirectory.ToDirectory("images");
    }
}
