namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.ContentServices.Contracts;

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

        public bool IsDictionaryChanged
        {
            get
            {
                return base.IsChanged;
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
