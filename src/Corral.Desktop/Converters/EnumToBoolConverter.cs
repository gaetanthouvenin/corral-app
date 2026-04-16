// ------------------------------------------------------------------------------------------------
// <copyright file="EnumToBoolConverter.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Globalization;
using System.Windows.Data;

using Binding = System.Windows.Data.Binding;

namespace Corral.Desktop.Converters;

/// <summary>
///   Converts enum values to boolean for RadioButton binding.
/// </summary>
[ValueConversion(typeof(Enum), typeof(bool))]
public class EnumToBoolConverter : IValueConverter
{
  #region Implementation of IValueConverter

  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value == null || parameter == null)
    {
      return false;
    }

    var enumValue = (int)value;
    var paramValue = int.Parse(parameter.ToString());

    return enumValue == paramValue;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is not bool isChecked || !isChecked || parameter == null)
    {
      return Binding.DoNothing;
    }

    return Enum.ToObject(targetType, int.Parse(parameter.ToString()));
  }

  #endregion
}
