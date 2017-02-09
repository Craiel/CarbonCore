namespace CarbonCore.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

#if !__MonoCS__
    using EnvDTE;
#endif

    public static class ResourceGeneration
    {
        private static readonly List<string> PlainResultList = new List<string>();
        private static readonly List<string> UriResultList = new List<string>();
        private static readonly List<string> XamlResultList = new List<string>();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IncludeEmbeddedResources { get; set; }

        public static IReadOnlyCollection<string> PlainResult => PlainResultList.AsReadOnly();

        public static IReadOnlyCollection<string> UriResult => UriResultList.AsReadOnly();

        public static IReadOnlyCollection<string> XamlResult => XamlResultList.AsReadOnly();

        public static void Initialize(object host, string templateFile)
        {
            CodeGenerationUtils.Initialize(host, templateFile);

            PlainResultList.Clear();
            UriResultList.Clear();
            XamlResultList.Clear();
        }

        public static void LocateResources()
        {
#if __MonoCS__
            throw new InvalidOperationException("Resource Generation is not supported in Mono!");
#else
            var candidates = new Queue<ProjectItem>();
            foreach (ProjectItem item in CodeGenerationUtils.CurrentProject.ProjectItems)
            {
                candidates.Enqueue(item);
            }

            while (candidates.Count > 0)
            {
                ProjectItem candidate = candidates.Dequeue();

                ProcessItem(candidate);

                if (candidate.ProjectItems == null)
                {
                    continue;
                }

                foreach (ProjectItem item in candidate.ProjectItems)
                {
                    candidates.Enqueue(item);
                }
            }
#endif
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
#if !__MonoCS__
        private static void ProcessItem(ProjectItem item)
        {
            if (item.Properties == null)
            {
                return;
            }

            string type = GetProperty(item, Consts.ItemPropertyType);
            if (type == null)
            {
                return;
            }

            if (type.Equals(Consts.ItemTypeEmbeddedResource))
            {
                if (!IncludeEmbeddedResources)
                {
                    return;
                }
            }
            else if (!type.Equals(Consts.ItemTypeResource))
            {
                return;
            }

            string file = item.FileNames[0].Replace(CodeGenerationUtils.ProjectPath, string.Empty);
            ResourceGenerationType generationType = DetermineType(file);

            PlainResultList.Add(file);
            switch (generationType)
            {
                case ResourceGenerationType.Unhandled:
                    {
                        UriResultList.Add("// Unhandled Resource: " + file);
                        return;
                    }

                case ResourceGenerationType.Icon:
                    {
                        file = file.Replace("\\", "/");

                        string id = System.IO.Path.GetFileNameWithoutExtension(file);
                        id = string.Concat("Icon", id[0].ToString(CultureInfo.InvariantCulture).ToUpper(), id.Substring(1, id.Length - 1));

                        UriResultList.Add(
                            $"public static readonly Uri {id}Uri = new Uri(\"pack://application:,,,/{CodeGenerationUtils.CurrentProject.Name};component/{file}\", UriKind.Absolute);");
                        XamlResultList.Add(string.Concat(@"<Image x:Shared=""false"" x:Key=""", id, @""" Source=""{Binding Source={x:Static resources:Static.", id, @"Uri}}""/>"));
                        return;
                    }
            }
        }

        private static string GetProperty(ProjectItem item, string name)
        {
            if (item.Properties == null)
            {
                return null;
            }

            foreach (Property property in item.Properties)
            {
                if (property.Name.Equals(name))
                {
                    return property.Value.ToString();
                }
            }

            return null;
        }
#endif

        private static ResourceGenerationType DetermineType(string file)
        {
            int index = file.IndexOf(System.IO.Path.DirectorySeparatorChar);
            if (index <= 0)
            {
                return ResourceGenerationType.Unhandled;
            }

            string rootDirectory = file.Substring(0, index);
            if (rootDirectory.Equals("Icons", StringComparison.OrdinalIgnoreCase))
            {
                return ResourceGenerationType.Icon;
            }

            return ResourceGenerationType.Unhandled;
        }
    }
}
