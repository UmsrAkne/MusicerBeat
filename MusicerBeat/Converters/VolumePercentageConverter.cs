using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicerBeat.Converters
{
    public class VolumePercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }

            var param = (int)((float)value * 100);
            return param;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}