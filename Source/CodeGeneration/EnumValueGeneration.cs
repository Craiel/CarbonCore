namespace CarbonCore.CodeGeneration
{
    using System.Collections;
    using System.Collections.Generic;

    public static class EnumValueGeneration
    {
        private static readonly IList<string> Excludes = new List<string>();
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Initialize(object host, string templateFile)
        {
            CodeGenerationUtils.Initialize(host, templateFile);

            Excludes.Clear();
        }

        public static void Exclude(params string[] values)
        {
            foreach (string value in values)
            {
                Excludes.Add(value);
            }
        }

        public static IList<string> GetEnumNames()
        {
            string rootDirectory = System.IO.Path.GetDirectoryName(CodeGenerationUtils.TemplateFile);
            if (string.IsNullOrEmpty(rootDirectory) || !System.IO.Directory.Exists(rootDirectory))
            {
                return null;
            }

            IList<string> results = new List<string>();
            string[] files = System.IO.Directory.GetFiles(rootDirectory, "*.cs");
            foreach (string file in files)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(file);
                if (Excludes.Contains(name))
                {
                    continue;
                }

                results.Add(name);
            }

            return results;
        }

        public static string FormatEnumValueLine(string enumType)
        {
            return string.Format("        public static readonly IReadOnlyCollection<{0}> {0}Values = EnumExtensions.GetValues<{0}>();\r\n", enumType);
        }
    }
}
