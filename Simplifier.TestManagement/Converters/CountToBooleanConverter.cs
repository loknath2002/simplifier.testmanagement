using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Simplifier.TestManagement.Converters;

public class CountToBooleanConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            bool isEmpty = count == 0;
            if (parameter?.ToString() == "invert")
            {
                return !isEmpty;
            }
            return isEmpty;
        }
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value;
    }
}
