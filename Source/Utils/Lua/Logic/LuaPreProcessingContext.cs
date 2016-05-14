namespace CarbonCore.Utils.Lua.Logic
{
    using System.Collections.Generic;
    
    public class LuaPreProcessingContext
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaPreProcessingContext()
        {
            this.FinalScriptData = new List<string>();
            this.ProcessingStack = new Stack<LuaSource>();
            this.CurrentSourceData = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<string> FinalScriptData { get; private set; }

        public Stack<LuaSource> ProcessingStack { get; private set; }

        public IList<string> CurrentSourceData { get; private set; }

        public LuaSource? CurrentSource { get; set; }

        public void Prepare(LuaSource source)
        {
            this.FinalScriptData.Clear();
            this.ProcessingStack.Clear();
            this.ProcessingStack.Push(source);

            this.CurrentSourceData.Clear();
            this.CurrentSource = null;
        }
    }
}
