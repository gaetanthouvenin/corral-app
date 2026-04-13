// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateFenceCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.UpdateFence;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Commands;

public class UpdateFenceCommandHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepoMock = new();
  private readonly UpdateFenceCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public UpdateFenceCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(u => u.Fences).Returns(_fenceRepoMock.Object);
    _handler = new UpdateFenceCommandHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

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

  [Fact]
  public async Task Handle_ValidCommand_ShouldUpdateFenceProperties()
  {
    var fence = CreateExistingFence();
    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fence);

    var command = new UpdateFenceCommand("fence-1", "Updated Name", "#FFFF0000", 50);

    var result = await _handler.Handle(command, CancellationToken.None);

    result.Name.ShouldBe("Updated Name");
    result.BackgroundColor.ShouldBe(Color.FromHexString("#FFFF0000"));
    result.Opacity.Percentage.ShouldBe(50);
  }

  [Fact]
  public async Task Handle_ValidCommand_ShouldCallUpdateAndSave()
  {
    var fence = CreateExistingFence();
    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync(fence);

    var command = new UpdateFenceCommand("fence-1", "Updated", "#FFFFFFFF", 50);

    await _handler.Handle(command, CancellationToken.None);

    _fenceRepoMock.Verify(
      r => r.UpdateAsync(It.IsAny<Fence>(), It.IsAny<CancellationToken>()),
      Times.Once
    );
    _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  public async Task Handle_FenceNotFound_ShouldThrowInvalidOperationException()
  {
    _fenceRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
                  .ReturnsAsync((Fence)null);

    var command = new UpdateFenceCommand("non-existent", "Name", "#FFFFFFFF", 50);

    await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
                                                         command,
                                                         CancellationToken.None
                                                       )
    );
  }

  #endregion
}
