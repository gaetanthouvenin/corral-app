// ------------------------------------------------------------------------------------------------
// <copyright file="IIconService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Windows.Media;

namespace Corral.Desktop.Services;

/// <summary>
///   Service that extracts the Windows shell icon associated with a file, folder, shortcut,
///   or URL, and exposes it as a WPF <see cref="ImageSource" /> ready for UI binding.
/// </summary>
public interface IIconService
{
  #region Methods

  /// <summary>
  ///   Returns the shell icon for the specified path. For <c>.lnk</c> shortcuts the target's
  ///   icon is resolved automatically. URLs fall back to a generic link glyph. Results are
  ///   cached by <paramref name="path" /> for the lifetime of the service.
  /// </summary>
  /// <param name="path">Absolute file path, shortcut path, or URL.</param>
  /// <returns>An <see cref="ImageSource" /> that can be bound to an <c>Image</c> control, or <c>null</c>.</returns>
  ImageSource GetIcon(string path);

  #endregion
}
