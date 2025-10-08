using Microsoft.UI.Xaml.Data;

namespace DevTKSS.UnoWebView2App.Converters;

public class UriToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is Uri uri ? uri.ToString() : string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        var str = value as string;
        if (Uri.TryCreate(str, UriKind.Absolute, out var uri))
            return uri;
        return null;
    }
}

