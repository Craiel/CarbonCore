namespace CarbonCore.Utils.Collections
{
    public class DefaultValueExtendedDictionary<T, TN> : ExtendedDictionary<T, TN>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override TN this[T key]
        {
            get
            {
                TN value;
                if (this.TryGetValue(key, out value))
                {
                    return value;
                }

                return default(TN);
            }

            set
            {
                if (!this.ContainsKey(key))
                {
                    this.Add(key, default(TN));
                }

                base[key] = value;
            }
        }

        public override T this[TN key]
        {
            get
            {
                T value;
                if (this.TryGetKey(key, out value))
                {
                    return value;
                }

                return default(T);
            }

            set
            {
                // We don't support default values on TN
                base[key] = value;
            }
        }
    }
}
