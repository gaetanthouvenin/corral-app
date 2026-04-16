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

  [Fact]
  public async Task GetAllAsync_ShouldReturnAllMappedFences()
  {
    await using var database = new SqliteInMemoryDatabase();

    await using (var seedContext = database.CreateDbContext())
    {
      seedContext.Fences.AddRange(
        new FenceEntity
        {
          Id = "fence-1",
          Name = "A",
          PositionX = 0,
          PositionY = 0,
          Width = 200,
          Height = 200,
          BackgroundColor = "#FFFFFFFF",
          Opacity = 100,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        },
        new FenceEntity
        {
          Id = "fence-2",
          Name = "B",
          PositionX = 10,
          PositionY = 10,
          Width = 300,
          Height = 300,
          BackgroundColor = "#FF00FF00",
          Opacity = 75,
          IsActive = false,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        }
      );

      await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using var readContext = database.CreateDbContext();
    var repository = new FenceRepository(readContext, _mapper);

    var fences = await repository.GetAllAsync(TestContext.Current.CancellationToken);

    fences.Count.ShouldBe(2);
    fences.Select(x => x.Name).Order().ShouldBe(["A", "B"]);
  }

  [Fact]
  public async Task SearchByNameAsync_ShouldReturnCaseInsensitiveMatches()
  {
    await using var database = new SqliteInMemoryDatabase();

    await using (var seedContext = database.CreateDbContext())
    {
      seedContext.Fences.AddRange(
        new FenceEntity
        {
          Id = "fence-1",
          Name = "Zone Dev",
          PositionX = 0,
          PositionY = 0,
          Width = 200,
          Height = 200,
          BackgroundColor = "#FFFFFFFF",
          Opacity = 100,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        },
        new FenceEntity
        {
          Id = "fence-2",
          Name = "Zone Jeux",
          PositionX = 10,
          PositionY = 10,
          Width = 200,
          Height = 200,
          BackgroundColor = "#FFFFFFFF",
          Opacity = 100,
          IsActive = true,
          CreatedAt = DateTime.UtcNow,
          UpdatedAt = DateTime.UtcNow
        }
      );

      await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using var readContext = database.CreateDbContext();
    var repository = new FenceRepository(readContext, _mapper);

    var fences = await repository.SearchByNameAsync("DEV", TestContext.Current.CancellationToken);

    fences.Count.ShouldBe(1);
    fences[0].Name.ShouldBe("Zone Dev");
  }

  [Fact]
  public async Task UpdateAsync_ShouldSynchronizeScalarPropertiesAndItems()
  {
    await using var database = new SqliteInMemoryDatabase();

    await using (var seedContext = database.CreateDbContext())
    {
      seedContext.Fences.Add(
        new FenceEntity
        {
          Id = "fence-1",
          Name = "Original",
          PositionX = 0,
          PositionY = 0,
          Width = 200,
          Height = 200,
          BackgroundColor = "#FFFFFFFF",
          Opacity = 100,
          IsActive = true,
          CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
          UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
          Items =
          [
            new FenceItemEntity
            {
              Id = "item-1",
              FenceId = "fence-1",
              DisplayName = "Old A",
              Path = "a.txt",
              ItemType = (int)FenceItemType.File,
              SortOrder = 0,
              CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new FenceItemEntity
            {
              Id = "item-2",
              FenceId = "fence-1",
              DisplayName = "Old B",
              Path = "b.txt",
              ItemType = (int)FenceItemType.File,
              SortOrder = 1,
              CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
          ]
        }
      );

      await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using (var updateContext = database.CreateDbContext())
    {
      var repository = new FenceRepository(updateContext, _mapper);
      var fence = await repository.GetByIdAsync(
                    FenceId.Create("fence-1"),
                    TestContext.Current.CancellationToken
                  );

      fence.ShouldNotBeNull();

      fence.Rename("Updated");
      fence.Move(Position.Create(50, 60));
      fence.ChangeColor(Color.Blue);
      fence.ChangeOpacity(Opacity.Create(40));
      fence.RemoveItem("item-1");
      var keptItem = fence.Items.Single(i => i.Id == "item-2");
      keptItem.Rename("Kept");
      keptItem.UpdateSortOrder(0);
      fence.AddItem("Added", "c.txt", FenceItemType.Shortcut);

      await repository.UpdateAsync(fence, TestContext.Current.CancellationToken);
      await updateContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using var readContext = database.CreateDbContext();
    var updatedFence = await readContext.Fences.Include(x => x.Items)
                                        .SingleAsync(
                                          x => x.Id == "fence-1",
                                          TestContext.Current.CancellationToken
                                        );

    updatedFence.Name.ShouldBe("Updated");
    updatedFence.PositionX.ShouldBe(50);
    updatedFence.PositionY.ShouldBe(60);
    updatedFence.BackgroundColor.ShouldBe(Color.Blue.ToHexString());
    updatedFence.Opacity.ShouldBe(40);
    updatedFence.Items.Count.ShouldBe(2);
    updatedFence.Items.Any(i => i.Id == "item-1").ShouldBeFalse();
    updatedFence.Items.Single(i => i.Id == "item-2").DisplayName.ShouldBe("Kept");
    updatedFence.Items.Single(i => i.DisplayName == "Added").SortOrder.ShouldBe(1);
  }

  [Fact]
  public async Task UpdateAsync_WhenFenceDoesNotExist_ShouldThrow()
  {
    await using var database = new SqliteInMemoryDatabase();
    await using var dbContext = database.CreateDbContext();
    var repository = new FenceRepository(dbContext, _mapper);

    var fence = FenceTestData.CreateFence();

    await Should.ThrowAsync<InvalidOperationException>(() => repository.UpdateAsync(
                                                         fence,
                                                         TestContext.Current.CancellationToken
                                                       )
    );
  }

  [Fact]
  public async Task Delete_ShouldRemoveFence()
  {
    await using var database = new SqliteInMemoryDatabase();

    Fence fence;
    await using (var seedContext = database.CreateDbContext())
    {
      fence = FenceTestData.CreateFence();
      var repository = new FenceRepository(seedContext, _mapper);
      repository.Add(fence);
      await seedContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using (var deleteContext = database.CreateDbContext())
    {
      var repository = new FenceRepository(deleteContext, _mapper);
      repository.Delete(fence);
      await deleteContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    await using var readContext = database.CreateDbContext();
    (await readContext.Fences.CountAsync(TestContext.Current.CancellationToken)).ShouldBe(0);
  }

  #endregion
}
