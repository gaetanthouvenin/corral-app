// ------------------------------------------------------------------------------------------------
// <copyright file="IUserPreferencesService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Desktop.Models;

namespace Corral.Desktop.Services;

/// <summary>
///   Provides access to the user's overlay preferences, persisted in the database.
/// </summary>
public interface IUserPreferencesService
{
  #region Methods

  /// <summary>Retrieves the current preferences from the database.</summary>
  Task<OverlayPreferences> GetPreferencesAsync();

  /// <summary>Saves the given preferences to the database.</summary>
  Task SavePreferencesAsync(OverlayPreferences preferences);

  #endregion
}
