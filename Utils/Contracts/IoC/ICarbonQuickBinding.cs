namespace CarbonCore.Utils.Contracts.IoC
{
    public interface ICarbonQuickBinding
    {
        ICarbonQuickBinding Use<T>();

        ICarbonQuickBinding Singleton();
    }
}
