using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Simplifier.TestManagement.Converters;

public class DateOnlyToDateTimeOffsetConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateOnly dateOnly)
        {;
            return dateOnly.ToDateTime(TimeOnly.MinValue);
        }
        
        return null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }
        
        return null;
    }
}
