namespace CarbonCore.Utils.I18N
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;

    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    public static class Localization
    {
        private const string SubDirectory = "i18n";
        private const string DictionaryFileName = "strings.json";

        private static readonly IDictionary<CultureInfo, LocalizationStringDictionary> Dictionaries;

        private static CultureInfo culture;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static Localization()
        {
            Dictionaries = new Dictionary<CultureInfo, LocalizationStringDictionary>();

            CurrentCulture = Thread.CurrentThread.CurrentCulture;

            Root = RuntimeInfo.Path;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static CarbonDirectory Root { get; private set; }

        public static CultureInfo CurrentCulture
        {
            get
            {
                return culture;
            }

            set
            {
                culture = value;
                if (!Thread.CurrentThread.CurrentCulture.Equals(culture))
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
            }
        }

        public static string Localized(this string source)
        {
            return Get(source);
        }

        public static string Get(string key)
        {
            if (!Dictionaries.ContainsKey(CurrentCulture))
            {
                LoadDictionary(CurrentCulture);
            }
            else
            {
                CheckDictionary(CurrentCulture);
            }

            LocalizationStringDictionary dictionary = Dictionaries[CurrentCulture];
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, key);
            }

            return dictionary[key];
        }

        public static void SaveDictionaries()
        {
            foreach (CultureInfo dictionaryCulture in Dictionaries.Keys)
            {
                SaveDictionary(dictionaryCulture);
            }
        }

        public static void ReloadDictionaries()
        {
            IList<CultureInfo> loadedCultures = new List<CultureInfo>(Dictionaries.Keys);
            foreach (CultureInfo dictionaryCulture in loadedCultures)
            {
                LoadDictionary(dictionaryCulture);
            }
        }

        public static void SetString(string key, string value)
        {
            System.Diagnostics.Trace.TraceWarning("Manual SetString called, prefer using the auto loaded dictionaries!");
            CheckDictionary(CurrentCulture);
            LocalizationStringDictionary dictionary = Dictionaries[CurrentCulture];
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
            else
            {
                dictionary[key] = value;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void LoadDictionary(CultureInfo info, CarbonFile source = null)
        {
            CheckDictionary(info);
            if (source == null)
            {
                source = Root.ToFile(SubDirectory, info.Name, DictionaryFileName);
            }

            if (!source.Exists)
            {
                System.Diagnostics.Trace.TraceWarning("Could not load dictionary for {0}, file not found: {1}", info.Name, source);
                return;
            }

            System.Diagnostics.Trace.TraceInformation("Loading Dictionary {0} ({1})", info.Name, source);
            Dictionaries[info] = JsonExtensions.LoadFromFile<LocalizationStringDictionary>(source, false);
        }

        private static void SaveDictionary(CultureInfo info, CarbonFile target = null)
        {
            if (target == null)
            {
                target = Root.ToFile(SubDirectory, info.Name, DictionaryFileName);
            }

            target.GetDirectory().Create();
            JsonExtensions.SaveToFile(target, Dictionaries[info], false, Formatting.Indented);
        }

        private static void CheckDictionary(CultureInfo info)
        {
            if (!Dictionaries.ContainsKey(info))
            {
                Dictionaries.Add(info, new LocalizationStringDictionary());
            }
        }
    }
}
