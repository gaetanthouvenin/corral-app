// ------------------------------------------------------------------------------------------------
// <copyright file="IDialogService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
namespace Corral.Desktop.Services;

/// <summary>
///   Abstraction for WPF dialog management.
///   Enables dependency injection and testability of ViewModels.
/// </summary>
public interface IDialogService
{
  /// <summary>
  ///   Shows the create zone dialog.
  /// </summary>
  /// <returns>True if the user confirmed, false if cancelled.</returns>
  bool ShowCreateZoneDialog();

  /// <summary>
  ///   Shows the edit zone dialog.
  /// </summary>
  /// <param name="fenceId">The ID of the fence to edit.</param>
  /// <param name="name">The current name of the fence.</param>
  /// <param name="color">The current color in #AARRGGBB format.</param>
  /// <param name="opacity">The current opacity (0-100).</param>
  /// <returns>True if the user confirmed, false if cancelled.</returns>
  bool ShowEditZoneDialog(string fenceId, string name, string color, int opacity);
}
