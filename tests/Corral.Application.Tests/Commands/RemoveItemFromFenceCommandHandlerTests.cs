// ------------------------------------------------------------------------------------------------
// <copyright file="RemoveItemFromFenceCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.RemoveItemFromFence;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Commands;

public class RemoveItemFromFenceCommandHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepositoryMock = new();
  private readonly RemoveItemFromFenceCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public RemoveItemFromFenceCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(x => x.Fences).Returns(_fenceRepositoryMock.Object);
    _handler = new RemoveItemFromFenceCommandHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ShouldRemoveItemUpdateFenceAndSave()
  {
    var fence = CreateFence();
    var firstItem = fence.AddItem("A", "a.txt", FenceItemType.File);
    fence.AddItem("B", "b.txt", FenceItemType.File);
    _fenceRepositoryMock
      .Setup(x => x.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(fence);

    var result = await _handler.Handle(
                   new RemoveItemFromFenceCommand("fence-1", firstItem.Id),
                   CancellationToken.None
                 );

    result.Items.Count.ShouldBe(1);
    result.Items[0].DisplayName.ShouldBe("B");
    result.Items[0].SortOrder.ShouldBe(0);
    _fenceRepositoryMock.Verify(
      x => x.UpdateAsync(fence, It.IsAny<CancellationToken>()),
      Times.Once
    );

    _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task Handle_WhenFenceDoesNotExist_ShouldThrow()
  {
    _fenceRepositoryMock
      .Setup(x => x.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Fence)null);

    await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
                                                         new RemoveItemFromFenceCommand(
                                                           "missing",
                                                           "item-1"
                                                         ),
                                                         CancellationToken.None
                                                       )
    );
  }

  private static Fence CreateFence()
  {
    return Fence.Reconstitute(
      FenceId.Create("fence-1"),
      "Fence",
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
