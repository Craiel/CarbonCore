namespace CarbonCore.ContentServices.Logic
{
    using CarbonCore.Utils;

    public struct FileEntryKey
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileEntryKey(string fileName)
            : this()
        {
            this.File = fileName;
            this.Hash = HashFileName.GetHashFileName(fileName, HashFileNameMethod.SHA1);
        }

        public FileEntryKey(HashFileName hash)
            : this()
        {
            this.Hash = hash;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string File { get; private set; }

        public HashFileName Hash { get; private set; }

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
