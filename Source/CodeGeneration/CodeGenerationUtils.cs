namespace CarbonCore.CodeGeneration
{
    using System;

#if !__MonoCS__
    using EnvDTE;
#endif

    public static class CodeGenerationUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
#if !__MonoCS__
        public static Project CurrentProject { get; private set; }
#endif

        public static string ProjectPath { get; private set; }

        public static string TemplateFile { get; private set; }

        public static void Initialize(object host, string templateFile)
        {
#if __MonoCS__
            throw new InvalidOperationException("Code Generation is not supported in Mono!");
#else
            TemplateFile = templateFile;

            var provider = (IServiceProvider)host;
            var dteObject = (DTE)provider.GetService(typeof(DTE));

            CurrentProject = dteObject.Solution.FindProjectItem(templateFile).ContainingProject;

            ProjectPath = System.IO.Path.GetDirectoryName(CurrentProject.FullName) + System.IO.Path.DirectorySeparatorChar;
#endif
        }
    }
}
