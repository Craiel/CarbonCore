namespace CarbonCore.UtilsWPF
{
    using System;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public static class ImageExtension
    {
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
    }
}
