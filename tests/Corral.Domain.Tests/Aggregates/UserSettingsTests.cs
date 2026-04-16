// ------------------------------------------------------------------------------------------------
// <copyright file="UserSettingsTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.Tests.Aggregates;

public class UserSettingsTests
{
  #region Methods

  [Fact]
  public void Create_ShouldReturnSingletonWithDefaultValues()
  {
    var before = DateTime.UtcNow;

    var settings = UserSettings.Create();

    var after = DateTime.UtcNow;
    settings.Id.ShouldBe(UserSettings.SingletonId);
    settings.ClickMode.ShouldBe(0);
    settings.IconLayout.ShouldBe(0);
    settings.UpdatedAt.ShouldBeGreaterThanOrEqualTo(before);
    settings.UpdatedAt.ShouldBeLessThanOrEqualTo(after);
  }

  [Fact]
  public void Reconstitute_ShouldRestorePersistedValues()
  {
    var updatedAt = new DateTime(2026, 4, 1, 12, 30, 0, DateTimeKind.Utc);

    var settings = UserSettings.Reconstitute(1, 1, 2, updatedAt);

    settings.Id.ShouldBe(1);
    settings.ClickMode.ShouldBe(1);
    settings.IconLayout.ShouldBe(2);
    settings.UpdatedAt.ShouldBe(updatedAt);
  }

  [Fact]
  public void Update_ShouldApplyValuesAndRefreshTimestamp()
  {
    var previousTimestamp = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc);
    var settings = UserSettings.Reconstitute(UserSettings.SingletonId, 0, 0, previousTimestamp);

    settings.Update(1, 2);

    settings.ClickMode.ShouldBe(1);
    settings.IconLayout.ShouldBe(2);
    settings.UpdatedAt.ShouldBeGreaterThan(previousTimestamp);
  }

  #endregion
}
