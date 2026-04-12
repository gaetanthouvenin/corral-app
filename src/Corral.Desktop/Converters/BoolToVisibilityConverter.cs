// ------------------------------------------------------------------------------------------------
// <copyright file="BoolToVisibilityConverter.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Corral.Desktop.Converters;

/// <summary>
///   Converter qui transforme un bool en Visibility. True → Visible, False → Collapsed.
///   Passer "Invert" comme ConverterParameter pour inverser le comportement.
/// </summary>
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolToVisibilityConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    bool boolValue = value is bool b && b;
    bool invert = parameter is string p && p == "Invert";
    return (boolValue ^ invert) ? Visibility.Visible : Visibility.Collapsed;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    => throw new NotSupportedException();
}
