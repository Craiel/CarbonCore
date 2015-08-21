namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts;

    public class CarbonDiagnostics : ICarbonDiagnostics
    {
        private readonly IDictionary<int, TraceLog> contextLogs;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDiagnostics()
        {
            this.contextLogs = new Dictionary<int, TraceLog>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void RegisterLogContext(int id, string name)
        {
            lock (this.contextLogs)
            {
                this.contextLogs.Add(id, new TraceLog(name));
            }
        }

        public void UnregisterLogContext(int id)
        {
            lock (this.contextLogs)
            {
                this.contextLogs.Remove(id);
            }
        }
        
        public ILog GetLogContext(int id)
        {
            // ReSharper disable once InconsistentlySynchronizedField
            return this.contextLogs[id];
        }

        public void SetMute(int managedThreadId, bool mute)
        {
            lock (this.contextLogs)
            {
                this.contextLogs[managedThreadId].IsMuted = mute;
            }
        }
    }
}
