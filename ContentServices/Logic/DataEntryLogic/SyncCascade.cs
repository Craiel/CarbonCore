namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.Diagnostics;

    using CarbonCore.ContentServices.Contracts;

    [DebuggerDisplay("{Value}")]
    public class SyncCascade<T>
        where T : ISyncEntry
    {
        private T value;

        private bool isChanged;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncCascade()
            : this(default(T))
        {
        }

        public SyncCascade(T value)
        {
            this.value = value;
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

            set
            {
                if ((this.Value == null && value == null)
                    || (this.value != null && this.value.Equals(value)))
                {
                    return;
                }

                this.value = value;
                this.isChanged = true;
            }
        }

        public bool IsChanged
        {
            get
            {
                return this.isChanged || (this.value != null && this.value.GetEntryChanged());
            }
        }
        
        public override bool Equals(object obj)
        {
            var typed = (SyncCascade<T>)obj;
            if (this.value == null && typed.Value == null)
            {
                return true;
            }

            if (this.value == null || typed.Value == null)
            {
                return false;
            }

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
