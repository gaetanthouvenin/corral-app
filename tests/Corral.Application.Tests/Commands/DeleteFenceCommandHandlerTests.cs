// ------------------------------------------------------------------------------------------------
// <copyright file="DeleteFenceCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.DeleteFence;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Commands;

public class DeleteFenceCommandHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepoMock = new();
  private readonly DeleteFenceCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public DeleteFenceCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(u => u.Fences).Returns(_fenceRepoMock.Object);
    _handler = new DeleteFenceCommandHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  private static Fence CreateExistingFence(string id = "fence-1")
  {
    return Fence.Reconstitute(
      FenceId.Create(id),
      "Test",
      Position.Create(0, 0),
      Dimensions.Create(200, 200),
      Color.White,
      Opacity.Opaque,
      true,
      DateTime.UtcNow,
      null
    );
  }

  [Fact]
  public async Task Handle_ValidCommand_ShouldDeleteFence()
  {
    var fence = CreateExistingFence();
    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fence);

    var command = new DeleteFenceCommand("fence-1");

    var result = await _handler.Handle(command, CancellationToken.None);

    result.ShouldNotBeNull();
    _fenceRepoMock.Verify(r => r.Delete(It.IsAny<Fence>()), Times.Once);
    _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task Handle_FenceNotFound_ShouldThrowInvalidOperationException()
  {
    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Fence)null);

    var command = new DeleteFenceCommand("non-existent");

    await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
                                                         command,
                                                         CancellationToken.None
                                                       )
    );
  }

  #endregion
}
