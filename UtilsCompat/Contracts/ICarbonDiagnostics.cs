namespace CarbonCore.Utils.Compat.Contracts
{
    public interface ICarbonDiagnostics
    {
        void RegisterLogContext(int id, string name);

        void UnregisterLogContext(int id);

        ILog GetLogContext(int id);
    }
}
