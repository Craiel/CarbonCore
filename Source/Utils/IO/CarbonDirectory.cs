namespace CarbonCore.Utils.IO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    using NLog;

    [JsonConverter(typeof(JsonCarbonDirectoryConverter))]
    public class CarbonDirectory : CarbonPath
    {
        public static readonly CarbonDirectory TempDirectory = new CarbonDirectory(System.IO.Path.GetTempPath());

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string trimmedPath;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDirectory(string path, bool assertFileExistence)
            : base(path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // Check if we are creating from a File
                if (assertFileExistence && File.Exists(path))
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

            this.trimmedPath = this.Path;
            if (this.EndsWithSeparator)
            {
                this.trimmedPath = this.Path.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar);
            }

            this.GetPathUsingDefaultSeparator();
            this.DirectoryName = this.Path;
            this.DirectoryNameWithoutPath = System.IO.Path.GetFileName(this.GetStringUsingDefaultSeparator(this.trimmedPath));
        }

        public CarbonDirectory(CarbonFile file)
            : this(file.DirectoryName, true)
        {
        }

        public CarbonDirectory(string path)
            : this(path, true)
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
                if (filter.Directory == null || filter.Directory.IsNull || !filter.Directory.Exists)
                {
                    Logger.Warn("Specified directory is invalid: {0}", filter.Directory);
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
                    Logger.Warn("Specified directory is invalid: {0}", filter.Directory);
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
            Debug.Assert(root != null && !root.IsNull, "Re-root requires valid root directory");

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
            int expectedLength = this.trimmedPath.Length - this.DirectoryNameWithoutPath.Length - 1;
            if (expectedLength <= 0)
            {
                // No parent anymore
                return null;
            }

            string parentPath = this.trimmedPath.Substring(0, expectedLength);

            var result = new CarbonDirectory(parentPath);
            if (string.IsNullOrEmpty(result.DirectoryNameWithoutPath))
            {
                return null;
            }

            return result;
        }

        public CarbonFile ToFile<T>(params T[] other)
        {
            return new CarbonFile(this.CombineBefore(other));
        }

        public long GetFreeSpace()
        {
            if (this.Drive == null)
            {
                Logger.Warn("GetFreeSpace called with no drive available!");
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
                                     Relative = new CarbonDirectory(relative, assertFileExistence: false)
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

        public CarbonDirectory FindParent(string name, bool matchFullName = true, bool caseSensitive = false)
        {
            CarbonDirectory firstParent = this.GetParent();
            if (firstParent == null)
            {
                return null;
            }

            var queue = new Queue<CarbonDirectory>();
            queue.Enqueue(firstParent);
            while (queue.Count > 0)
            {
                var directory = queue.Dequeue();
                bool isMatch;
                if (matchFullName)
                {
                    isMatch = directory.DirectoryNameWithoutPath.Equals(name, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    isMatch = caseSensitive 
                        ? directory.DirectoryNameWithoutPath.Contains(name) 
                        : directory.DirectoryNameWithoutPath.ToLowerInvariant().Contains(name.ToLowerInvariant());
                }

                if (isMatch)
                {
                    return directory;
                }

                CarbonDirectory parent = directory.GetParent();
                if (parent == null || parent.Equals(directory))
                {
                    break;
                }

                queue.Enqueue(parent);
            }

            return null;
        }
    }
}
