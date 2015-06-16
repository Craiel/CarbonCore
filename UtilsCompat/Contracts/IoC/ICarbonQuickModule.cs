namespace CarbonCore.Utils.Compat.Contracts.IoC
{
    using System.Collections.Generic;

    public interface ICarbonQuickModule
    {
        ICarbonQuickBinding For<T>();

        IList<ICarbonQuickBinding> GetQuickBindings();
    }
}
