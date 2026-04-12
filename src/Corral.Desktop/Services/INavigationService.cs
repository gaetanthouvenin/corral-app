// ------------------------------------------------------------------------------------------------
// <copyright file="INavigationService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Desktop.Services;

/// <summary>
///   Provides navigation services for the application.
/// </summary>
public interface INavigationService
{
  #region Methods

  /// <summary>
  ///   Navigates to the main window of the application.
  /// </summary>
  void NavigateToMainWindow();

  /// <summary>
  ///   Navigates to the settings window of the application.
  /// </summary>
  void NavigateToSettings();

  /// <summary>
  ///   Displays a dialog with the specified title and message.
  /// </summary>
  /// <param name="title">The title of the dialog.</param>
  /// <param name="message">The message to display in the dialog.</param>
  void ShowDialog(string title, string message);

  /// <summary>
  ///   Displays an error dialog with the specified title and message.
  /// </summary>
  /// <param name="title">The title of the error dialog.</param>
  /// <param name="message">The error message to display.</param>
  void ShowError(string title, string message);

  #endregion
}
