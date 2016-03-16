namespace CarbonCore.Unity.Utils
{
    using System;
    using System.IO;

    using CarbonCore.Utils.IO;

    using UnityEngine;

    public static class TextureExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Texture2D Rescale(this Texture2D source, int maxWidth, int maxHeight)
        {
            int newWidth = Math.Min(source.width, maxWidth);
            int newHeight = Math.Min(source.height, maxHeight);
            float ratio = source.width / (float)source.height;
            float newRatio = newWidth / (float)newHeight;
            if (Math.Abs(newRatio - ratio) > float.Epsilon)
            {
                if (newWidth < newHeight)
                {
                    newWidth = (int)(newHeight / ratio);
                }
                else
                {
                    newHeight = (int)(newWidth / ratio);
                }
            }

            return source.ResizeIncludingContent(newWidth, newHeight);
        }

        public static Texture2D ResizeIncludingContent(this Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
            Color[] rpixels = result.GetPixels(0);
            float incX = 1.0f / targetWidth;
            float incY = 1.0f / targetHeight;
            for (int px = 0; px < rpixels.Length; px++)
            {
                rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * Mathf.Floor(px / (float)targetWidth));
            }

            result.SetPixels(rpixels, 0);
            result.Apply();
            return result;
        }

        public static void SaveToPNGFile(this Texture2D source, CarbonFile file)
        {
            file.DeleteIfExists();
            using (var stream = file.OpenWrite(FileMode.CreateNew))
            {
                byte[] data = source.EncodeToPNG();
                stream.Write(data, 0, data.Length);
            }
        }

        public static void SaveToJPGFile(this Texture2D source, CarbonFile file, int quality = 75)
        {
            file.DeleteIfExists();
            using (var stream = file.OpenWrite(FileMode.CreateNew))
            {
                byte[] data = source.EncodeToJPG(quality);
                stream.Write(data, 0, data.Length);
            }
        }

        public static Texture2D LoadTexture2D(this CarbonFile source)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(source.ReadAsByte());
            return texture;
        }
    }
}
