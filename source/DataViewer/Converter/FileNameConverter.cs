using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace DataViewer.Converter;

public class FileNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue)
        {
            if (Path.IsPathRooted(stringValue))
            {
                return Path.GetFileName(stringValue);
            }
            else
            {
                return stringValue;
            }
        }

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}