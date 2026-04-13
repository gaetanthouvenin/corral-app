// ------------------------------------------------------------------------------------------------
// <copyright file="ITrayIconService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Desktop.Services;

/// <summary>
///   Service for managing the system tray (notification area) icon.
/// </summary>
public interface ITrayIconService : IDisposable
{
  #region Methods

  /// <summary>
  ///   Initializes and shows the tray icon.
  /// </summary>
  void Show();

  /// <summary>
  ///   Hides and removes the tray icon.
  /// </summary>
  void Hide();

  /// <summary>
  ///   Event raised when the user requests to show/restore the main window.
  /// </summary>
  event EventHandler ShowWindowRequested;

  /// <summary>
  ///   Event raised when the user requests to toggle overlays.
  /// </summary>
  event EventHandler ToggleOverlaysRequested;

  /// <summary>
  ///   Event raised when the user requests to exit the application.
  /// </summary>
  event EventHandler ExitRequested;

  #endregion
}
