using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using SolarSystemWiki.Models;

namespace SolarSystemWiki.Helpers;

/// <summary>bool → Visibility: true = Visible.</summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
        => value is true ? Visibility.Visible : Visibility.Collapsed;
    public object ConvertBack(object value, Type t, object p, string l)
        => value is Visibility.Visible;
}

/// <summary>
/// null → Visible, non-null → Collapsed (use Inverse=true to flip).
/// Handles WikiEntry? and any nullable object.
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    public bool Inverse { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool isNull = value is null;
        bool show   = Inverse ? !isNull : isNull;
        return show ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type t, object p, string l)
        => throw new NotImplementedException();
}

/// <summary>
/// Non-empty string → Visible (used for InfoBar.IsOpen via string).
/// Also returns bool when targetType is bool.
/// </summary>
public class EmptyStringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool hasContent = !string.IsNullOrEmpty(value as string);
        if (targetType == typeof(bool))
            return hasContent;
        return hasContent ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type t, object p, string l)
        => throw new NotImplementedException();
}

/// <summary>Non-empty string → Visible (for Summary section).</summary>
public class SummaryVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
        => string.IsNullOrWhiteSpace(value as string)
            ? Visibility.Collapsed
            : Visibility.Visible;

    public object ConvertBack(object value, Type t, object p, string l)
        => throw new NotImplementedException();
}

/// <summary>Hex string "#RRGGBB" → SolidColorBrush.</summary>
public class StringToSolidBrushConverter : IValueConverter
{
    public object Convert(object value, Type t, object p, string l)
    {
        try
        {
            if (value is string hex)
            {
                hex = hex.TrimStart('#');
                if (hex.Length == 6)
                {
                    var r = System.Convert.ToByte(hex[0..2], 16);
                    var g = System.Convert.ToByte(hex[2..4], 16);
                    var b = System.Convert.ToByte(hex[4..6], 16);
                    return new SolidColorBrush(Windows.UI.Color.FromArgb(255, r, g, b));
                }
            }
        }
        catch { /* fall through */ }

        return new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 120, 212));
    }

    public object ConvertBack(object value, Type t, object p, string l)
        => throw new NotImplementedException();
}
