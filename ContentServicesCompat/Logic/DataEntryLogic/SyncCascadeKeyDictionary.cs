namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.ContentServices.Compat.Contracts;

    public class SyncCascadeKeyDictionary<T, TK, TV> : SyncDictionary<T, TK, TV>
        where T : IDictionary<TK, TV>
        where TK : ISyncEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged || this.Keys.Any(x => x.IsChanged);
            }
        }

        public override void ResetChangeState(bool state = false)
        {
            base.ResetChangeState(state);

            foreach (TK key in this.Keys)
            {
                key.ResetChangeState(state);
            }
        }
    }
}
