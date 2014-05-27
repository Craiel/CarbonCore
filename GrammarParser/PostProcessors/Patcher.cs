namespace CarbonCore.GrammarParser.PostProcessors
{
    using CarbonCore.GrammarParser.Patching;
    using CarbonCore.GrammarParser.PostProcessing;

    public class Patcher : BasePostProcessor<PatcherData, PatcherInstruction, Line>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Patcher()
        {
            this.Verbose = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool Verbose { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool PrepareProcess(PatcherData data, PatcherInstruction instruction)
        {
            foreach (Patch customPatch in instruction.Patches)
            {
                if (customPatch.FileFilters != null)
                {
                    foreach (string fileFilter in customPatch.FileFilters)
                    {
                        if (instruction.FileName == fileFilter)
                        {
                            data.Patches.Add(customPatch);
                        }
                    }
                }
                else
                {
                    data.Patches.Add(customPatch);
                }
            }

            if (data.Patches.Count > 0)
            {
                if (this.Verbose)
                {
                    System.Diagnostics.Trace.TraceInformation(
                        " -- Patching {0} with {1} Patches", instruction.FileName, data.Patches.Count);
                }

                return true;
            }

            return false;
        }

        protected override bool ProcessLine(PatcherData data, Line line)
        {
            for (int i = 0; i < data.Patches.Count; i++)
            {
                Patch patch = data.Patches[i];

                string lineData = line.Processed;
                if (!patch.Apply(ref lineData))
                {
                    continue;
                }

                line.Processed = lineData;
                line.UpdateMode = LineUpdateMode.Update;

                // After this we only do checks, skip on non-verbose
                if (!this.Verbose)
                {
                    continue;
                }

                System.Diagnostics.Trace.TraceInformation(
                    "    | Applied Patch @line {0}, {1}", line.LineNumber, patch.Name ?? patch.Pattern);
            }

            return true;
        }

        protected override void FinalizeProcess(PatcherData data, PatcherInstruction instruction)
        {
            for (int i = 0; i < data.Patches.Count; i++)
            {
                Patch patch = data.Patches[i];
                if (patch.FileFilters != null && patch.ApplyCount <= 0)
                {
                    System.Diagnostics.Trace.TraceWarning(
                        "Custom patch did not apply for {0}: {1}", instruction.FileName, patch.Name ?? patch.Pattern);
                }
            }
        }
    }
}
