namespace CarbonCore.Utils.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IJsonConfig<T>
    {
        T Current { get; set; }

        bool Load(CarbonFile file);
        bool Save(CarbonFile file = null);

        void Reset();
    }
}
