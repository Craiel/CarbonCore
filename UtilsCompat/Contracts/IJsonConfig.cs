namespace CarbonCore.Utils.Compat.Contracts
{
    using CarbonCore.Utils.Compat.IO;

    public interface IJsonConfig<T>
    {
        T Current { get; set; }

        bool Load(CarbonFile file);
        bool Save(CarbonFile file = null);

        void Reset();
    }
}
