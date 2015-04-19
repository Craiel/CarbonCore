namespace CarbonCore.Utils.Contracts.IoC
{
    using System;

    public interface IFactory
    {
        T Resolve<T>();

        object Resolve(Type type);
    }
}
