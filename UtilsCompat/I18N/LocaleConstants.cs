namespace CarbonCore.Utils.Compat.I18N
{
    using System.Globalization;

    public static class LocaleConstants
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static readonly CultureInfo LocaleEnglishGB = new CultureInfo("en-GB");
        public static readonly CultureInfo LocaleEnglishUS = new CultureInfo("en-US");
        public static readonly CultureInfo LocaleFrench = new CultureInfo("fr-FR");
        public static readonly CultureInfo LocaleGerman = new CultureInfo("de-DE");

        public static CultureInfo GetCulture(string shortLangString)
        {
            switch (shortLangString)
            {
                case "fr": return LocaleFrench;
                case "de": return LocaleGerman;

                default: return LocaleEnglishUS;
            }
        }
    }
}
