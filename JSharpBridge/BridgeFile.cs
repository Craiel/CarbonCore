namespace CarbonCore.JSharpBridge
{
    using System;
    using System.IO;

    public static class BridgeFile
    {
        public static FileInfo JavaOpenFile(string directory, string fileName)
        {
            return new FileInfo(Path.Combine(directory, fileName));
        }

        // Apparently we use File for directories as well...
        public static FileInfo JavaOpenFile(FileInfo directory, string fileName)
        {
            return new FileInfo(Path.Combine(directory.FullName, fileName));
        }
        
        public static string GetAbsolutePath(this FileInfo info)
        {
            throw new NotImplementedException();
        }
    }
}
