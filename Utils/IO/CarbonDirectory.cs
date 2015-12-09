namespace CarbonCore.Utils.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [JsonConverter(typeof(JsonCarbonDirectoryConverter))]
    public class CarbonDirectory : CarbonPath
    {
        public static readonly CarbonDirectory TempDirectory = new CarbonDirectory(System.IO.Path.GetTempPath());

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDirectory(string path)
            : base(path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Check if we are creating from a File
                if (File.Exists(path))
                {
                    throw new InvalidOperationException("File with the same name exists: " + path);
                }

                if (!this.EndsWithSeparator)
                {
                    this.Path += System.IO.Path.DirectorySeparatorChar;
                }
            }

            if (string.IsNullOrEmpty(this.Path))
            {
                return;
            }

            string trimmedPath = this.Path;
            if (this.EndsWithSeparator)
            {
                trimmedPath = this.Path.TrimEnd(System.IO.Path.DirectorySeparatorChar);
            }

            this.DirectoryName = this.Path;
            this.DirectoryNameWithoutPath = System.IO.Path.GetFileName(trimmedPath);
        }

        public CarbonDirectory(CarbonFile file)
            : this(file.DirectoryName)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool Exists
        {
            get
            {
                return Directory.Exists(this.DirectoryName);
            }
        }

        public static CarbonDirectory GetTempDirectory()
        {
            return TempDirectory.ToDirectory(System.IO.Path.GetRandomFileName());
        }

        public static IList<CarbonFileResult> GetFiles(IEnumerable<CarbonDirectoryFilter> filters)
        {
            System.Diagnostics.Debug.Assert(filters != null, "Filters need to be specified");

            IList<CarbonFileResult> results = new List<CarbonFileResult>();
            foreach (CarbonDirectoryFilter filter in filters)
            {
                if (filter.Directory.IsNull || !filter.Directory.Exists)
                {
                    System.Diagnostics.Trace.TraceWarning("Specified directory is invalid: {0}", filter.Directory);
                    continue;
                }

                foreach (string filterString in filter.FilterStrings)
                {
                    CarbonFileResult[] files = filter.Directory.GetFiles(filterString, filter.Option);
                    if (files == null)
                    {
                        continue;
                    }

                    foreach (CarbonFileResult file in files)
                    {
                        if (results.Contains(file))
                        {
                            continue;
                        }

                        results.Add(file);
                    }
                }
            }

            return results;
        }

        public static IList<CarbonDirectoryResult> GetDirectories(IEnumerable<CarbonDirectoryFilter> filters)
        {
            IList<CarbonDirectoryResult> results = new List<CarbonDirectoryResult>();
            foreach (CarbonDirectoryFilter filter in filters)
            {
                if (filter.Directory.IsNull || !filter.Directory.Exists)
                {
                    System.Diagnostics.Trace.TraceWarning("Specified directory is invalid: {0}", filter.Directory);
                    continue;
                }

                foreach (string filterString in filter.FilterStrings)
                {
                    CarbonDirectoryResult[] directories = filter.Directory.GetDirectories(filterString, filter.Option);
                    if (directories == null)
                    {
                        continue;
                    }

                    foreach (CarbonDirectoryResult directory in directories)
                    {
                        if (results.Contains(directory))
                        {
                            continue;
                        }

                        results.Add(directory);
                    }
                }
            }

            return results;
        }

        public static IList<CarbonDirectory> ReRootDirectories(CarbonDirectory root, IEnumerable<CarbonDirectory> directories)
        {
            System.Diagnostics.Trace.Assert(root != null && !root.IsNull, "Re-root requires valid root directory");

            IList<CarbonDirectory> results = new List<CarbonDirectory>();
            foreach (CarbonDirectory directory in directories)
            {
                System.Diagnostics.Debug.Assert(directory.IsRelative, "Can not re-root absolute directories!");

                results.Add(root.ToDirectory(directory));
            }

            return results;
        }

        public void Create()
        {
            if (this.Exists)
            {
                return;
            }

            Directory.CreateDirectory(this.DirectoryName);
        }

        public void Delete(bool recursive = false)
        {
            Directory.Delete(this.DirectoryName, recursive);
        }
        
        public CarbonDirectory ToDirectory<T>(params T[] other)
        {
            return new CarbonDirectory(this.CombineBefore(other));
        }

        public CarbonDirectory GetParent()
        {
            string trimmedPath = this.TrimEnd(DirectorySeparator, DirectorySeparatorAlternative);
            var info = new DirectoryInfo(trimmedPath);
            if (info.Parent != null)
            {
                var subDirRegex = new Regex(string.Format(DirectoryRegex, info.Parent.Name));
                MatchCollection matches = subDirRegex.Matches(trimmedPath);
                if (matches.Count <= 0)
                {
                    return null;
                }

                Match lastMatch = matches[matches.Count - 1];
                return new CarbonDirectory(trimmedPath.Substring(0, lastMatch.Index + lastMatch.Value.Length));
            }

            return null;
        }

        public CarbonFile ToFile<T>(params T[] other)
        {
            return new CarbonFile(this.CombineBefore(other));
        }

        public long GetFreeSpace()
        {
            if (this.Drive == null)
            {
                Diagnostic.Warning("GetFreeSpace called with no drive available!");
                return 0;
            }

            return this.Drive.AvailableFreeSpace;
        }

        public void RemoveAttributes(params FileAttributes[] attributes)
        {
            var info = new DirectoryInfo(this.Path);
            foreach (FileAttributes attribute in attributes)
            {
                info.Attributes &= ~attribute;
            }
        }

        public CarbonFileResult[] GetFiles(string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!this.Exists)
            {
                return new CarbonFileResult[0];
            }

            string[] files = Directory.GetFiles(this.DirectoryName, pattern, options);
            var results = new CarbonFileResult[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                string relative = files[i].Replace(this.ToString(), string.Empty);
                var result = new CarbonFileResult
                                 {
                                     Root = this,
                                     Absolute = new CarbonFile(files[i]),
                                     Relative = new CarbonFile(relative)
                                 };
                results[i] = result;
            }

            return results;
        }

        public CarbonDirectoryResult[] GetDirectories(string pattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!this.Exists)
            {
                return new CarbonDirectoryResult[0];
            }

            string[] subDirectories = Directory.GetDirectories(this.DirectoryName, pattern, options);
            var results = new CarbonDirectoryResult[subDirectories.Length];
            for (int i = 0; i < subDirectories.Length; i++)
            {
                string relative = subDirectories[i].Replace(this.ToString(), string.Empty);
                var result = new CarbonDirectoryResult
                                 {
                                     Absolute = new CarbonDirectory(subDirectories[i]),
                                     Relative = new CarbonDirectory(relative)
                                 };
                results[i] = result;
            }

            return results;
        }

        public override string ToString()
        {
            return this.GetPath();
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var typed = obj as CarbonDirectory;
            if (typed == null)
            {
                return false;
            }

            return typed.Path == this.Path;
        }
    }
}
