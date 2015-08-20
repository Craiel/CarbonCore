namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.Diagnostics;

    using CarbonCore.Utils.Compat;

    [DebuggerDisplay("{Value}")]
    public class SyncNull<T>
    {
        private T value;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncNull()
            : this(default(T))
        {
        }

        public SyncNull(T value)
        {
            Trace.Assert(typeof(T).IsNullable(), "Use this sync only for nullable types!");

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
            var typed = (SyncNull<T>)obj;
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
