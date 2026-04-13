// ------------------------------------------------------------------------------------------------
// <copyright file="FenceRepositoryTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;
using Corral.Infrastructure.Mappers;
using Corral.Infrastructure.Persistence.Entities;
using Corral.Infrastructure.Repositories;
using Corral.Infrastructure.Tests.TestSupport;

using Microsoft.EntityFrameworkCore;

namespace Corral.Infrastructure.Tests.Repositories;

public class FenceRepositoryTests
{
  #region Fields

  private readonly FenceEntityToDomainMapper _mapper = new();

  #endregion

  #region Methods

  [Fact]
  public async Task Add_ShouldPersistFenceAndItems()
  {
    await using var database = new SqliteInMemoryDatabase();
    await using var dbContext = database.CreateDbContext();
    var repository = new FenceRepository(dbContext, _mapper);
    var fence = FenceTestData.CreateFence(items: FenceTestData.CreateItems());

    repository.Add(fence);
    await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

    var savedFence = await dbContext.Fences.SingleAsync(TestContext.Current.CancellationToken);
    savedFence.Name.ShouldBe(fence.Name);
    savedFence.BackgroundColor.ShouldBe(fence.BackgroundColor.ToHexString());
    savedFence.Opacity.ShouldBe(fence.Opacity.Percentage);

    var savedItems = await dbContext.FenceItems.OrderBy(i => i.SortOrder)
                                    .ToListAsync(TestContext.Current.CancellationToken);

    savedItems.Count.ShouldBe(2);
    savedItems.Select(i => i.DisplayName).ShouldBe(["Visual Studio", "Documentation"]);
  }

  [Fact]
  public async Task GetByIdAsync_ShouldReturnMappedFenceWithItems()
  {
    await using var database = new SqliteInMemoryDatabase();

    await using (var seedContext = database.CreateDbContext())
    {
      seedContext.Fences.Add(
        new FenceEntity
        {
          Id = "fence-1",
          Name = "Zone dev",
          PositionX = 15,
          PositionY = 30,
          Width = 400,
          Height = 250,
          BackgroundColor = "#FF0078D4",
          Opacity = 80,
          IsActive = true,
          CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
          UpdatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc),
          Items =
          [
            new FenceItemEntity
            {
              Id = "item-1",
              FenceId = "fence-1",
              DisplayName = "Visual Studio",
              Path = "C:/Apps/VisualStudio.lnk",
              ItemType = (int)FenceItemType.Shortcut,
              SortOrder = 0,
              CreatedAt = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc)
            }
          ]
        }
      );

      await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using var readContext = database.CreateDbContext();
    var repository = new FenceRepository(readContext, _mapper);

    var fence = await repository.GetByIdAsync(
                  FenceId.Create("fence-1"),
                  TestContext.Current.CancellationToken
                );

    fence.ShouldNotBeNull();
    fence.Name.ShouldBe("Zone dev");
    fence.Items.Count.ShouldBe(1);
    fence.Items[0].DisplayName.ShouldBe("Visual Studio");
  }

  [Fact]
  public async Task GetActivesAsync_ShouldReturnOnlyActiveFences()
  {
    await using var database = new SqliteInMemoryDatabase();

    await using (var seedContext = database.CreateDbContext())
    {
      seedContext.Fences.AddRange(
        new FenceEntity
        {
          Id = "fence-1",
          Name = "Active",
          PositionX = 0,
          PositionY = 0,
          Width = 200,
          Height = 200,
          BackgroundColor = "#FFFFFFFF",
          Opacity = 100,
          IsActive = true,
          CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
          UpdatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
        },
        new FenceEntity
        {
          Id = "fence-2",
          Name = "Inactive",
          PositionX = 10,
          PositionY = 10,
          Width = 200,
          Height = 200,
          BackgroundColor = "#FF00FF00",
          Opacity = 75,
          IsActive = false,
          CreatedAt = new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc),
          UpdatedAt = new DateTime(2025, 1, 4, 0, 0, 0, DateTimeKind.Utc)
        }
      );

      await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using var readContext = database.CreateDbContext();
    var repository = new FenceRepository(readContext, _mapper);

    var fences = await repository.GetActivesAsync(TestContext.Current.CancellationToken);

    fences.Count.ShouldBe(1);
    fences[0].Name.ShouldBe("Active");
  }

  #endregion
}
