namespace CarbonCore.Utils.Lua.Logic
{
    using System;
    using System.Text.RegularExpressions;

    using CarbonCore.Utils.IO;

    public static class LuaPreProcessor
    {
        private static readonly Regex LuaSingleLineCommentRegex = new Regex(@"^[\s\t]*--[^\[]", RegexOptions.Compiled);

        private static readonly Regex LuaIncludeDirectiveRegex = new Regex(@"#include\s+([\<\""])(.*?)[\>\""]", RegexOptions.IgnoreCase);
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static LuaScript Process(CarbonFile file, bool allowCaching = true)
        {
            return DoProcess(new LuaSource(file), allowCaching);
        }

        public static LuaScript Process(string scriptData, bool allowCaching = true)
        {
            return DoProcess(new LuaSource(scriptData), allowCaching);
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
                context.Source = source;
                LoadCurrentSource(context);

                // Now do the actual processing
                ProcessSourceData(context);
            }
        }

        private static void LoadCurrentSource(LuaPreProcessingContext context)
        {
            if (context.Source.IsFile)
            {
                context.SourceData.AddRange(context.Source.FileSource.ReadAsList());
            }
            else
            {
                context.SourceData.AddRange(context.Source.CustomData.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
            }
        }

        private static void ProcessSourceData(LuaPreProcessingContext context)
        {
            for(var i = 0; i < context.SourceData.Count; i++)
            {
                context.CurrentLineIndex = i;
                context.CurrentLine = context.SourceData[i];
                context.IncludeCurrentLine = true;

                ProcessSourceInclude(context);

                if (context.IncludeCurrentLine)
                {
                    context.ProcessedData.Add(context.CurrentLine);
                }
            }
        }
        
        private static void ProcessSourceInclude(LuaPreProcessingContext context)
        {
            Match includeMatch = LuaIncludeDirectiveRegex.Match(context.CurrentLine);
            if (includeMatch.Success && !LuaSingleLineCommentRegex.IsMatch(context.CurrentLine))
            {
                if (includeMatch.Groups[1].Value == "<")
                {
                    string includeName = includeMatch.Groups[2].Value;
                    if (context.LibraryIncludes.Contains(includeName))
                    {
                        return;
                    }

                    context.LibraryIncludes.Add(includeName);
                }
                else
                {
                    if (!context.Source.IsFile)
                    {
                        context.Error("Can not have file include in script, script has invalid source info");
                        return;
                    }

                    string includeFileName = includeMatch.Groups[2].Value;
                    CarbonFile includeFile = context.Source.FileSource.GetDirectory().ToFile(includeFileName);
                    Diagnostics.Diagnostic.Info("Processing Included script {0}", includeFile);

                    LuaScript processedInclude = Process(includeFile);
                    if (processedInclude == null)
                    {
                        context.Error("Could not process included script " + includeFile);
                        return;
                    }

                    context.ProcessedData.AddRange(processedInclude.Data);
                }

                context.IncludeCurrentLine = false;
            }
        }

        private static LuaScript DoProcess(LuaSource source, bool allowCaching)
        {
            if (allowCaching)
            {
                LuaScript result = DoProcessCached(source);
                return result;
            }

            return DoProcessNonCached(source);
        }

        private static LuaScript DoProcessNonCached(LuaSource source)
        {
            if (source.IsFile && !source.FileSource.Exists)
            {
                return null;
            }

            LuaPreProcessingContext context = new LuaPreProcessingContext(source);

            Process(context);

            if (context.HasError)
            {
                Diagnostics.Diagnostic.Error("Error in script processing: {0}", context.ErrorReason);
                return null;
            }

            return BuildResult(context);
        }

        private static LuaScript DoProcessCached(LuaSource source)
        {
            LuaScript result = LuaCache.GetScript(source);
            if (result != null)
            {
                return result;
            }

            if (source.IsFile && !source.FileSource.Exists)
            {
                return null;
            }

            LuaPreProcessingContext context = new LuaPreProcessingContext(source);

            Process(context);

            if (context.HasError)
            {
                Diagnostics.Diagnostic.Error("Error in script processing: {0}", context.ErrorReason);
                return null;
            }
            
            return BuildResult(context);
        }

        private static LuaScript BuildResult(LuaPreProcessingContext context)
        {
            var result = new LuaScript(context.Source, context.ProcessedData);
            result.LibraryIncludes.AddRange(context.LibraryIncludes);
            return result;
        }
    }
}
