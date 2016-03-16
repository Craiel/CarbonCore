namespace CarbonCore.Utils
{
    using System;

    public static class ProcessorUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static int GetAvailableProcessorCount(ProcessorSettings settings, int count)
        {
            // Determine how many processors we can use
            switch (settings)
            {
                case ProcessorSettings.Single:
                    {
                        return 1;
                    }

                case ProcessorSettings.CountOnly:
                    {
                        return count;
                    }

                case ProcessorSettings.CountOrLess:
                    {
                        return Math.Min(count, Environment.ProcessorCount);
                    }

                case ProcessorSettings.CountOrMore:
                    {
                        return Math.Max(count, Environment.ProcessorCount);
                    }

                case ProcessorSettings.MaxExceptCount:
                    {
                        return Math.Max(1, Environment.ProcessorCount - count);
                    }

                default:
                    {
                        throw new InvalidOperationException("Unknown Processor Setting: " + settings);
                    }
            }
        }
    }
}
