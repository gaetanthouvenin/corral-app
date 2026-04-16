// ------------------------------------------------------------------------------------------------
// <copyright file="GetUserSettingsQueryHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;

using MediatR;

namespace Corral.Application.Queries.GetUserSettings;

/// <summary>
///   Handles <see cref="GetUserSettingsQuery" />.
///   Returns the singleton settings row, creating it with defaults if it does not yet exist.
/// </summary>
public class GetUserSettingsQueryHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<GetUserSettingsQuery, UserSettings>
{
  #region Implementation of IRequestHandler<GetUserSettingsQuery,UserSettings>

  public async Task<UserSettings> Handle(
    GetUserSettingsQuery request,
    CancellationToken cancellationToken)
  {
    return await unitOfWork.UserSettings.GetAsync(cancellationToken);
  }

  #endregion
}
