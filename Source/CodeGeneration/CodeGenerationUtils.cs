namespace CarbonCore.CodeGeneration
{
    using System;

    using EnvDTE;

    public static class CodeGenerationUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Project CurrentProject { get; private set; }

        public static string ProjectPath { get; private set; }

        public static string TemplateFile { get; private set; }

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
