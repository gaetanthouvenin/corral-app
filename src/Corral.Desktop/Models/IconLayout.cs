// ------------------------------------------------------------------------------------------------
// <copyright file="IconLayout.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Desktop.Models;

/// <summary>
///   Defines the layout style for icons in overlays.
/// </summary>
public enum IconLayout
{
  /// <summary>
  ///   Large grid layout (48px icons).
  /// </summary>
  LargeGrid = 0,

  /// <summary>
  ///   Small grid layout (32px icons).
  /// </summary>
  SmallGrid = 1,

  /// <summary>
  ///   Vertical list layout (16px icons with names).
  /// </summary>
  List = 2
}
