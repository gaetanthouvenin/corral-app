// ------------------------------------------------------------------------------------------------
// <copyright file="ReorderFenceItemsCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.ReorderFenceItems;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Commands;

public class ReorderFenceItemsCommandHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepoMock = new();
  private readonly ReorderFenceItemsCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public ReorderFenceItemsCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(u => u.Fences).Returns(_fenceRepoMock.Object);
    _handler = new ReorderFenceItemsCommandHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ValidCommand_ShouldReorderFenceItems()
  {
    var fence = CreateExistingFence();
    var firstItem = fence.AddItem("A", "a.txt", FenceItemType.File);
    var secondItem = fence.AddItem("B", "b.txt", FenceItemType.File);
    var thirdItem = fence.AddItem("C", "c.txt", FenceItemType.File);

    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fence);

    var command = new ReorderFenceItemsCommand("fence-1", thirdItem.Id, secondItem.Id);

    var result = await _handler.Handle(command, CancellationToken.None);

    result.Items[0].Id.ShouldBe(firstItem.Id);
    result.Items[1].Id.ShouldBe(thirdItem.Id);
    result.Items[2].Id.ShouldBe(secondItem.Id);
    result.Items[1].SortOrder.ShouldBe(1);
    result.Items[2].SortOrder.ShouldBe(2);
  }

  [Fact]
  public async Task Handle_ValidCommand_ShouldCallUpdateAndSave()
  {
    var fence = CreateExistingFence();
    var firstItem = fence.AddItem("A", "a.txt", FenceItemType.File);
    var secondItem = fence.AddItem("B", "b.txt", FenceItemType.File);

    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fence);

    var command = new ReorderFenceItemsCommand("fence-1", firstItem.Id, secondItem.Id);

    await _handler.Handle(command, CancellationToken.None);

    _fenceRepoMock.Verify(
      r => r.UpdateAsync(It.IsAny<Fence>(), It.IsAny<CancellationToken>()),
      Times.Once
    );

    _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  private static Fence CreateExistingFence(string id = "fence-1")
  {
    return Fence.Reconstitute(
      FenceId.Create(id),
      "Original",
      Position.Create(0, 0),
      Dimensions.Create(200, 200),
      Color.White,
      Opacity.Opaque,
      true,
      DateTime.UtcNow,
      null
    );
  }

  #endregion
}
