// ------------------------------------------------------------------------------------------------
// <copyright file="FenceTestData.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;

namespace Corral.Infrastructure.Tests.TestSupport;

public static class FenceTestData
{
  #region Methods

  public static Fence CreateFence(
    string id = "fence-1",
    string name = "Fence de test",
    bool isActive = true,
    int positionX = 10,
    int positionY = 20,
    int width = 300,
    int height = 200,
    string backgroundColor = "#FF0078D4",
    int opacity = 75,
    DateTime? createdAt = null,
    DateTime? updatedAt = null,
    IEnumerable<FenceItem> items = null)
  {
    var fence = Fence.Reconstitute(
      FenceId.Create(id),
      name,
      Position.Create(positionX, positionY),
      Dimensions.Create(width, height),
      Color.FromHexString(backgroundColor),
      Opacity.Create(opacity),
      isActive,
      createdAt ?? new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
      updatedAt ?? new DateTime(2025, 1, 2, 0, 0, 0, DateTimeKind.Utc)
    );

    if (items != null)
    {
      fence.LoadItems(items);
    }

    return fence;
  }

  public static List<FenceItem> CreateItems()
  {
    return
    [
      FenceItem.Reconstitute(
        "item-1",
        "Visual Studio",
        "C:/Apps/VisualStudio.lnk",
        FenceItemType.Shortcut,
        0,
        new DateTime(2025, 1, 3, 0, 0, 0, DateTimeKind.Utc)
      ),
      FenceItem.Reconstitute(
        "item-2",
        "Documentation",
        "https://learn.microsoft.com",
        FenceItemType.Link,
        1,
        new DateTime(2025, 1, 4, 0, 0, 0, DateTimeKind.Utc)
      )
    ];
  }

  #endregion
}
