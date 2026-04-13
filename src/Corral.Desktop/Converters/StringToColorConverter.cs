// ------------------------------------------------------------------------------------------------
// <copyright file="StringToColorConverter.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace Corral.Desktop.Converters;

/// <summary>
///   Converter that transforms a hexadecimal color string to a WPF SolidColorBrush.
/// </summary>
public class StringToColorConverter : IValueConverter
{
  #region Implementation of IValueConverter

  /// <summary>
  ///   Converts a hex color string (e.g., "#FF0000" or "FF0000") to a SolidColorBrush.
  /// </summary>
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is not string colorString || string.IsNullOrWhiteSpace(colorString))
    {
      return new SolidColorBrush(Colors.Gray);
    }

    try
    {
      // Ensure the string starts with # for BrushConverter
      var colorHex = colorString.StartsWith("#") ? colorString : $"#{colorString}";

      // Use WPF's built-in color parsing
      var color = (Color)ColorConverter.ConvertFromString(colorHex);
      return new SolidColorBrush(color);
    }
    catch
    {
      return new SolidColorBrush(Colors.Gray);
    }
  }

  /// <summary>
  ///   Converts back is not supported for this converter.
  /// </summary>
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    throw new NotSupportedException();
  }

  #endregion
}
