namespace CarbonCore.UtilsWPF
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Image = System.Windows.Controls.Image;

    public static class ImageExtension
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Image ToImage(this Uri uri)
        {
            var image = new Image { Source = uri.ToImageSource() };
            return image;
        }

        public static ImageSource ToImageSource(this Uri uri)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = uri;
            bitmap.EndInit();
            return bitmap;
        }

        public static BitmapSource ToBitmapSource(this Bitmap source)
        {
            BitmapSource bitSrc;

            var bitmapHandle = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    bitmapHandle,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                NativeMethods.DeleteObject(bitmapHandle);
            }

            return bitSrc;
        }

        // -------------------------------------------------------------------
        // Internal
        // -------------------------------------------------------------------
        internal static class NativeMethods
        {
            [DllImport("gdi32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            internal static extern bool DeleteObject(IntPtr hObject);
        }
    }
}
