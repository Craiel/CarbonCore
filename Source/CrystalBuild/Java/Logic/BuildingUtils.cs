namespace CarbonCore.CrystalBuild.Java.Logic
{
    using CarbonCore.CrystalBuild.Contracts;
    using CarbonCore.Utils.Diagnostics;

    public static class BuildingUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void TraceProcessorResult(IContentProcessor processor, string name)
        {
            var context = processor.GetContext<IProcessingContext>();

            Diagnostic.Info(string.Empty);
            Diagnostic.Info("Result for {0}", name);
            Diagnostic.Info(" -------------------");
            foreach (string warning in context.Warnings)
            {
                Diagnostic.Warning(" - WARNING: {0}", warning);
            }

            foreach (string error in context.Errors)
            {
                Diagnostic.Error(" - ERROR: {0}", error);
            }

            Diagnostic.Info(string.Empty);
            Diagnostic.Info("{0} Done with {1} Errors, {2} Warnings\n\n", name, context.Errors.Count, context.Warnings.Count);
        }
    }
}
