namespace CarbonCore.ContentServices.Contracts
{
    using System;
    
    public interface IDataConnector : IDisposable
    {
        bool Connect();

        void Disconnect();
    }
}
