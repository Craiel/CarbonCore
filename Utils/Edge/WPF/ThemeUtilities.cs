namespace CarbonCore.Utils.Edge.WPF
{
    using System.Windows.Media;

    using MahApps.Metro;

    public static class ThemeUtilities
    {
        public static Color GetBlackColor()
        {
            return ((SolidColorBrush)ThemeManager.GetAppTheme("BaseLight").Resources["BlackBrush"]).Color;
        }
    }
}
