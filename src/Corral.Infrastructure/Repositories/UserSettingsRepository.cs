// ------------------------------------------------------------------------------------------------
// <copyright file="UserSettingsRepository.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Infrastructure.Persistence;
using Corral.Infrastructure.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Corral.Infrastructure.Repositories;

/// <summary>
///   EF Core implementation of <see cref="IUserSettingsRepository" />.
/// </summary>
public class UserSettingsRepository(CorralDbContext dbContext) : IUserSettingsRepository
{
  #region Implementation of IUserSettingsRepository

  /// <inheritdoc />
  public async Task<UserSettings> GetAsync(CancellationToken cancellationToken = default)
  {
    var entity = await dbContext.UserSettings.FirstOrDefaultAsync(cancellationToken);

    if (entity == null)
    {
      return UserSettings.Create();
    }

    return UserSettings.Reconstitute(
      entity.Id,
      entity.ClickMode,
      entity.IconLayout,
      entity.UpdatedAt
    );
  }

  /// <inheritdoc />
  public void Upsert(UserSettings settings)
  {
    // If GetAsync already loaded this row, the EF entity is still tracked in Local.
    // In that case: just mutate its properties — EF will detect the change and emit UPDATE.
    // Otherwise the row doesn't exist yet in the DB → emit INSERT with Add().
    var tracked = dbContext.UserSettings.Local.FirstOrDefault(e => e.Id == settings.Id);

    if (tracked != null)
    {
      tracked.ClickMode = settings.ClickMode;
      tracked.IconLayout = settings.IconLayout;
      tracked.UpdatedAt = settings.UpdatedAt;
    }
    else
    {
      dbContext.UserSettings.Add(
        new UserSettingsEntity
        {
          Id = settings.Id,
          ClickMode = settings.ClickMode,
          IconLayout = settings.IconLayout,
          UpdatedAt = settings.UpdatedAt
        }
      );
    }
  }

  #endregion
}
