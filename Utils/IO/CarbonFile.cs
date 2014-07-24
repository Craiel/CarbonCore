namespace CarbonCore.Utils.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;

    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [JsonConverter(typeof(JsonCarbonFileConverter))]
    public class CarbonFile : CarbonPath
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonFile(string path)
            : base(path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                this.FileName = System.IO.Path.GetFileName(path);
                this.Extension = System.IO.Path.GetExtension(path);
                this.DirectoryName = System.IO.Path.GetDirectoryName(path);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string FileName { get; protected set; }
        public string Extension { get; protected set; }

        public DateTime LastWriteTime
        {
            get
            {
                return File.GetLastWriteTime(this.Path);
            }
        }

        public long Size
        {
            get
            {
                if (!this.Exists)
                {
                    return -1;
                }

                var info = new FileInfo(this.Path);
                return info.Length;
            }
        }

        public override bool Exists
        {
            get
            {
                return File.Exists(this.Path);
            }
        }

        public string FileNameWithoutExtension
        {
            get
            {
                return System.IO.Path.GetFileNameWithoutExtension(this.FileName);
            }
        }

        public static CarbonFile GetTempFile()
        {
            return CarbonDirectory.TempDirectory.ToFile(System.IO.Path.GetRandomFileName());
        }

        public static CarbonFile GetRandomFile()
        {
            return new CarbonFile(System.IO.Path.GetRandomFileName());
        }

        public static bool FileExists(CarbonFile file)
        {
            return file != null && !file.IsNull && file.Exists;
        }

        public CarbonFile ChangeExtension(string newExtension)
        {
            if (this.IsNull)
            {
                throw new InvalidOperationException();
            }

            return new CarbonFile(System.IO.Path.ChangeExtension(this.Path, newExtension));
        }

        public FileStream OpenCreate(FileMode mode = FileMode.Create)
        {
            return new FileStream(this.Path, mode, FileAccess.ReadWrite, FileShare.Read);
        }

        public FileStream OpenRead()
        {
            return File.OpenRead(this.Path);
        }

        public FileStream OpenWrite(FileMode mode = FileMode.OpenOrCreate)
        {
            switch (mode)
            {
                case FileMode.Append:
                    {
                        return new FileStream(this.Path, mode, FileAccess.Write, FileShare.Read);
                    }

                default:
                    {
                        return new FileStream(this.Path, mode, FileAccess.ReadWrite, FileShare.Read);
                    }
            }
        }

        public XmlReader OpenXmlRead()
        {
            return XmlReader.Create(this.Path);
        }

        public XmlWriter OpenXmlWrite()
        {
            return XmlWriter.Create(this.Path);
        }

        public void Delete()
        {
            File.Delete(this.Path);
        }

        public void DeleteIfExists()
        {
            if (!this.Exists)
            {
                return;
            }

            this.Delete();
        }

        public void Move(CarbonFile target)
        {
            File.Move(this.Path, target.GetPath());
        }

        public CarbonDirectory GetDirectory()
        {
            if (string.IsNullOrEmpty(this.DirectoryName))
            {
                return null;
            }

            return new CarbonDirectory(this.DirectoryName);
        }
        
        public CarbonFile ToFile<T>(params T[] other)
        {
            return new CarbonFile(this.Path + string.Concat(other));
        }

        public override string ToString()
        {
            return this.Path;
        }

        public bool StartsWith(string pattern, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
        {
            return this.FileName.StartsWith(pattern, comparisonType);
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var typed = obj as CarbonFile;
            if (typed == null)
            {
                return false;
            }

            return typed.Path == this.Path;
        }

        public bool Equals(CarbonFile other, StringComparison comparison)
        {
            return other.GetPath().Equals(this.Path, comparison);
        }

        public CarbonFile Rotate(int maxRotations)
        {
            if (this.Exists)
            {
                // Build a list of all possible files
                IList<CarbonFile> files = new List<CarbonFile>();
                for (int i = maxRotations - 1; i >= 0; i--)
                {
                    files.Add(this.ChangeExtension(string.Concat(".", i, this.Extension)));
                }

                // Delete the oldest one if it's there
                files[0].DeleteIfExists();

                // Move all files one up
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].Exists)
                    {
                        files[i].Move(files[i - 1]);
                    }
                }

                this.Move(files[files.Count - 1]);
                return files[files.Count - 1];
            }

            return this;
        }
    }
}
