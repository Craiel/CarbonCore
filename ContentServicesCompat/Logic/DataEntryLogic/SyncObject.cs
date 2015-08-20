namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Diagnostics;

    using CarbonCore.ContentServices.Compat.Contracts;

    [DebuggerDisplay("{Value}")]
    public class SyncObject<T>
        where T : class
    {
        private T value;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncObject()
            : this(default(T))
        {
        }

        public SyncObject(T value)
        {
            Trace.Assert(typeof(T) != typeof(ISyncEntry), "Use SyncCascade for ISyncEntry objects!");

            this.value = value;
            this.IsChanged = true;
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
                this.IsChanged = true;
            }
        }

        public bool IsChanged { get; private set; }
        
        public override bool Equals(object obj)
        {
            var typed = (SyncObject<T>)obj;
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
            this.IsChanged = state;
        }
    }
}
