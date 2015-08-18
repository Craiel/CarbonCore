namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.Diagnostics;
    using System.IO;

    using CarbonCore.ContentServices.Contracts;

    [DebuggerDisplay("{Value}")]
    public class SyncContent<T> : ISyncContent
    {
        private static DataEntryElementSerializer Serializer;
        private T value;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncContent()
            : this(default(T))
        {
        }

        public SyncContent(T value)
        {
            if (Serializer == null)
            {
                Serializer = DataEntrySyncDescriptors.GetSerializer(this.GetType());
            }

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
        
        public static bool operator !=(SyncContent<T> first, SyncContent<T> second)
        {
            return !first.Equals(second);
        }

        public static bool operator ==(SyncContent<T> first, SyncContent<T> second)
        {
            return first.Equals(second);
        }

        public override bool Equals(object obj)
        {
            var typed = (SyncContent<T>)obj;
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

        public int Save(Stream stream)
        {
            if (this.IsChanged)
            {
                stream.WriteByte(1);
                return 1 + Serializer.Serialize(stream, this.value);
            }

            stream.WriteByte(0);
            return 1;
        }

        public void Load(Stream stream)
        {
            if (stream.ReadByte() == 0)
            {
                return;
            }

            this.value = (T)Serializer.Deserialize(stream);
        }
    }
}
