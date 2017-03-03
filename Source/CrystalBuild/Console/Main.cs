namespace CarbonCore.CrystalBuild.Console
{
    using System.Collections.Generic;
    using System.IO;

    using Applications.CrystalBuild.Contracts;
    using Applications.CrystalBuild.CSharp;
    using Applications.CrystalBuild.CSharp.Contracts;

    using Contracts;

    using Scripting;

    using ToolFramework.Console.Logic;

    using Utils.Contracts.IoC;
    using Utils.Edge.CommandLine.Contracts;
    using Utils.IO;

    public class Main : ConsoleApplicationBase, IMain
    {
        private readonly IConfig config;
        private readonly IBuildLogic logic;
        
        private CarbonDirectory projectRoot;
        private CarbonFile scriptFileName;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Main(IFactory factory)
            : base(factory)
        {
            this.config = factory.Resolve<IConfig>();
            this.logic = factory.Resolve<IBuildLogic>();
            
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Name => "CrystalBuild";

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void StartFinished()
        {
            if (this.projectRoot == null)
            {
                this.Arguments.PrintArgumentUse();
                return;
            }

            IList<CarbonFile> sources = new List<CarbonFile>();
            if (this.scriptFileName != null && this.scriptFileName.Exists)
            {
                sources.Add(this.scriptFileName);
            }
            else
            {
                CarbonFileResult[] matches = this.projectRoot.GetFiles("*" + Constants.ProjectExtension, SearchOption.AllDirectories);
                foreach (CarbonFileResult match in matches)
                {
                    sources.Add(match.Relative);
                }
            }

            this.config.Load(new CarbonFile(Constants.BuildConfigFileName));
            
            this.logic.BuildProjectFile(sources, this.projectRoot);
        }

        protected override bool RegisterCommandLineArguments()
        {
            ICommandLineSwitchDefinition definition = this.Arguments.Define("s", "script", x => this.scriptFileName = new CarbonFile(x));
            definition.RequireArgument = false;
            definition.Description = "The script to process";

            definition = this.Arguments.Define("p", "projectroot", x => this.projectRoot = new CarbonDirectory(x));
            definition.RequireArgument = true;
            definition.Description = "The root of the project";

            return true;
        }
    }
}
