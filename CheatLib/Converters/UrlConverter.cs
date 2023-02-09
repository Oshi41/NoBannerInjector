using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CheatLib.Converters;

public class UrlConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var str = value?.ToString();
        if (Uri.IsWellFormedUriString(str, UriKind.Absolute))
            return new Uri(str);

        return new Uri("about:blank");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() ?? "about:blank";
    }
}