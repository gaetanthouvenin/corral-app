// ------------------------------------------------------------------------------------------------
// <copyright file="ResizeFenceCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.ResizeFence;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Commands;

public class ResizeFenceCommandHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepositoryMock = new();
  private readonly ResizeFenceCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public ResizeFenceCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(x => x.Fences).Returns(_fenceRepositoryMock.Object);
    _handler = new ResizeFenceCommandHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ShouldResizeFenceUpdateAndSave()
  {
    var fence = CreateFence();
    _fenceRepositoryMock
      .Setup(x => x.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(fence);

    var result = await _handler.Handle(
                   new ResizeFenceCommand("fence-1", 640, 480),
                   CancellationToken.None
                 );

    result.Dimensions.Width.ShouldBe(640);
    result.Dimensions.Height.ShouldBe(480);
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
                                                         new ResizeFenceCommand(
                                                           "missing",
                                                           640,
                                                           480
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
