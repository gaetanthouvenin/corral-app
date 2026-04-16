// ------------------------------------------------------------------------------------------------
// <copyright file="UserSettingsRepositoryTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Infrastructure.Persistence.Entities;
using Corral.Infrastructure.Repositories;
using Corral.Infrastructure.Tests.TestSupport;

using Microsoft.EntityFrameworkCore;

namespace Corral.Infrastructure.Tests.Repositories;

public class UserSettingsRepositoryTests
{
  #region Methods

  [Fact]
  public async Task GetAsync_WhenRowDoesNotExist_ShouldReturnDefaultSettings()
  {
    await using var database = new SqliteInMemoryDatabase();
    await using var dbContext = database.CreateDbContext();
    var repository = new UserSettingsRepository(dbContext);

    var settings = await repository.GetAsync(TestContext.Current.CancellationToken);

    settings.Id.ShouldBe(UserSettings.SingletonId);
    settings.ClickMode.ShouldBe(0);
    settings.IconLayout.ShouldBe(0);
  }

  [Fact]
  public async Task GetAsync_WhenRowExists_ShouldMapPersistedSettings()
  {
    await using var database = new SqliteInMemoryDatabase();

    await using (var seedContext = database.CreateDbContext())
    {
      seedContext.UserSettings.Add(
        new UserSettingsEntity
        {
          Id = UserSettings.SingletonId,
          ClickMode = 1,
          IconLayout = 2,
          UpdatedAt = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc)
        }
      );

      await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using var readContext = database.CreateDbContext();
    var repository = new UserSettingsRepository(readContext);

    var settings = await repository.GetAsync(TestContext.Current.CancellationToken);

    settings.ClickMode.ShouldBe(1);
    settings.IconLayout.ShouldBe(2);
  }

  [Fact]
  public async Task Upsert_WhenRowIsNotTracked_ShouldInsertNewEntity()
  {
    await using var database = new SqliteInMemoryDatabase();
    await using var dbContext = database.CreateDbContext();
    var repository = new UserSettingsRepository(dbContext);
    var settings = UserSettings.Reconstitute(
      UserSettings.SingletonId,
      1,
      2,
      new DateTime(2026, 4, 1, 11, 0, 0, DateTimeKind.Utc)
    );

    repository.Upsert(settings);
    await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var entity = await dbContext.UserSettings.SingleAsync(TestContext.Current.CancellationToken);
    entity.ClickMode.ShouldBe(1);
    entity.IconLayout.ShouldBe(2);
  }

  [Fact]
  public async Task Upsert_WhenRowIsTracked_ShouldUpdateTrackedEntity()
  {
    await using var database = new SqliteInMemoryDatabase();

    await using (var seedContext = database.CreateDbContext())
    {
      seedContext.UserSettings.Add(
        new UserSettingsEntity
        {
          Id = UserSettings.SingletonId,
          ClickMode = 0,
          IconLayout = 0,
          UpdatedAt = new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc)
        }
      );

      await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using var dbContext = database.CreateDbContext();
    var repository = new UserSettingsRepository(dbContext);
    await repository.GetAsync(TestContext.Current.CancellationToken);

    var settings = UserSettings.Reconstitute(
      UserSettings.SingletonId,
      1,
      2,
      new DateTime(2026, 4, 1, 12, 0, 0, DateTimeKind.Utc)
    );

    repository.Upsert(settings);
    await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var entity = await dbContext.UserSettings.SingleAsync(TestContext.Current.CancellationToken);
    entity.ClickMode.ShouldBe(1);
    entity.IconLayout.ShouldBe(2);
    entity.UpdatedAt.ShouldBe(new DateTime(2026, 4, 1, 12, 0, 0, DateTimeKind.Utc));
  }

  #endregion
}
