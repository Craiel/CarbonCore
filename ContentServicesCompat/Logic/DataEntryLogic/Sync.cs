namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Diagnostics;

    [DebuggerDisplay("{Value}")]
    public struct Sync<T>
        where T : struct
    {
        private readonly bool isUnchanged;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Sync(T value, bool isUnchanged = false)
            : this()
        {
            this.Value = value;
            this.isUnchanged = isUnchanged;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Value { get; private set; }
        
        public bool IsChanged
        {
            get
            {
                return !this.isUnchanged;
            }
        }

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
            return ((Sync<T>)obj).Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public Sync<T> ResetChangeState(bool state = false)
        {
            return new Sync<T>(this.Value, !state);
        }
    }
}
