using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace SongbookManager.Converters
{
    class ReverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(bool) || targetType == typeof(bool?))
            {
                bool boolValue = (bool)value;
                return !boolValue;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(bool) || targetType == typeof(bool?))
            {
                bool boolValue = (bool)value;
                return !boolValue;
            }
            else
            {
                return false;
            }
        }
    }
}
