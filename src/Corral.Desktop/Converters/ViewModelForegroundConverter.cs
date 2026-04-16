// ------------------------------------------------------------------------------------------------
// <copyright file="ViewModelForegroundConverter.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using Corral.Desktop.ViewModels;

using Binding = System.Windows.Data.Binding;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;

namespace Corral.Desktop.Converters;

/// <summary>
///   Converts a ViewModel instance to a foreground brush color for active menu highlighting.
/// </summary>
[ValueConversion(typeof(object), typeof(Brush))]
public class ViewModelForegroundConverter : IValueConverter
{
  #region Fields

  private static readonly Brush InactiveBrush = new SolidColorBrush(Color.FromRgb(192, 192, 192));
  private static readonly Brush ActiveBrush = new SolidColorBrush(Color.FromRgb(0, 120, 212));

  #endregion

  #region Implementation of IValueConverter

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null || parameter == null)
    {
      return InactiveBrush;
    }

    var parameterString = parameter.ToString();
    var isActive = parameterString switch
    {
      "Zones" => value is ZonesViewModel,
      "Settings" => value is SettingsViewModel,
      var _ => false
    };

    return isActive ? ActiveBrush : InactiveBrush;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return Binding.DoNothing;
  }

  #endregion
}
