namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Diagnostics;

    using CarbonCore.ContentServices.Contracts;

    [DebuggerDisplay("{Value}")]
    public class SyncCascadeReadOnly<T>
        where T : ISyncEntry
    {
        private readonly T value;

        private bool isChanged;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncCascadeReadOnly()
        {
            this.value = Activator.CreateInstance<T>();
            this.isChanged = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Value
        {
            get
            {
                return this.value;
            }
        }

        public bool IsChanged
        {
            get
            {
                return this.isChanged || this.value.IsChanged;
            }
        }
        
        public override bool Equals(object obj)
        {
            var typed = (SyncCascadeReadOnly<T>)obj;
            return typed.Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            if (this.value == null)
            {
                return 0;
            }

            return this.value.GetHashCode();
        }

        public void ResetChangeState(bool state = false)
        {
            this.isChanged = state;
            if (this.value != null)
            {
                this.value.ResetChangeState(state);
            }
        }
    }
}
