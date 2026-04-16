// ------------------------------------------------------------------------------------------------
// <copyright file="OverlayPreferences.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Desktop.Models;

/// <summary>
///   User preferences for overlay windows.
/// </summary>
public class OverlayPreferences
{
  #region Properties

  /// <summary>
  ///   Gets or sets the click mode for opening items.
  /// </summary>
  public ClickMode ClickMode { get; set; } = ClickMode.SingleClick;

  /// <summary>
  ///   Gets or sets the icon layout style.
  /// </summary>
  public IconLayout IconLayout { get; set; } = IconLayout.LargeGrid;

  #endregion
}
