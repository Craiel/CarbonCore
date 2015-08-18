namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.Diagnostics;

    [DebuggerDisplay("{Value}")]
    public class Sync<T>
    {
        private T value;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Sync()
            : this(default(T))
        {
        }

        public Sync(T value)
        {
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

        public static implicit operator Sync<T>(T value)
        {
            return new Sync<T>(value);
        }

        public static bool operator !=(Sync<T> first, Sync<T> second)
        {
            return !first.Equals(second);
        }

        public static bool operator ==(Sync<T> first, Sync<T> second)
        {
            return first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            var typed = (Sync<T>)obj;
            if (this.value == null && typed.value == null)
            {
                return true;
            }

            if (this.value == null || typed.value == null)
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

        public void ResetChangeState()
        {
            this.IsChanged = false;
        }
    }
}
