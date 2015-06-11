namespace CarbonCore.Utils.Compat.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;

    public abstract class CarbonPath
    {
        public static readonly string DirectorySeparator = System.IO.Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);
        public static readonly string DirectorySeparatorAlternative = System.IO.Path.AltDirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);

        // Note: this is win32 specific and might have to be adjusted for other platforms
        private static readonly Regex Win32AbsolutePathRegex = new Regex(@"^([a-z]):[\\\/]+(.*)$", RegexOptions.IgnoreCase);

        private string path;

        private DriveInfo drive;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected CarbonPath(string path)
        {
            this.Path = path;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string DirectoryName { get; protected set; }

        public bool IsNull { get; private set; }

        public bool EndsWithSeperator { get; private set; }

        public bool IsRelative { get; private set; }

        public abstract bool Exists { get; }

        public Uri GetUri(UriKind kind)
        {
            return new Uri(this.path, kind);
        }

        public override int GetHashCode()
        {
            return this.path.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (this.path == null)
            {
                return obj == null;
            }

            if (obj as CarbonPath == null)
            {
                return false;
            }
            
            return this.path.Equals(obj.ToString());
        }

        public bool EqualsPath(CarbonPath other, CarbonPath root)
        {
            // First we do a direct compare using the default equals
            if (this.Equals(other))
            {
                return true;
            }
            
            // Now lets try to find out if we are dealing with the same file by taking the absolute paths of both
            string thisString = this.GetAbsolutePath(root);
            string otherString = other.GetAbsolutePath(root);

            if (thisString.Equals(otherString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return this.path;
        }

        public virtual bool CopyTo(CarbonPath target, bool overwrite = false)
        {
            throw new NotImplementedException();
        }

        public T ToRelative<T>(CarbonPath other) where T : CarbonPath
        {
            string relativePath = this.GetRelativePath(other);
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }

            // Uri transforms this so we have to bring it back in line
            relativePath = relativePath.Replace("/", DirectorySeparator);
            return (T)Activator.CreateInstance(typeof(T), relativePath);
        }

        public T ToAbsolute<T>(CarbonPath root) where T : CarbonPath
        {
            if (root.IsRelative)
            {
                throw new ArgumentException();
            }

            string absolutePath = this.GetAbsolutePath(root);
            return (T)Activator.CreateInstance(typeof(T), absolutePath);
        }

        public bool Contains(string pattern)
        {
            return this.Path.Contains(pattern);
        }

        public string GetPath()
        {
            return this.path;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
        protected string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                this.path = value;

                if (string.IsNullOrEmpty(this.path))
                {
                    this.IsNull = true;
                }
                else
                {
                    this.EndsWithSeperator = this.path.EndsWith(DirectorySeparator)
                                             || this.path.EndsWith(DirectorySeparatorAlternative);

                    this.IsRelative = !Win32AbsolutePathRegex.IsMatch(this.path);
                }
            }
        }

        protected DriveInfo Drive
        {
            get
            {
                if (this.drive == null)
                {
                    this.UpdateDrive();
                }

                return this.drive;
            }
        }

        protected string CombineBefore<T>(params T[] other)
        {
            string result = this.Path;
            for (int i = 0; i < other.Length; i++)
            {
                string otherValue = other[i].ToString();
                result = string.IsNullOrEmpty(this.path) || this.HasDelimiter(result, otherValue) ? 
                    string.Concat(result, otherValue) :
                    string.Concat(result, DirectorySeparator, otherValue);
            }

            return result;
        }

        protected bool HasDelimiter(string first, string second)
        {
            return first.EndsWith(DirectorySeparator) || second.StartsWith(DirectorySeparator);
        }

        protected string GetRelativePath(CarbonPath other)
        {
            if (this.IsRelative)
            {
                return this.path;
            }

            return other.GetUri(UriKind.Absolute).MakeRelativeUri(this.GetUri(UriKind.Absolute)).OriginalString;
        }

        protected string GetAbsolutePath(CarbonPath other)
        {
            if (!this.IsRelative)
            {
                return this.path;
            }

            return new Uri(other.GetUri(UriKind.Absolute), this.path).AbsolutePath;
        }

        protected void UpdateDrive()
        {
            System.Diagnostics.Trace.Assert(!this.IsRelative);

            foreach (DriveInfo info in DriveInfo.GetDrives())
            {
                if (!info.IsReady)
                {
                    continue;
                }

                if (!this.GetPath().StartsWith(info.RootDirectory.FullName))
                {
                    continue;
                }

                this.drive = info;
                break;
            }
        }
    }
}
