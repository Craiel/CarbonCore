namespace CarbonCore.Processing.Processors
{
    using System;
    using System.Diagnostics;
    using System.Text;

    using CarbonCore.Processing.Resource;
    using CarbonCore.Utils.IO;

    public enum TextureTargetFormat
    {
        Undefined,

        DDSRgb,
        DDSDxt1,
        DDSDxt3,
        DDSDxt5
    }

    public struct TextureProcessingOptions
    {
        public TextureTargetFormat Format;

        public bool IsNormalMap;

        public bool ConvertToNormalMap;

        public bool HasAlpha;
    }

    public static class TextureProcessor
    {
        private const string CompressionTool = "nvcompress.exe";
        public static CarbonDirectory TextureToolsPath { get; set; }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static RawResource Process(CarbonFile file, TextureProcessingOptions options)
        {
            if (options.Format == TextureTargetFormat.Undefined)
            {
                throw new ArgumentException("Target format was not defined properly");
            }

            if (!TextureToolsPath.Exists)
            {
                throw new InvalidOperationException("Texture tools have not been set or directory does not exist");
            }

            CarbonFile toolPath = TextureToolsPath.ToFile(CompressionTool);
            if (!toolPath.Exists)
            {
                throw new InvalidOperationException("Texture tool was not found in the expected location: " + toolPath);
            }

            CarbonFile tempFile = CarbonFile.GetTempFile();
            string parameter = BuildCompressionParameter(options);

            try
            {
                var startInfo = new ProcessStartInfo
                                    {
                                        FileName = toolPath.ToString(),
                                        Arguments = string.Format("{0} \"{1}\" \"{2}\"", parameter, file, tempFile),
                                        WorkingDirectory = Environment.CurrentDirectory,
                                        CreateNoWindow = true,
                                        UseShellExecute = false
                                    };

                var process = new Process { StartInfo = startInfo };
                process.Start();
                process.WaitForExit();

                if (!tempFile.Exists)
                {
                    throw new InvalidOperationException("Expected result file was not found after texture compression");
                }

                using (var stream = tempFile.OpenRead())
                {
                    var data = new byte[stream.Length];
                    stream.Read(data, 0, (int)stream.Length);
                    return new RawResource { Data = data };
                }
            }
            finally
            {
                // Make sure we clean up after
                tempFile.DeleteIfExists();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static string BuildCompressionParameter(TextureProcessingOptions options)
        {
            var builder = new StringBuilder();
            bool isNormal;
            if (options.ConvertToNormalMap)
            {
                builder.Append("-tonormal");
                isNormal = true;
            }
            else
            {
                isNormal = options.IsNormalMap;
                builder.Append(options.IsNormalMap ? "-normal " : "-color ");
            }

            if (options.HasAlpha)
            {
                builder.Append("-alpha ");
            }

            switch (options.Format)
            {
                case TextureTargetFormat.DDSRgb:
                    {
                        builder.Append("-rgb ");
                        break;
                    }

                case TextureTargetFormat.DDSDxt1:
                    {
                        builder.Append(isNormal ? "-bc1n " : "-bc1 ");
                        break;
                    }

                case TextureTargetFormat.DDSDxt3:
                    {
                        builder.Append("-bc2 ");
                        break;
                    }

                case TextureTargetFormat.DDSDxt5:
                    {
                        builder.Append(isNormal ? "-bc3n " : "-bc3 ");
                        break;
                    }

                default:
                    {
                        Utils.Edge.Diagnostics.Internal.NotImplemented("Compression format has no setting: " + options.Format);
                        break;
                    }
            }

            return builder.ToString();
        }
    }
}
