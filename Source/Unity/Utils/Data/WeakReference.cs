namespace CarbonCore.Unity.Utils.Data
{
    using System;
    using System.Runtime.Serialization;

    public class WeakReference<T> : ISerializable
    {
        private readonly WeakReference reference;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public WeakReference(T target)
        {
            this.reference = new WeakReference(target);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsAlive
        {
            get
            {
                return this.reference.IsAlive;
            }
        }

        public T Target
        {
            get
            {
                if (this.reference.IsAlive)
                {
                    return (T)this.reference.Target;
                }

                return default(T);
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            this.reference.GetObjectData(info, context);
        }
    }
}
