namespace CarbonCore.Processing.Processors
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    using CarbonCore.Processing.Resource;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;

    public delegate string ResolveIncludeDelegate(string include);
    
    public class ScriptProcessor
    {
        private static readonly Regex ScriptFieldRegex = new Regex("{([a-z]+)[\\s]*([^\"]*)}", RegexOptions.IgnoreCase);

        private static ScriptProcessingOptions? currentOptions;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ScriptResource Process(CarbonFile file, ScriptProcessingOptions options)
        {
            if (!CarbonFile.FileExists(file))
            {
                throw new ArgumentException("Invalid Script Processing options");
            }

            Diagnostic.Info("Processing script {0}", file);
            currentOptions = options;
            try
            {
                using (var stream = file.OpenRead())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string scriptData = reader.ReadToEnd();

                        scriptData = ScriptFieldRegex.Replace(scriptData, FieldEvaluator);

                        return new ScriptResource { Script = scriptData };
                    }
                }
            }
            finally
            {
                currentOptions = null;
            }
        }

        private static string FieldEvaluator(Match match)
        {
            if (match.Captures.Count <= 0 || match.Groups.Count < 2)
            {
                Diagnostic.Warning("Could not evaluate Resource, no capture data");
                return "ERROR";
            }

            string fieldId = match.Groups[1].Value.ToLower();
            string fieldValue = match.Groups.Count > 2 ? match.Groups[2].Value : null;
            switch (fieldId)
            {
                case "resource":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            Diagnostic.Warning("Argument missing in resource Field");
                            return "ERROR";
                        }

                        string hash = HashUtils.BuildResourceHash(fieldValue);
                        Diagnostic.Info(" Resource: {0} -> {1}", fieldValue, hash);
                        return hash;
                    }

                case "include":
                    {
                        if (string.IsNullOrEmpty(fieldValue))
                        {
                            Diagnostic.Warning("Argument missing in include Field");
                            return "ERROR";
                        }

                        if (currentOptions == null || currentOptions.Value.IncludeResolver == null)
                        {
                            Diagnostic.Warning("Include Resolver not present, can not resolve include field");
                            return "ERROR";
                        }

                        string include = currentOptions.Value.IncludeResolver(fieldValue);
                        Diagnostic.Info(" Include: {0} -> {1}", fieldValue, include);
                        return include;
                    }
            }

            Diagnostic.Warning("Unknown Field in Script: " + match.Captures[0].Value);
            return "ERROR";
        }
    }
}
