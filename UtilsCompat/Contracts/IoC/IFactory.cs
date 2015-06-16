namespace CarbonCore.Utils.Compat.Contracts.IoC
{
    using System;

    public interface IFactory
    {
        T Resolve<T>();

        object Resolve(Type type);
    }
}
