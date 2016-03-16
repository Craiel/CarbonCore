namespace CarbonCore.Utils
{
    public enum ProcessorSettings
    {
        Single,         // Use only 1 core
        CountOrMore,    // Use at least n cores but take more
        CountOrLess,    // Use at most n cores
        CountOnly,      // Use exactly n cores (or throw if fail)
        MaxExceptCount  // Use as many as possible - n
    }
}
