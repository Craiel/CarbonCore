namespace CarbonCore.Modules.D3Theory.Contracts
{
    using System;

    using CarbonCore.Modules.D3Theory.Data;

    public interface IEntity : IDisposable
    {
        bool IsAlive { get; }

        float GetValue(D3EntityAttribute attribute);
        void SetValue(D3EntityAttribute attribute, float value);
        float AddValue(D3EntityAttribute attribute, float value, float max);
        float RemoveValue(D3EntityAttribute attribute, float value);
    }
}
