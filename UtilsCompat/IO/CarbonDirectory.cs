﻿namespace CarbonCore.Utils.Compat.IO
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Utils.Compat.Json;

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
                    this.Path = System.IO.Path.GetDirectoryName(path);
                }

                if (!this.EndsWithSeperator)
                {
                    this.Path += System.IO.Path.DirectorySeparatorChar;
                }
            }

            this.DirectoryName = this.Path;
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
                    System.Diagnostics.Trace.TraceWarning("Specified directory is invalid: {0}", filter.Directory.ToString());
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
            var info = new DirectoryInfo(this.Path);
            if (info.Parent != null)
            {
                int index = this.Path.LastIndexOf(System.IO.Path.DirectorySeparatorChar + info.Parent.Name + System.IO.Path.DirectorySeparatorChar, System.StringComparison.Ordinal);
                return new CarbonDirectory(this.Path.Substring(0, index));
            }

            return null;
        }

        public CarbonFile ToFile<T>(params T[] other)
        {
            return new CarbonFile(this.CombineBefore(other));
        }

        public long GetFreeSpace()
        {
            return this.Drive.AvailableFreeSpace;
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
