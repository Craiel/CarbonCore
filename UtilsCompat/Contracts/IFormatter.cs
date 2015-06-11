namespace CarbonCore.Utils.Compat.Contracts
{
    public interface IFormatter
    {
        void Clear();

        string Get(string key);
        void Set(string key, string value);

        string Format(string template);
    }
}
