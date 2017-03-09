namespace CarbonCore.Utils.Lua.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using CarbonCore.Utils.IO;

    using NLog;

    public static class LuaPreProcessor
    {
        private const string LuaCommentStart = "--";
        private const string LuaVariableCheck = "$(";

        private const char LuaCommentDirectiveChar = '#';
        private const char IncludeInternalStart = '<';
        private const char IncludeInternalEnd = '>';
        private const char IncludeExternal = '"';

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Regex LuaPreprocessorVariablesRegex = new Regex(@"\$\((\w+)\)", RegexOptions.Compiled);

        private static readonly IDictionary<string, string> Variables = new Dictionary<string, string>();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void DefineVariable(string key, string value)
        {
            if (Variables.ContainsKey(key))
            {
                Variables[key] = value;
            }
            else
            {
                Variables.Add(key, value);
            }
        }

        public static void DefineVariableFromPath(string key, CarbonDirectory path)
        {
            DefineVariable(key, path.ToString().Replace(@"\", @"\\"));
        }

        public static void DefineVariableFromPath(string key, CarbonFile path)
        {
            DefineVariable(key, path.ToString().Replace(@"\", @"\\"));
        }

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
            for (var i = 0; i < context.SourceData.Count; i++)
            {
                context.CurrentLineIndex = i;
                context.CurrentLineSource = context.SourceData[i];
                context.CurrentLineTarget = context.CurrentLineSource;
                context.IncludeCurrentLine = true;

                ProcessVariables(context);
                ProcessComments(context);

                if (context.IncludeCurrentLine)
                {
                    context.ProcessedData.Add(context.CurrentLineTarget);
                }
            }
        }

        private static void ProcessVariables(LuaPreProcessingContext context)
        {
            if (!context.CurrentLineSource.Contains(LuaVariableCheck))
            {
                // No variables in the line
                return;
            }
            
            MatchCollection matches = LuaPreprocessorVariablesRegex.Matches(context.CurrentLineSource);
            if (matches.Count <= 0)
            {
                // No matches
                return;
            }

            IList<string> variableKeys = new List<string>();
            foreach (Match match in matches)
            {
                string key = match.Groups[1].Value;
                if (variableKeys.Contains(key) || !Variables.ContainsKey(key))
                {
                    continue;
                }

                variableKeys.Add(key);
            }

            foreach (string key in variableKeys)
            {
                string replacementPattern = string.Format("$({0})", key);
                context.CurrentLineTarget = context.CurrentLineTarget.Replace(replacementPattern, Variables[key]);
            }
        }

        private static void ProcessComments(LuaPreProcessingContext context)
        {
            if (!context.CurrentLineTarget.StartsWith(LuaCommentStart) || context.CurrentLineTarget.Length <= LuaCommentStart.Length)
            {
                return;
            }

            string line = context.CurrentLineTarget.Trim();
            // Check if this comment is a preprocessor directive
            if (line[2] == LuaCommentDirectiveChar)
            {
                string directive = line.SubstringUntil(' ', 3);
                if (directive == null)
                {
                    Logger.Error("Could not determine directive for line {0}: {1}", context.CurrentLineIndex, line);
                    return;
                }

                string directiveParams = line.Substring(
                    3 + directive.Length,
                    line.Length - 3 - directive.Length);
                directiveParams = string.IsNullOrEmpty(directiveParams) ? null : directiveParams.Trim();

                switch (directive.ToLowerInvariant())
                {
                    case "include":
                        {
                            ProcessInclude(context, directiveParams);
                            break;
                        }
                }
            }
        }

        private static void ProcessInclude(LuaPreProcessingContext context, string includeParams)
        {
            bool isSystemInclude = includeParams[0] == IncludeInternalStart;
            string includeName = includeParams.TrimStart(IncludeInternalStart, IncludeExternal).TrimEnd(IncludeInternalEnd, IncludeExternal);

            if (isSystemInclude)
            {
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
                
                CarbonFile includeFile = new CarbonFile(includeName);
                if (!includeFile.Exists)
                {
                    includeFile = context.Source.FileSource.GetDirectory().ToFile(includeName);
                }
                
                Logger.Info("Processing Included script {0}", includeFile);

                LuaScript processedInclude = Process(includeFile);
                if (processedInclude == null)
                {
                    context.Error("Could not process included script " + includeFile);
                    return;
                }

                // Add the includes of the file
                foreach (string libraryInclude in processedInclude.LibraryIncludes)
                {
                    if (context.LibraryIncludes.Contains(libraryInclude))
                    {
                        continue;
                    }

                    context.LibraryIncludes.Add(libraryInclude);
                }

                context.ProcessedData.AddRange(processedInclude.Data);
            }

            context.IncludeCurrentLine = false;
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
                Logger.Error("Error in script processing: {0}", context.ErrorReason);
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
                Logger.Error("Error in script processing: {0}", context.ErrorReason);
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
