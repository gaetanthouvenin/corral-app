// ------------------------------------------------------------------------------------------------
// <copyright file="FenceEntityToDomainMapperTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;
using Corral.Infrastructure.Mappers;
using Corral.Infrastructure.Persistence.Entities;

namespace Corral.Infrastructure.Tests.Mappers;

public class FenceEntityToDomainMapperTests
{
  #region Fields

  private readonly FenceEntityToDomainMapper _mapper = new();

  #endregion

  #region Methods

  [Fact]
  public void Map_ShouldMapFenceEntityWithItems()
  {
    var createdAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    var updatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc);
    var entity = new FenceEntity
    {
      Id = "fence-1",
      Name = "Zone dev",
      PositionX = 25,
      PositionY = 35,
      Width = 640,
      Height = 480,
      BackgroundColor = "#FF0078D4",
      Opacity = 60,
      IsActive = true,
      CreatedAt = createdAt,
      UpdatedAt = updatedAt,
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
    };

    var fence = _mapper.Map(entity);

    fence.Id.ShouldBe(FenceId.Create("fence-1"));
    fence.Name.ShouldBe("Zone dev");
    fence.Position.ShouldBe(Position.Create(25, 35));
    fence.Dimensions.ShouldBe(Dimensions.Create(640, 480));
    fence.BackgroundColor.ShouldBe(Color.FromHexString("#FF0078D4"));
    fence.Opacity.ShouldBe(Opacity.Create(60));
    fence.IsActive.ShouldBeTrue();
    fence.CreatedAt.ShouldBe(createdAt);
    fence.UpdatedAt.ShouldBe(updatedAt);
    fence.Items.Count.ShouldBe(1);
    fence.Items[0].DisplayName.ShouldBe("Visual Studio");
    fence.Items[0].Path.ShouldBe("C:/Apps/VisualStudio.lnk");
    fence.Items[0].ItemType.ShouldBe(FenceItemType.Shortcut);
  }

  [Fact]
  public void MapList_ShouldMapAllEntities()
  {
    var entities = new List<FenceEntity>
    {
      new()
      {
        Id = "fence-1",
        Name = "Zone A",
        PositionX = 0,
        PositionY = 0,
        Width = 100,
        Height = 100,
        BackgroundColor = "#FFFFFFFF",
        Opacity = 100,
        IsActive = true,
        CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
      },
      new()
      {
        Id = "fence-2",
        Name = "Zone B",
        PositionX = 50,
        PositionY = 50,
        Width = 200,
        Height = 150,
        BackgroundColor = "#FF00FF00",
        Opacity = 75,
        IsActive = false,
        CreatedAt = new DateTime(2025, 1, 5, 0, 0, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2025, 1, 6, 0, 0, 0, DateTimeKind.Utc)
      }
    };

    var fences = _mapper.MapList(entities);

    fences.Count.ShouldBe(2);
    fences.Select(f => f.Name).ShouldBe(["Zone A", "Zone B"]);
    fences.Select(f => f.IsActive).ShouldBe([true, false]);
  }

  #endregion
}
