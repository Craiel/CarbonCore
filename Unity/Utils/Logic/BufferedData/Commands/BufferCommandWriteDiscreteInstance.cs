namespace CarbonCore.Utils.Unity.Logic.BufferedData.Commands
{
    using System;
    
    using CarbonCore.Utils.Unity.Contracts.BufferedData;

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
