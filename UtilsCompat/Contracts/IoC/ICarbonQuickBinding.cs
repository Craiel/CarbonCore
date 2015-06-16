namespace CarbonCore.Utils.Compat.Contracts.IoC
{
    using System;

    public interface ICarbonQuickBinding
    {
        Type Interface { get; }
        Type Implementation { get; }
        bool IsSingleton { get; }

        ICarbonQuickBinding For<T>();
        ICarbonQuickBinding Use<T>();

        ICarbonQuickBinding Singleton();
    }
}
