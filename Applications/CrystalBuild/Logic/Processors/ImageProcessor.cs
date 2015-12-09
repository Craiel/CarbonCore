namespace CarbonCore.Applications.CrystalBuild.Logic.Processors
{
    using System;
    using System.Text;

    using CarbonCore.Applications.CrystalBuild.Contracts.Processors;
    using CarbonCore.Utils.IO;

    public class ImageProcessor : ContentProcessor, IImageProcessor
    {
        private static readonly  char[] SegmentSplitChars = @"\".ToCharArray();
        
        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void DoProcess(CarbonFile source)
        {
            string relativeRootPath = source.GetDirectory().GetPath().Replace(this.Context.Root.GetPath(), string.Empty);
            string fileName = source.FileName;
            string fileNameId = this.BuildPathId(relativeRootPath + source.FileNameWithoutExtension);
            string rootPathId = this.BuildPathId(relativeRootPath, true);

            this.Context.Cache.RegisterImage(fileNameId, $@"staticData.imageRoot{rootPathId} + ""{fileName}""");
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private string BuildPathId(string path, bool firstSegmentUpperCase = false)
        {
            string[] segments = path.Split(SegmentSplitChars, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder builder = new StringBuilder();
            for(var i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (firstSegmentUpperCase || i > 0)
                {
                    builder.Append(char.ToUpperInvariant(segment[0]));
                    builder.Append(segment.Substring(1, segment.Length - 1));
                }
                else
                {
                    builder.Append(segment);
                }
            }

            return builder.ToString();
        }
    }
}
