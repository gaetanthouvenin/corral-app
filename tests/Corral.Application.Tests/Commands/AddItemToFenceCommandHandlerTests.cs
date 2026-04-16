// ------------------------------------------------------------------------------------------------
// <copyright file="AddItemToFenceCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.AddItemToFence;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Commands;

public class AddItemToFenceCommandHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepositoryMock = new();
  private readonly AddItemToFenceCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public AddItemToFenceCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(x => x.Fences).Returns(_fenceRepositoryMock.Object);
    _handler = new AddItemToFenceCommandHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ShouldAddItemUpdateFenceAndSave()
  {
    var fence = CreateFence();
    _fenceRepositoryMock
      .Setup(x => x.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(fence);

    var result = await _handler.Handle(
                   new AddItemToFenceCommand("fence-1", "Docs", "https://example.com", 2),
                   CancellationToken.None
                 );

    result.Items.Count.ShouldBe(1);
    result.Items[0].DisplayName.ShouldBe("Docs");
    result.Items[0].ItemType.ShouldBe(FenceItemType.Link);
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

    var exception = await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
                        new AddItemToFenceCommand("missing", "Docs", "https://example.com", 2),
                        CancellationToken.None
                      )
                    );

    exception.Message.ShouldContain("missing");
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
