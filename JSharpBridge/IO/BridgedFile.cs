namespace CarbonCore.JSharpBridge.IO
{
    using CarbonCore.JSharpBridge.Net;

    public class BridgedFile
    {
        public BridgedFile(BridgedFile directory, string fileName)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public BridgedFile(string directory)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public BridgedFile GetParentFile()
        {
            Utils.Diagnostics.Internal.NotImplemented();
            return null;
        }

        public void Mkdirs()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public string GetAbsolutePath()
        {
            return Utils.Diagnostics.Internal.NotImplemented<string>();
        }

        public bool IsDirectory()
        {
            return Utils.Diagnostics.Internal.NotImplemented<bool>();
        }

        public void Delete()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public BridgedFile[] ListFiles(FileFilter resourcePackFilter = null)
        {
            Utils.Diagnostics.Internal.NotImplemented();
            return null;
        }

        public void Mkdir()
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public string GetName()
        {
            return Utils.Diagnostics.Internal.NotImplemented<string>();
        }

        public bool Exists()
        {
            return Utils.Diagnostics.Internal.NotImplemented<bool>();
        }

        public long LastModified()
        {
            return Utils.Diagnostics.Internal.NotImplemented<long>();
        }

        public URI ToURI()
        {
            return Utils.Diagnostics.Internal.NotImplemented<URI>();
        }

        public bool IsFile()
        {
            return Utils.Diagnostics.Internal.NotImplemented<bool>();
        }

        public void RenameTo(BridgedFile var8)
        {
            Utils.Diagnostics.Internal.NotImplemented();
        }

        public string[] List()
        {
            return Utils.Diagnostics.Internal.NotImplemented<string[]>();
        }
    }
}
