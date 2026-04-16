// ------------------------------------------------------------------------------------------------
// <copyright file="FenceTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.DomainEvents;
using Corral.Domain.ValueObjects;

namespace Corral.Domain.Tests.Aggregates;

public class FenceTests
{
  #region Methods

  private static Fence CreateValidFence(
    string name = "Test Fence",
    int x = 100,
    int y = 100,
    int width = 200,
    int height = 200,
    string hexColor = "#FF0078D4",
    int opacity = 75)
  {
    return Fence.Create(
      name,
      Position.Create(x, y),
      Dimensions.Create(width, height),
      Color.FromHexString(hexColor),
      Opacity.Create(opacity)
    );
  }

  #region Reconstitute

  [Fact]
  public void Reconstitute_ShouldRestoreFenceWithoutEvents()
  {
    var id = FenceId.Create("reconstituted-id");
    var createdAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    var fence = Fence.Reconstitute(
      id,
      "Restored",
      Position.Create(50, 50),
      Dimensions.Create(300, 300),
      Color.Red,
      Opacity.Opaque,
      true,
      createdAt,
      null
    );

    fence.Id.ShouldBe(id);
    fence.Name.ShouldBe("Restored");
    fence.IsActive.ShouldBeTrue();
    fence.GetDomainEvents().Count.ShouldBe(0);
  }

  #endregion

  #region Delete

  [Fact]
  public void Delete_ShouldRaiseFenceDeletedEvent()
  {
    var fence = CreateValidFence();
    fence.ClearDomainEvents();

    fence.Delete();

    var events = fence.GetDomainEvents();
    events.Count.ShouldBe(1);
    events[0].ShouldBeOfType<FenceDeletedEvent>();
  }

  #endregion

  #region ClearDomainEvents

  [Fact]
  public void ClearDomainEvents_ShouldRemoveAllEvents()
  {
    var fence = CreateValidFence();
    fence.GetDomainEvents().Count.ShouldBeGreaterThan(0);

    fence.ClearDomainEvents();

    fence.GetDomainEvents().Count.ShouldBe(0);
  }

  #endregion

  #endregion

  #region Create

  [Fact]
  public void Create_ValidParameters_ShouldReturnFence()
  {
    var fence = CreateValidFence();

    fence.Name.ShouldBe("Test Fence");
    fence.Position.ShouldBe(Position.Create(100, 100));
    fence.Dimensions.ShouldBe(Dimensions.Create(200, 200));
    fence.BackgroundColor.ShouldBe(Color.FromHexString("#FF0078D4"));
    fence.Opacity.ShouldBe(Opacity.Create(75));
    fence.IsActive.ShouldBeTrue();
    fence.Id.Value.ShouldNotBeNullOrWhiteSpace();
  }

  [Fact]
  public void Create_ShouldSetCreatedAt()
  {
    var before = DateTime.UtcNow;
    var fence = CreateValidFence();
    var after = DateTime.UtcNow;

    fence.CreatedAt.ShouldBeGreaterThanOrEqualTo(before);
    fence.CreatedAt.ShouldBeLessThanOrEqualTo(after);
  }

  [Fact]
  public void Create_ShouldRaiseFenceCreatedEvent()
  {
    var fence = CreateValidFence();

    var events = fence.GetDomainEvents();
    events.Count.ShouldBe(1);
    events[0].ShouldBeOfType<FenceCreatedEvent>();
    var createdEvent = (FenceCreatedEvent)events[0];
    createdEvent.FenceId.ShouldBe(fence.Id);
    createdEvent.Name.ShouldBe("Test Fence");
  }

  [Fact]
  public void Create_NullOpacity_ShouldDefaultToSemiTransparent()
  {
    var fence = Fence.Create(
      "Test",
      Position.Create(0, 0),
      Dimensions.Create(200, 200),
      Color.White
    );

    fence.Opacity.ShouldBe(Opacity.SemiTransparent);
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Create_EmptyName_ShouldThrowArgumentException(string name)
  {
    Should.Throw<ArgumentException>(() => Fence.Create(
                                      name,
                                      Position.Create(0, 0),
                                      Dimensions.Create(200, 200),
                                      Color.White
                                    )
    );
  }

  [Fact]
  public void Create_NullPosition_ShouldThrowArgumentNullException()
  {
    Should.Throw<ArgumentNullException>(() => Fence.Create(
                                          "Test",
                                          null,
                                          Dimensions.Create(200, 200),
                                          Color.White
                                        )
    );
  }

  [Fact]
  public void Create_NullDimensions_ShouldThrowArgumentNullException()
  {
    Should.Throw<ArgumentNullException>(() => Fence.Create(
                                          "Test",
                                          Position.Create(0, 0),
                                          null,
                                          Color.White
                                        )
    );
  }

  [Fact]
  public void Create_NullColor_ShouldThrowArgumentNullException()
  {
    Should.Throw<ArgumentNullException>(() => Fence.Create(
                                          "Test",
                                          Position.Create(0, 0),
                                          Dimensions.Create(200, 200),
                                          null
                                        )
    );
  }

  #endregion

  #region Move

  [Fact]
  public void Move_ShouldUpdatePosition()
  {
    var fence = CreateValidFence();
    var newPosition = Position.Create(500, 600);

    fence.Move(newPosition);

    fence.Position.ShouldBe(newPosition);
  }

  [Fact]
  public void Move_ShouldSetUpdatedAt()
  {
    var fence = CreateValidFence();

    fence.Move(Position.Create(500, 600));

    fence.UpdatedAt.ShouldNotBeNull();
  }

  [Fact]
  public void Move_ShouldRaiseFencePositionChangedEvent()
  {
    var fence = CreateValidFence();
    fence.ClearDomainEvents();
    var newPosition = Position.Create(500, 600);

    fence.Move(newPosition);

    var events = fence.GetDomainEvents();
    events.Count.ShouldBe(1);
    events[0].ShouldBeOfType<FencePositionChangedEvent>();
  }

  [Fact]
  public void Move_NullPosition_ShouldThrowArgumentNullException()
  {
    var fence = CreateValidFence();

    Should.Throw<ArgumentNullException>(() => fence.Move(null));
  }

  #endregion

  #region ChangeColor

  [Fact]
  public void ChangeColor_ShouldUpdateColor()
  {
    var fence = CreateValidFence();
    var newColor = Color.Red;

    fence.ChangeColor(newColor);

    fence.BackgroundColor.ShouldBe(Color.Red);
  }

  [Fact]
  public void ChangeColor_ShouldRaiseFenceColorChangedEvent()
  {
    var fence = CreateValidFence();
    fence.ClearDomainEvents();

    fence.ChangeColor(Color.Blue);

    var events = fence.GetDomainEvents();
    events.Count.ShouldBe(1);
    events[0].ShouldBeOfType<FenceColorChangedEvent>();
  }

  [Fact]
  public void ChangeColor_NullColor_ShouldThrowArgumentNullException()
  {
    var fence = CreateValidFence();

    Should.Throw<ArgumentNullException>(() => fence.ChangeColor(null));
  }

  #endregion

  #region ChangeOpacity

  [Fact]
  public void ChangeOpacity_ShouldUpdateOpacity()
  {
    var fence = CreateValidFence();
    var newOpacity = Opacity.Create(25);

    fence.ChangeOpacity(newOpacity);

    fence.Opacity.ShouldBe(Opacity.Create(25));
  }

  [Fact]
  public void ChangeOpacity_NullOpacity_ShouldThrowArgumentNullException()
  {
    var fence = CreateValidFence();

    Should.Throw<ArgumentNullException>(() => fence.ChangeOpacity(null));
  }

  #endregion

  #region Resize

  [Fact]
  public void Resize_ShouldUpdateDimensions()
  {
    var fence = CreateValidFence();
    var newDimensions = Dimensions.Create(500, 400);

    fence.Resize(newDimensions);

    fence.Dimensions.ShouldBe(Dimensions.Create(500, 400));
  }

  [Fact]
  public void Resize_NullDimensions_ShouldThrowArgumentNullException()
  {
    var fence = CreateValidFence();

    Should.Throw<ArgumentNullException>(() => fence.Resize(null));
  }

  [Fact]
  public void ReorderItems_ShouldMoveItemBeforeTargetAndUpdateSortOrder()
  {
    var fence = CreateValidFence();
    var firstItem = fence.AddItem("A", "a.txt", FenceItemType.File);
    var secondItem = fence.AddItem("B", "b.txt", FenceItemType.File);
    var thirdItem = fence.AddItem("C", "c.txt", FenceItemType.File);

    fence.ReorderItems(thirdItem.Id, secondItem.Id);

    fence.Items[0].Id.ShouldBe(firstItem.Id);
    fence.Items[1].Id.ShouldBe(thirdItem.Id);
    fence.Items[2].Id.ShouldBe(secondItem.Id);
    fence.Items[0].SortOrder.ShouldBe(0);
    fence.Items[1].SortOrder.ShouldBe(1);
    fence.Items[2].SortOrder.ShouldBe(2);
  }

  [Fact]
  public void ReorderItems_EmptyTargetItemId_ShouldMoveItemToEnd()
  {
    var fence = CreateValidFence();
    var firstItem = fence.AddItem("A", "a.txt", FenceItemType.File);
    var secondItem = fence.AddItem("B", "b.txt", FenceItemType.File);
    var thirdItem = fence.AddItem("C", "c.txt", FenceItemType.File);

    fence.ReorderItems(firstItem.Id, string.Empty);

    fence.Items[0].Id.ShouldBe(secondItem.Id);
    fence.Items[1].Id.ShouldBe(thirdItem.Id);
    fence.Items[2].Id.ShouldBe(firstItem.Id);
  }

  #endregion

  #region Rename

  [Fact]
  public void Rename_ValidName_ShouldUpdateName()
  {
    var fence = CreateValidFence();

    fence.Rename("New Name");

    fence.Name.ShouldBe("New Name");
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData("   ")]
  public void Rename_InvalidName_ShouldThrowArgumentException(string name)
  {
    var fence = CreateValidFence();

    Should.Throw<ArgumentException>(() => fence.Rename(name));
  }

  #endregion

  #region Activate / Deactivate

  [Fact]
  public void Deactivate_ActiveFence_ShouldSetIsActiveFalse()
  {
    var fence = CreateValidFence();

    fence.Deactivate();

    fence.IsActive.ShouldBeFalse();
  }

  [Fact]
  public void Deactivate_ActiveFence_ShouldRaiseFenceDeactivatedEvent()
  {
    var fence = CreateValidFence();
    fence.ClearDomainEvents();

    fence.Deactivate();

    var events = fence.GetDomainEvents();
    events.Count.ShouldBe(1);
    events[0].ShouldBeOfType<FenceDeactivatedEvent>();
  }

  [Fact]
  public void Deactivate_AlreadyInactive_ShouldNotRaiseEvent()
  {
    var fence = CreateValidFence();
    fence.Deactivate();
    fence.ClearDomainEvents();

    fence.Deactivate();

    fence.GetDomainEvents().Count.ShouldBe(0);
  }

  [Fact]
  public void Activate_InactiveFence_ShouldSetIsActiveTrue()
  {
    var fence = CreateValidFence();
    fence.Deactivate();

    fence.Activate();

    fence.IsActive.ShouldBeTrue();
  }

  [Fact]
  public void Activate_AlreadyActive_ShouldNotChangeState()
  {
    var fence = CreateValidFence();
    var updatedAt = fence.UpdatedAt;

    fence.Activate();

    fence.UpdatedAt.ShouldBe(updatedAt);
  }

  #endregion
}
