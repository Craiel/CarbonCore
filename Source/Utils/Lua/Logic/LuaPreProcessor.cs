namespace CarbonCore.Utils.Lua.Logic
{
    using System;
    using System.Linq;

    using CarbonCore.Utils.IO;

    public class LuaPreProcessor
    {
        private readonly LuaPreProcessingContext context;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LuaPreProcessor()
        {
            this.context = new LuaPreProcessingContext();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Process(CarbonFile file, bool allowCaching = true)
        {
            return this.DoProcess(new LuaSource(file), allowCaching);
        }

        public string Process(string scriptData, bool allowCaching = true)
        {
            return this.DoProcess(new LuaSource(scriptData), allowCaching);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void Process(LuaPreProcessingContext context)
        {
            while (context.ProcessingStack.Count > 0)
            {
                LuaSource source = context.ProcessingStack.Pop();

                // Set and load the next source
                context.CurrentSource = source;
                LoadCurrentSource(context);

                // Now do the actual processing
                ProcessSourceData(context);
            }
        }

        private static void LoadCurrentSource(LuaPreProcessingContext context)
        {
            if (context.CurrentSource == null)
            {
                throw new InvalidOperationException("Current lua source was null!");
            }

            if (context.CurrentSource.Value.IsFile)
            {
                context.CurrentSourceData.AddRange(context.CurrentSource.Value.FileSource.ReadAsList());
            }
            else
            {
                context.CurrentSourceData.AddRange(context.CurrentSource.Value.CustomData.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
            }
        }

        private static void ProcessSourceData(LuaPreProcessingContext context)
        {
            foreach (string line in context.CurrentSourceData)
            {
                ProcessSourceLine(context, line);
            }
        }

        private static void ProcessSourceLine(LuaPreProcessingContext context, string line)
        {
            // TODO
            context.FinalScriptData.Add(line);
        }

        private string DoProcess(LuaSource source, bool allowCaching)
        {
            if (allowCaching)
            {
                LuaCachedScript result = this.DoProcessCached(source);
                return result.GetCompleteData();
            }

            return this.DoProcessNonCached(source);
        }

        private string DoProcessNonCached(LuaSource source)
        {
            if (source.IsFile && !source.FileSource.Exists)
            {
                return null;
            }
            
            this.context.Prepare(source);
            Process(this.context);

            string resultData = string.Join(Environment.NewLine, this.context.FinalScriptData.ToArray());
            return resultData;
        }
        
        private LuaCachedScript DoProcessCached(LuaSource source)
        {
            LuaCachedScript result = LuaCache.GetScript(source);
            if (result != null)
            {
                return result;
            }

            if (source.IsFile && !source.FileSource.Exists)
            {
                return null;
            }

            this.context.Prepare(source);
            Process(this.context);
            
            return LuaCache.SetScript(source, this.context.FinalScriptData);
        }
    }
}
