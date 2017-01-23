namespace CarbonCore.Utils.Contracts
{
    public interface IFormatter
    {
        void Clear();

        string Get(string key);

        void Set(string key, string value);

        void Unset(string key);

        string Format(string template);
    }
}
