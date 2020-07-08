using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace WCFClient
{
    public class ColorConverter : IValueConverter
    {
        public ColorConverter()
        {

        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(Style) && value is bool)
            {
                if ((bool)value)
                {
                    return Application.Current.Resources["TrueST"] as Style;
                }
                else
                {
                    return Application.Current.Resources["FalseST"] as Style;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
