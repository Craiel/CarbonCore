namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.ContentServices.Contracts;

    public class SyncCascadeValueDictionary<T, TK, TV> : SyncDictionary<T, TK, TV>
        where T : IDictionary<TK, TV>
        where TV : ISyncEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged || this.Values.Any(x => x.IsChanged);
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

            foreach (TV value in this.Values)
            {
                value.ResetChangeState(state);
            }
        }
    }
}
