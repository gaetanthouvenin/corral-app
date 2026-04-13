// ------------------------------------------------------------------------------------------------
// <copyright file="FenceItemType.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Domain.ValueObjects;

/// <summary>
///   Type of item contained within a Fence.
/// </summary>
public enum FenceItemType
{
  /// <summary>
  ///   Shortcut to a file or application.
  /// </summary>
  Shortcut = 0,

  /// <summary>
  ///   File or folder.
  /// </summary>
  File = 1,

  /// <summary>
  ///   URL link.
  /// </summary>
  Link = 2
}
