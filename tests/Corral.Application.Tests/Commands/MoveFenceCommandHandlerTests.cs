// ------------------------------------------------------------------------------------------------
// <copyright file="MoveFenceCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.MoveFence;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Commands;

public class MoveFenceCommandHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepoMock = new();
  private readonly MoveFenceCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public MoveFenceCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(u => u.Fences).Returns(_fenceRepoMock.Object);
    _handler = new MoveFenceCommandHandler(_unitOfWorkMock.Object);
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
  public async Task Handle_ValidCommand_ShouldMoveFence()
  {
    var fence = CreateExistingFence();
    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fence);

    var command = new MoveFenceCommand("fence-1", 500, 300);

    var result = await _handler.Handle(command, CancellationToken.None);

    result.Position.X.ShouldBe(500);
    result.Position.Y.ShouldBe(300);
  }

  [Fact]
  public async Task Handle_FenceNotFound_ShouldThrowInvalidOperationException()
  {
    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Fence)null);

    var command = new MoveFenceCommand("non-existent", 100, 100);

    await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
                                                         command,
                                                         CancellationToken.None
                                                       )
    );
  }

  #endregion
}
