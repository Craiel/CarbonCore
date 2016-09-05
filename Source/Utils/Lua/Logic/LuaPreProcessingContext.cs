namespace CarbonCore.Utils.Lua.Logic
{
    using System.Collections.Generic;
    
    public class LuaPreProcessingContext
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaPreProcessingContext(LuaSource source)
        {
            this.ProcessedData = new List<string>();
            this.ProcessingStack = new Stack<LuaSource>();
            this.SourceData = new List<string>();
            this.LibraryIncludes = new List<string>();

            this.Source = source;
            this.ProcessingStack.Push(source);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool HasError { get; set; }

        public string ErrorReason { get; set; }

        public Stack<LuaSource> ProcessingStack { get; private set; }

        public IList<string> ProcessedData { get; private set; }

        public IList<string> SourceData { get; private set; }

        public IList<string> LibraryIncludes { get; private set; }

        public int CurrentLineIndex { get; set; }

        public string CurrentLineSource { get; set; }

        public string CurrentLineTrimmed { get; set; }

        public string CurrentLineTarget { get; set; }

        public LuaSource Source { get; set; }

        public bool IncludeCurrentLine { get; set; }
        
        public void Error(string reason)
        {
            this.HasError = true;
            this.ErrorReason = reason;
        }
    }
}
