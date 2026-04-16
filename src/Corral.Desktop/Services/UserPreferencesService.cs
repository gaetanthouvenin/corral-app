// ------------------------------------------------------------------------------------------------
// <copyright file="UserPreferencesService.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Commands.UpdateUserSettings;
using Corral.Application.Queries.GetUserSettings;
using Corral.Desktop.Models;

using MediatR;

namespace Corral.Desktop.Services;

/// <summary>
///   Persists user preferences in the database via MediatR.
/// </summary>
public class UserPreferencesService(IMediator mediator) : IUserPreferencesService
{
  #region Implementation of IUserPreferencesService

  /// <inheritdoc />
  public async Task<OverlayPreferences> GetPreferencesAsync()
  {
    var settings = await mediator.Send(new GetUserSettingsQuery());
    return new OverlayPreferences
    {
      ClickMode = (ClickMode)settings.ClickMode, IconLayout = (IconLayout)settings.IconLayout
    };
  }

  /// <inheritdoc />
  public async Task SavePreferencesAsync(OverlayPreferences preferences)
  {
    var command = new UpdateUserSettingsCommand(
      (int)preferences.ClickMode,
      (int)preferences.IconLayout
    );

    await mediator.Send(command);
  }

  #endregion
}
