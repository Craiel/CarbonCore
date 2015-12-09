namespace D3Theory.Contracts
{
    using System.Collections.Generic;

    using CarbonCore.Utils.IO;

    using D3Theory.Data;

    public interface IMainData
    {
        D3Generic Generic { get; }

        IReadOnlyCollection<D3Class> Classes { get; }

        void Load(CarbonDirectory path);
        void Save(CarbonDirectory path);
    }
}
