namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Diagnostics;

    [DebuggerDisplay("{Value}")]
    public struct Sync<T>
        where T : struct
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Sync(T value, bool isChanged = true)
            : this()
        {
            this.Value = value;
            this.IsChanged = isChanged;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Value { get; private set; }

        public bool IsChanged { get; private set; }
        
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

        public Sync<T> Change(T newValue)
        {
            if (this.Value.Equals(newValue))
            {
                return new Sync<T>(this.Value, false);
            }

            return new Sync<T>(newValue);
        }

        public Sync<T> ChangeState(bool state = false)
        {
            return new Sync<T>(this.Value, state);
        }
    }
}
