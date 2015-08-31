namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using CarbonCore.ContentServices.Compat.Contracts;

    [DebuggerDisplay("{Value}")]
    public class SyncCascadeList<T, TN> : SyncList<T, TN>
        where T : IList<TN>
        where TN : ISyncEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged || this.Any(x => x.IsChanged);
            }
        }

        public bool IsListChanged
        {
            get
            {
                return base.IsChanged;
            }
        }

        public override void ResetChangeState(bool state = false)
        {
            base.ResetChangeState(state);

            foreach (TN entry in this)
            {
                entry.ResetChangeState(state);
            }
        }
    }
}
