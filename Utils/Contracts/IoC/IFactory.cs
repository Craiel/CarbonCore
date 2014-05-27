namespace CarbonCore.Utils.Contracts.IoC
{
    public interface IFactory
    {
        T Resolve<T>();
    }
}
