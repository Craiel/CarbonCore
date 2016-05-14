namespace CarbonCore.Utils.Lua.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    
    public class LuaCachedScript
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaCachedScript(LuaSource source, IList<string> data)
        {
            this.Source = source;
            this.Data = new List<string>();

            this.Update(data);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public LuaSource Source { get; private set; }

        public DateTime LastWriteTime { get; private set; }

        public long SourceSize { get; private set; }

        public IList<string> Data { get; private set; }

        public bool HasChanged
        {
            get
            {
                return this.Source.IsFile && this.Source.FileSource.Exists
                       && this.Source.FileSource.LastWriteTime == this.LastWriteTime
                       && this.Source.FileSource.Size == this.SourceSize;
            }
        }
        
        public string GetCompleteData()
        {
            return string.Join(Environment.NewLine, this.Data.ToArray());
        }

        public void Update(IList<string> data)
        {
            this.Data.Clear();
            this.Data.AddRange(data);

            if (this.Source.IsFile && this.Source.FileSource.Exists)
            {
                this.LastWriteTime = this.Source.FileSource.LastWriteTime;
                this.SourceSize = this.Source.FileSource.Size;
            }
        }
    }
}
