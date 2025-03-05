using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicerBeat.Models
{
    public class IntToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = value != null ? (int)value : 0;
            return TimeSpan.FromMilliseconds(intValue).ToString("h':'mm':'ss");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}