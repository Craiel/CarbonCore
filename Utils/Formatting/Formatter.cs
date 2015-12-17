namespace CarbonCore.Utils.Formatting
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;

    using CarbonCore.Utils.Contracts;

    // Formats a string using a dictionary approach
    public class Formatter : IFormatter
    {
        private static readonly IDictionary<string, object> GlobalCustomDictionary = new Dictionary<string, object>();

        private static readonly Regex ParserExpression = new Regex(@"\{([^\}]+)\}", RegexOptions.Compiled);

        private readonly IDictionary<string, object> defaultDictionary;
        private readonly IDictionary<string, object> dictionary;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Formatter()
        {
            this.dictionary = new Dictionary<string, object>();

            this.defaultDictionary = new Dictionary<string, object>
                {
                    { "DATETIME", new FormatHandler { DefaultParameter = "g", HandlerFunction = this.HandleFormatDateTime } },
                    { "CORETIME", new FormatHandler { HandlerFunction = this.HandleFormatCoreTime } },
                    { "CORETIMESTRING", new FormatHandler { HandlerFunction = this.HandleFormatCoreTimeToTimeString } },
                    { "THREADID", new FormatHandler { HandlerFunction = this.HandleFormatThreadId } },
                    { "PROCESSID", new FormatHandler { HandlerFunction = this.HandleFormatProcessId } },
                    { "PROCESSNAME", new FormatHandler { HandlerFunction = this.HandleFormatProcessName } },
                    { "ASSEMBLYNAME", new FormatHandler { HandlerFunction = this.HandleFormatAssemblyName } },
                };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void SetGlobal(string rawKey, string value)
        {
            string key = rawKey.Trim().ToUpper();
            if (!GlobalCustomDictionary.ContainsKey(key))
            {
                GlobalCustomDictionary.Add(key, value);
            }
            else
            {
                GlobalCustomDictionary[key] = value;
            }
        }

        public void Clear()
        {
            this.dictionary.Clear();
        }

        public string Get(string rawKey)
        {
            string key = rawKey.Trim().ToUpper();
            return this.GetFormattedValue(key, null);
        }

        public string Format(string template)
        {
            return ParserExpression.Replace(template, this.FormatEvaluator);
        }
        
        public void Set(string rawKey, string value)
        {
            string key = rawKey.Trim().ToUpper();
            if (!this.dictionary.ContainsKey(key))
            {
                this.dictionary.Add(key, value);
            }
            else
            {
                this.dictionary[key] = value;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private string GetFormattedValue(string rawKey, string parameter)
        {
            string key = rawKey.Trim().ToUpper();
            object value;

            if (this.dictionary.TryGetValue(key, out value))
            {
                return this.GetFormattedValue(key, parameter, value);
            }
            
            if (this.defaultDictionary.TryGetValue(key, out value))
            {
                return this.GetFormattedValue(key, parameter, value);
            }

            if (GlobalCustomDictionary.TryGetValue(key, out value))
            {
                return this.GetFormattedValue(key, parameter, value);
            }

            return rawKey;
        }

        private string GetFormattedValue(string key, string parameter, object handlerValue)
        {
            Type handlerType = handlerValue.GetType();
            if (handlerType == typeof(string))
            {
                return handlerValue as string;
            }

            if (handlerType == typeof(FormatHandler))
            {
                return ((FormatHandler)handlerValue).Evaluate(parameter);
            }

            System.Diagnostics.Trace.TraceError("Unknown Handler ({0}) for GetFormatted Value of {1}", handlerType, key);
            return string.Empty;
        }

        private string FormatEvaluator(Match match)
        {
            string key = match.Groups[1].Value;
            if (key.Contains(":"))
            {
                string[] pair = key.Split(':');
                return this.GetFormattedValue(pair[0], pair[1]);
            }

            return this.GetFormattedValue(key, null);
        }

        // -------------------------------------------------------------------
        // Private Handlers
        // -------------------------------------------------------------------
        private string HandleFormatDateTime(string parameter)
        {
            return DateTime.Now.ToString(parameter);
        }

        private string HandleFormatCoreTime(string parameter)
        {
            return Utils.Timer.CoreTimer.ElapsedTime.ToString(parameter);
        }

        private string HandleFormatCoreTimeToTimeString(string parameter)
        {
            return TimeSpan.FromTicks(Utils.Timer.CoreTimer.ElapsedTime).ToString();
        }

        private string HandleFormatThreadId(string parameter)
        {
            return Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture);
        }

        private string HandleFormatProcessId(string parameter)
        {
            return RuntimeInfo.ProcessId.ToString(CultureInfo.InvariantCulture);
        }

        private string HandleFormatProcessName(string parameter)
        {
            return RuntimeInfo.ProcessName;
        }

        private string HandleFormatAssemblyName(string parameter)
        {
            return RuntimeInfo.AssemblyName;
        }
    }
}
