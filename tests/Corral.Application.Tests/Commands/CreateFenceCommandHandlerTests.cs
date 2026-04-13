// ------------------------------------------------------------------------------------------------
// <copyright file="CreateFenceCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.CreateFence;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;

namespace Corral.Application.Tests.Commands;

public class CreateFenceCommandHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepoMock = new();
  private readonly CreateFenceCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public CreateFenceCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(u => u.Fences).Returns(_fenceRepoMock.Object);
    _handler = new CreateFenceCommandHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ValidCommand_ShouldCreateAndReturnFence()
  {
    var command = new CreateFenceCommand("Dev Zone", 100, 200, 800, 600, "#FF0078D4", 75);

    var result = await _handler.Handle(command, CancellationToken.None);

    result.Name.ShouldBe("Dev Zone");
    result.Position.X.ShouldBe(100);
    result.Position.Y.ShouldBe(200);
    result.Dimensions.Width.ShouldBe(800);
    result.Dimensions.Height.ShouldBe(600);
    result.Opacity.Percentage.ShouldBe(75);
    result.IsActive.ShouldBeTrue();
  }

  [Fact]
  public async Task Handle_ShouldAddFenceToRepository()
  {
    var command = new CreateFenceCommand("Test", 0, 0, 200, 200, "#FFFFFFFF", 50);

    await _handler.Handle(command, CancellationToken.None);

    _fenceRepoMock.Verify(r => r.Add(It.IsAny<Fence>()), Times.Once);
  }

  [Fact]
  public async Task Handle_ShouldCallSaveChanges()
  {
    var command = new CreateFenceCommand("Test", 0, 0, 200, 200, "#FFFFFFFF", 50);

    await _handler.Handle(command, CancellationToken.None);

    _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  #endregion
}
