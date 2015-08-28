namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.ContentServices.Compat.Contracts;

    public class SyncCascadeDictionary<T, TK, TV> : SyncDictionary<T, TK, TV>
        where T : IDictionary<TK, TV>
        where TK : ISyncEntry
        where TV : ISyncEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool IsChanged
        {
            get
            {
                return base.IsChanged || this.Any(x => x.Key.IsChanged || x.Value.IsChanged);
            }
        }

        public override void ResetChangeState(bool state = false)
        {
            base.ResetChangeState(state);

            foreach (KeyValuePair<TK, TV> pair in this)
            {
                pair.Key.ResetChangeState(state);
                pair.Value.ResetChangeState(state);
            }
        }
    }
}
