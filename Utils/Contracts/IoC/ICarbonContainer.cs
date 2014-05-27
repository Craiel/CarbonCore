namespace CarbonCore.Utils.Contracts.IoC
{
    using Autofac;
    using Autofac.Core;

    public interface ICarbonContainer : IContainer
    {
        T Resolve<T>(params Parameter[] parameter);
    }
}
