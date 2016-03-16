namespace CarbonCore.Unity.Utils.Logic.BufferedData.Commands
{
    using System;
    
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferCommandWriteDiscreteInstance<T> : IBufferedDataCommand
        where T : IDataEntry
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
