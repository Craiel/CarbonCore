namespace CarbonCore.Unity.Utils.Logic.BufferedData.Commands
{
    using System;

    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferCommandWriteDiscreteInstance<T> : IBufferedDataCommand
        where T : IBufferedDataEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Execute(IBufferedDataset target)
        {
            target.AddInstance(Activator.CreateInstance<T>());
        }
    }
}
