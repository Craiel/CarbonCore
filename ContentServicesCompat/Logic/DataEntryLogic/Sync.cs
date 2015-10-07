namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.Diagnostics;

    // NOTE:
    //  There is a unity issue with this class if the bool is after the generic Value<T>
    //  It will result in an empty error and a mono crash with the message
    //  `class->image->dynamic || field->offset > 0'

    [DebuggerDisplay("{Value}")]
    public class Sync<T>
        where T : struct
    {
        private bool isChanged;
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
            this.isChanged = isChanged;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsChanged
        {
            get
            {
                return this.isChanged;
            }
        }

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
                    this.isChanged = true;
                }
            }
        }

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
            this.isChanged = state;
        }
    }
}
