namespace CarbonCore.CodeGeneration
{
    using System;

    using EnvDTE;

    public static class CodeGenerationUtils
    {
        public static Project CurrentProject;
        public static string ProjectPath;

        public static string TemplateFile;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Initialize(object host, string templateFile)
        {
            TemplateFile = templateFile;

            var provider = (IServiceProvider)host;
            var dteObject = (DTE)provider.GetService(typeof(DTE));

            CurrentProject = dteObject.Solution.FindProjectItem(templateFile).ContainingProject;

            ProjectPath = System.IO.Path.GetDirectoryName(CurrentProject.FullName) + System.IO.Path.DirectorySeparatorChar;
        }
    }
}
