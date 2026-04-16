// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateUserSettingsCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;

using MediatR;

namespace Corral.Application.Commands.UpdateUserSettings;

/// <summary>
///   Handles <see cref="UpdateUserSettingsCommand" />.
/// </summary>
public class UpdateUserSettingsCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<UpdateUserSettingsCommand, UserSettings>
{
  #region Implementation of IRequestHandler<UpdateUserSettingsCommand,UserSettings>

  public async Task<UserSettings> Handle(
    UpdateUserSettingsCommand request,
    CancellationToken cancellationToken)
  {
    var settings = await unitOfWork.UserSettings.GetAsync(cancellationToken);
    settings.Update(request.ClickMode, request.IconLayout);
    unitOfWork.UserSettings.Upsert(settings);
    await unitOfWork.SaveChangesAsync(cancellationToken);
    return settings;
  }

  #endregion
}
