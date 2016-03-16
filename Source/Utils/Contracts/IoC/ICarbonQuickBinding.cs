namespace CarbonCore.Utils.Contracts.IoC
{
    using System;

    public interface ICarbonQuickBinding
    {
        Type Interface { get; }
        Type Implementation { get; }
        object Instance { get; }

        bool IsSingleton { get; }
        bool IsAlwaysUnique { get; }

        ICarbonQuickBinding For<T>();
        ICarbonQuickBinding Use<T>();
        ICarbonQuickBinding Use<T>(T instance);

        ICarbonQuickBinding Singleton();
    }
}
