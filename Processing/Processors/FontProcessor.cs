namespace CarbonCore.Processing.Processors
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;

    using CarbonCore.Processing.Resource;
    using CarbonCore.Utils.IO;

    public struct FontProcessingOptions
    {
        public FontStyle Style;

        public int Size;
        public int CharactersPerRow;
    }

    public static class FontProcessor
    {
        private static readonly bool[] Characters;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static FontProcessor()
        {
            var charSelection = new List<char>();
            for (byte c = 0; c < byte.MaxValue; c++)
            {
                var current = (char)c;
                if (char.IsLetterOrDigit(current) || char.IsPunctuation(current) || char.IsSymbol(current))
                {
                    charSelection.Add(current);
                }
            }

            Characters = new bool[255];
            foreach (char c in charSelection)
            {
                Characters[(byte)c] = true;
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static RawResource Process(CarbonFile file, FontProcessingOptions options)
        {
            if (options.Size <= 0 || options.CharactersPerRow <= 0 || !CarbonFile.FileExists(file))
            {
                throw new ArgumentException("Invalid Font Processing options");
            }

            using (var fontCollection = new PrivateFontCollection())
            {
                fontCollection.AddFontFile(file.ToString());
                using (var font = new Font(fontCollection.Families[0], options.Size, options.Style))
                {
                    using (Bitmap image = Draw(options, font))
                    {
                        using (var stream = new MemoryStream())
                        {
                            image.Save(stream, ImageFormat.Png);
                            stream.Position = 0;
                            var data = new byte[stream.Length];
                            stream.Read(data, 0, (int)stream.Length);
                            var resource = new RawResource { Data = data };
                            return resource;
                        }
                    }
                }
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static Point Measure(Font font)
        {
            var image = new Bitmap(1, 1);
            Graphics graphics = Graphics.FromImage(image);
            int width = 0;
            int height = 0;
            for (int i = 0; i < Characters.Length; i++)
            {
                if (!Characters[i])
                {
                    continue;
                }

                SizeF size = graphics.MeasureString(new string((char)i, 1), font);
                if (size.Width > width)
                {
                    width = (int)size.Width;
                }

                if (size.Height > height)
                {
                    height = (int)size.Height;
                }
            }

            return new Point(width, height);
        }

        private static Bitmap Draw(FontProcessingOptions options, Font font)
        {
            Point glyphSize = Measure(font);

            int rowCount = byte.MaxValue / options.CharactersPerRow;
            var image = new Bitmap(glyphSize.X * options.CharactersPerRow, glyphSize.Y * rowCount);
            var graphics = Graphics.FromImage(image);
            graphics.Clear(Color.Transparent);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            int rowCounter = 0;
            int x = 0;
            int y = 0;
            for (int i = 0; i < Characters.Length; i++)
            {
                string str = Characters[i] ? new string((char)i, 1) : string.Empty;

                graphics.DrawString(str, font, new SolidBrush(Color.White), x, y);
                x += glyphSize.X;
                rowCounter++;
                if (rowCounter >= options.CharactersPerRow)
                {
                    rowCounter = 0;
                    y += glyphSize.Y;
                    x = 0;
                }
            }

            graphics.Flush();
            return image;
        }
    }
}
