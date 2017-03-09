namespace CarbonCore.CrystalBuild.Java.Logic
{
    using CarbonCore.CrystalBuild.Contracts;

    using NLog;

    public static class BuildingUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void TraceProcessorResult(IContentProcessor processor, string name)
        {
            var context = processor.GetContext<IProcessingContext>();

            Logger.Info(string.Empty);
            Logger.Info("Result for {0}", name);
            Logger.Info(" -------------------");
            foreach (string warning in context.Warnings)
            {
                Logger.Warn(" - WARNING: {0}", warning);
            }

            foreach (string error in context.Errors)
            {
                Logger.Error(" - ERROR: {0}", error);
            }

            Logger.Info(string.Empty);
            Logger.Info("{0} Done with {1} Errors, {2} Warnings\n\n", name, context.Errors.Count, context.Warnings.Count);
        }
    }
}
