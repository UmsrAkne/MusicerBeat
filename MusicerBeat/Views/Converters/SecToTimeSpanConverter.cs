using System;
using System.Globalization;
using System.Windows.Data;

namespace MusicerBeat.Views.Converters
{
    public class SecToTimeSpanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan.TotalSeconds.ToString("F0");  // 小数点以下は省略
            }

            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && double.TryParse(str, out var seconds))
            {
                return TimeSpan.FromSeconds(seconds);
            }

            return TimeSpan.Zero;  // 無効な値はデフォルトで 0秒に
        }
    }
}