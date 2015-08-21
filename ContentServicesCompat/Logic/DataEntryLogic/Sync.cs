namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Diagnostics;

    [DebuggerDisplay("{Value}")]
    public class Sync<T>
        where T : struct
    {
        private T value;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Sync()
            : this(default(T))
        {
        }

        public Sync(T value, bool isChanged = true)
        {
            this.value = value;
            this.IsChanged = isChanged;
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
                if (!this.value.Equals(value))
                {
                    this.value = value;
                    this.IsChanged = true;
                }
            }
        }

        public bool IsChanged { get; private set; }
        
        public override bool Equals(object obj)
        {
            return ((Sync<T>)obj).Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public void ResetChangeState(bool state = false)
        {
            this.IsChanged = state;
        }
    }
}
