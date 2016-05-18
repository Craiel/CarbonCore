namespace CarbonCore.Utils
{
    using System;

    using CarbonCore.Utils.Diagnostics;

    // Java-like UUID structure with compatibility for GUID
    // based on https://gist.github.com/rickbeerendonk/13655dd24ec574954366
    public struct Uuid : IEquatable<Uuid>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Uuid(Guid guid)
            : this()
        {
            Diagnostic.Assert(BitConverter.IsLittleEndian, "Uuid is untested in big endian systems!");

            this.Guid = guid;

            this.LoadFromGuid();
        }

        public Uuid(long mostSignificant, long leastSignificant)
            : this()
        {
            Diagnostic.Assert(BitConverter.IsLittleEndian, "Uuid is untested in big endian systems!");

            this.MostSignificantBits = mostSignificant;
            this.LeastSignificantBits = leastSignificant;

            this.GenerateGuid();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Guid Guid { get; private set; }

        public long LeastSignificantBits { get; private set; }
        public long MostSignificantBits { get; private set; }

        public static bool operator ==(Uuid a, Uuid b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Uuid a, Uuid b)
        {
            return !a.Equals(b);
        }

        public static Uuid FromString(string input)
        {
            return new Uuid(new Guid(input));
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Uuid))
            {
                return false;
            }

            Uuid uuid = (Uuid)obj;

            return this.Equals(uuid);
        }
        
        public bool Equals(Uuid uuid)
        {
            return this.MostSignificantBits == uuid.MostSignificantBits && this.LeastSignificantBits == uuid.LeastSignificantBits;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for this instance.</returns>
        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.MostSignificantBits, this.LeastSignificantBits);
        }
        
        public override string ToString()
        {
            return this.Guid.ToString("D");
        }

        private void LoadFromGuid()
        {
            // TODO: Handle big endian
            byte[] guidBytes = this.Guid.ToByteArray();
            byte[] uuidBytes = 
                {
                    guidBytes[6],
                    guidBytes[7],
                    guidBytes[4],
                    guidBytes[5],
                    guidBytes[0],
                    guidBytes[1],
                    guidBytes[2],
                    guidBytes[3],
                    guidBytes[15],
                    guidBytes[14],
                    guidBytes[13],
                    guidBytes[12],
                    guidBytes[11],
                    guidBytes[10],
                    guidBytes[9],
                    guidBytes[8]
                };

            this.MostSignificantBits = BitConverter.ToInt64(uuidBytes, 0);
            this.LeastSignificantBits = BitConverter.ToInt64(uuidBytes, 8);
        }

        private void GenerateGuid()
        {
            byte[] mostSignificantBytes;
            byte[] leastSignificantBytes;
            if (BitConverter.IsLittleEndian == false)
            {
                mostSignificantBytes = BitConverter.GetBytes(this.LeastSignificantBits);
                leastSignificantBytes = BitConverter.GetBytes(this.MostSignificantBits);
            }
            else
            {
                mostSignificantBytes = BitConverter.GetBytes(this.MostSignificantBits);
                leastSignificantBytes = BitConverter.GetBytes(this.LeastSignificantBits);
            }

            byte[] guidBytes = 
                {
                    mostSignificantBytes[4],
                    mostSignificantBytes[5],
                    mostSignificantBytes[6],
                    mostSignificantBytes[7],
                    mostSignificantBytes[2],
                    mostSignificantBytes[3],
                    mostSignificantBytes[0],
                    mostSignificantBytes[1],
                    leastSignificantBytes[7],
                    leastSignificantBytes[6],
                    leastSignificantBytes[5],
                    leastSignificantBytes[4],
                    leastSignificantBytes[3],
                    leastSignificantBytes[2],
                    leastSignificantBytes[1],
                    leastSignificantBytes[0]
                };

            this.Guid = new Guid(guidBytes);
        }
    }
}
