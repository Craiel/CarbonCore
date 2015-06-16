namespace CarbonCore.Utils.Compat.Contracts.IoC
{
    using System;
    using System.Collections.Generic;

    public interface ICarbonContainer : IDisposable
    {
        T Resolve<T>(IDictionary<string, object> customParameters = null);

        object Resolve(Type type, IDictionary<string, object> customParameters = null);
    }
}
