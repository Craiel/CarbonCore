namespace CarbonCore.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using EnvDTE;

    public static class ResourceGeneration
    {
        private static readonly List<string> PlainResultList = new List<string>();
        private static readonly List<string> UriResultList = new List<string>();
        private static readonly List<string> XamlResultList = new List<string>();

        public static Project currentProject;
        public static string projectPath;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IncludeEmbeddedResources { get; set; }

        public static IReadOnlyCollection<string> PlainResult
        {
            get
            {
                return PlainResultList.AsReadOnly();
            }
        }

        public static IReadOnlyCollection<string> UriResult
        {
            get
            {
                return UriResultList.AsReadOnly();
            }
        }

        public static IReadOnlyCollection<string> XamlResult
        {
            get
            {
                return XamlResultList.AsReadOnly();
            }
        }

        public static void Initialize(object host, string templateFile)
        {
            PlainResultList.Clear();
            UriResultList.Clear();
            XamlResultList.Clear();

            var provider = (IServiceProvider)host;
            var dteObject = (DTE)provider.GetService(typeof(DTE));

            currentProject = dteObject.Solution.FindProjectItem(templateFile).ContainingProject;

            projectPath = System.IO.Path.GetDirectoryName(currentProject.FullName) + System.IO.Path.DirectorySeparatorChar;
        }

        public static void LocateResources()
        {
            var candidates = new Queue<ProjectItem>();
            foreach (ProjectItem item in currentProject.ProjectItems)
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
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
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

            string file = item.FileNames[0].Replace(projectPath, string.Empty);
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
                        if (id == null)
                        {
                            return;
                        }

                        id = string.Concat("Icon", id[0].ToString(CultureInfo.InvariantCulture).ToUpper(), id.Substring(1, id.Length - 1));

                        UriResultList.Add(string.Format("public static readonly Uri {0}Uri = new Uri(\"pack://application:,,,/{1};component/{2}\", UriKind.Absolute);", id, currentProject.Name, file));
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
