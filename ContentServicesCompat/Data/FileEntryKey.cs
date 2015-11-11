namespace CarbonCore.ContentServices.Compat.Data
{
    public struct FileEntryKey
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileEntryKey(string fileName, string hash)
            : this()
        {
            this.File = fileName;
            this.Hash = hash;
        }

        public FileEntryKey(string hash)
            : this()
        {
            this.Hash = hash;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string File { get; private set; }

        public string Hash { get; private set; }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return ((FileEntryKey)obj).Hash.Equals(this.Hash);
        }

        public override int GetHashCode()
        {
            return this.Hash.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Key({0}|{1})", this.File, this.Hash);
        }
    }
}
