// ------------------------------------------------------------------------------------------------
// <copyright file="FenceItemTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Tests.Aggregates;

public class FenceItemTests
{
  #region Methods

  [Fact]
  public void Create_ShouldInitializeItem()
  {
    var before = DateTime.UtcNow;

    var item = FenceItem.Create("Docs", "https://example.com", FenceItemType.Link, 3);

    var after = DateTime.UtcNow;
    item.Id.ShouldNotBeNullOrWhiteSpace();
    item.DisplayName.ShouldBe("Docs");
    item.Path.ShouldBe("https://example.com");
    item.ItemType.ShouldBe(FenceItemType.Link);
    item.SortOrder.ShouldBe(3);
    item.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
    item.CreatedAt.ShouldBeLessThanOrEqualTo(after);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Create_InvalidDisplayName_ShouldThrow(string displayName)
  {
    Should.Throw<ArgumentException>(() => FenceItem.Create(
                                      displayName,
                                      "C:/apps/tool.exe",
                                      FenceItemType.File
                                    )
    );
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Create_InvalidPath_ShouldThrow(string path)
  {
    Should.Throw<ArgumentException>(() => FenceItem.Create("Tool", path, FenceItemType.File));
  }

  [Fact]
  public void Reconstitute_ShouldRestorePersistedValues()
  {
    var createdAt = new DateTime(2026, 4, 1, 8, 0, 0, DateTimeKind.Utc);

    var item = FenceItem.Reconstitute(
      "item-1",
      "Explorer",
      "C:/Windows/explorer.exe",
      FenceItemType.Shortcut,
      2,
      createdAt
    );

    item.Id.ShouldBe("item-1");
    item.DisplayName.ShouldBe("Explorer");
    item.Path.ShouldBe("C:/Windows/explorer.exe");
    item.ItemType.ShouldBe(FenceItemType.Shortcut);
    item.SortOrder.ShouldBe(2);
    item.CreatedAt.ShouldBe(createdAt);
  }

  [Fact]
  public void Rename_ShouldUpdateDisplayNameAndReturnSameInstance()
  {
    var item = FenceItem.Create("Old", "C:/apps/tool.exe", FenceItemType.File);

    var result = item.Rename("New");

    result.ShouldBeSameAs(item);
    item.DisplayName.ShouldBe("New");
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Rename_InvalidDisplayName_ShouldThrow(string displayName)
  {
    var item = FenceItem.Create("Old", "C:/apps/tool.exe", FenceItemType.File);

    Should.Throw<ArgumentException>(() => item.Rename(displayName));
  }

  [Fact]
  public void UpdateSortOrder_ShouldUpdateValueAndReturnSameInstance()
  {
    var item = FenceItem.Create("Tool", "C:/apps/tool.exe", FenceItemType.File, 1);

    var result = item.UpdateSortOrder(5);

    result.ShouldBeSameAs(item);
    item.SortOrder.ShouldBe(5);
  }

  #endregion
}
