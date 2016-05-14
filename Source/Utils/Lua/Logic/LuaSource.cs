namespace CarbonCore.Utils.Lua.Logic
{
    using CarbonCore.Utils.IO;

    public struct LuaSource
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaSource(CarbonFile file)
            : this()
        {
            this.FileSource = file;
            this.IsFile = true;
        }

        public LuaSource(string customData)
            : this()
        {
            this.CustomData = customData;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsFile { get; private set; }

        public CarbonFile FileSource { get; private set; }
        
        public string CustomData { get; private set; }
        
        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.FileSource, this.CustomData);
        }

        public override bool Equals(object obj)
        {
            var other = (LuaSource)obj;
            return other.FileSource.Equals(this.FileSource) && other.CustomData.Equals(this.CustomData);
        }
    }
}
