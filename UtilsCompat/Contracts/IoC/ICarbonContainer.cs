namespace CarbonCore.Utils.Compat.Contracts.IoC
{
    using System;

    public interface ICarbonContainer
    {
        T Resolve<T>();

        object Resolve(Type type);
    }
}
