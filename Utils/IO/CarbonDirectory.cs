namespace CarbonCore.Utils.IO
{
    using System.IO;

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

        public CarbonFile ToFile<T>(params T[] other)
        {
            return new CarbonFile(this.CombineBefore(other));
        }

        public CarbonFile[] GetFiles(string pattern, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!this.Exists)
            {
                return new CarbonFile[0];
            }

            string[] files = Directory.GetFiles(this.DirectoryName, pattern, options);
            var results = new CarbonFile[files.Length];
            for (int i = 0; i < files.Length; i++)
            {
                results[i] = new CarbonFile(files[i]);
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
