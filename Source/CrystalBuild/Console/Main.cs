namespace CarbonCore.CrystalBuild.Console
{
    using System.Collections.Generic;
    using System.IO;

    using Applications.CrystalBuild.Contracts;
    using Applications.CrystalBuild.CSharp;

    using Contracts;

    using Logic;

    using ToolFramework.Console.Logic;

    using Utils.Contracts.IoC;
    using Utils.Edge.CommandLine.Contracts;
    using Utils.IO;

    public class Main : ConsoleApplicationBase, IMain
    {
        private readonly IConfig config;
        
        private CarbonDirectory projectRoot;
        private CarbonFile scriptFileName;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Main(IFactory factory)
            : base(factory)
        {
            this.config = factory.Resolve<IConfig>();
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

            var context = new BuildContext { Root = this.projectRoot };
            if (this.scriptFileName != null && this.scriptFileName.Exists)
            {
                context.ConfigFiles.Add(this.scriptFileName);
            }
            else
            {
                CarbonFileResult[] matches = this.projectRoot.GetFiles("*" + Constants.BuildConfigExtension, SearchOption.AllDirectories);
                foreach (CarbonFileResult match in matches)
                {
                    context.ConfigFiles.Add(match.Relative);
                }
            }

            this.config.Load(new CarbonFile(Constants.BuildConfigFileName));
            
            BuildLogic.Build(context);
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
