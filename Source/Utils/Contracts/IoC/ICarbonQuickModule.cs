namespace CarbonCore.Utils.Contracts.IoC
{
    using System.Collections.Generic;

    public interface ICarbonQuickModule
    {
        ICarbonQuickBinding For<T>();

        IList<ICarbonQuickBinding> GetQuickBindings();
    }
}
