namespace CarbonCore.Modules.D3Theory.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.Modules.D3Theory.Data;
    using CarbonCore.Utils.IO;

    public interface IMainData
    {
        D3Generic Generic { get; }

        IReadOnlyCollection<D3Class> Classes { get; }

        void Load(CarbonDirectory path);
        void Save(CarbonDirectory path);
    }
}
