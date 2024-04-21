using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DataViewer.Converter;

public class BoolToVisibilityConverter : IValueConverter
{
    public Visibility True { get; set; } = Visibility.Visible;
    public Visibility False { get; set; } = Visibility.Collapsed;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? True : False;
        }
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility == True;
        }
        return DependencyProperty.UnsetValue;
    }
}