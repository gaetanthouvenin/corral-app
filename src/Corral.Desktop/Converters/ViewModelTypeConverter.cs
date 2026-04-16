// ------------------------------------------------------------------------------------------------
// <copyright file="ViewModelTypeConverter.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;

using Corral.Desktop.ViewModels;

using Binding = System.Windows.Data.Binding;

namespace Corral.Desktop.Converters;

/// <summary>
///   Converts a ViewModel instance to a string representing its type for UI logic.
/// </summary>
[ValueConversion(typeof(object), typeof(bool))]
public class ViewModelTypeConverter : IValueConverter
{
  #region Implementation of IValueConverter

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null || parameter == null)
    {
      return false;
    }

    var parameterString = parameter.ToString();

    return parameterString switch
    {
      "Zones" => value is ZonesViewModel,
      "Settings" => value is SettingsViewModel,
      var _ => false
    };
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    return Binding.DoNothing;
  }

  #endregion
}
