// ------------------------------------------------------------------------------------------------
// <copyright file="IconLayoutDataTemplateSelector.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

using Corral.Desktop.Models;
using Corral.Desktop.ViewModels;

namespace Corral.Desktop.Controls;

/// <summary>
///   Selects the appropriate DataTemplate based on the current IconLayout.
/// </summary>
public class IconLayoutDataTemplateSelector : DataTemplateSelector
{
  #region Properties

  /// <summary>
  ///   Gets or sets the template for LargeGrid layout (48px icons).
  /// </summary>
  public DataTemplate LargeGridTemplate { get; set; }

  /// <summary>
  ///   Gets or sets the template for SmallGrid layout (32px icons).
  /// </summary>
  public DataTemplate SmallGridTemplate { get; set; }

  /// <summary>
  ///   Gets or sets the template for List layout (16px icons with full names).
  /// </summary>
  public DataTemplate ListTemplate { get; set; }

  /// <summary>
  ///   Gets or sets the current icon layout being applied.
  /// </summary>
  public IconLayout CurrentLayout { get; set; }

  #endregion

  #region Methods

  /// <summary>
  ///   Selects the template based on the current layout.
  /// </summary>
  public override DataTemplate SelectTemplate(object item, DependencyObject container)
  {
    if (item is not FenceItemViewModel)
    {
      return base.SelectTemplate(item, container);
    }

    return CurrentLayout switch
    {
      IconLayout.LargeGrid => LargeGridTemplate,
      IconLayout.SmallGrid => SmallGridTemplate,
      IconLayout.List => ListTemplate,
      var _ => LargeGridTemplate
    };
  }

  #endregion
}
