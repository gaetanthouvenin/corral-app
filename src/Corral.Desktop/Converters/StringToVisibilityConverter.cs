// ------------------------------------------------------------------------------------------------
// <copyright file="StringToVisibilityConverter.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Corral.Desktop.Converters;

/// <summary>
///   Converter qui transforme une chaîne non-vide en Visible, et une chaîne vide/nulle en Collapsed.
/// </summary>
[ValueConversion(typeof(string), typeof(Visibility))]
public class StringToVisibilityConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return value is string str && !string.IsNullOrWhiteSpace(str)
      ? Visibility.Visible
      : Visibility.Collapsed;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    => throw new NotSupportedException();
}
