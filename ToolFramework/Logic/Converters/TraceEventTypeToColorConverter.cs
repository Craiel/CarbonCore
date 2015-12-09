namespace CarbonCore.ToolFramework.Logic.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;

    using CarbonCore.Utils.Edge.WPF;

    [ValueConversion(typeof(TraceEventType), typeof(Color))]
    public class TraceEventTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Trace.Assert(value.GetType() == typeof(TraceEventType));

            switch ((TraceEventType)value)
            {
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    {
                        return Colors.Red;
                    }

                case TraceEventType.Warning:
                    {
                        return Colors.Orange;
                    }
            }

            return ThemeUtilities.GetBlackColor();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
