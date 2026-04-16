// ------------------------------------------------------------------------------------------------
// <copyright file="GetFenceByIdQueryHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Queries.GetFenceById;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Queries;

public class GetFenceByIdQueryHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepositoryMock = new();
  private readonly GetFenceByIdQueryHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public GetFenceByIdQueryHandlerTests()
  {
    _unitOfWorkMock.Setup(x => x.Fences).Returns(_fenceRepositoryMock.Object);
    _handler = new GetFenceByIdQueryHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ShouldReturnFenceWhenFound()
  {
    var fence = Fence.Reconstitute(
      FenceId.Create(Guid.NewGuid().ToString()),
      "Fence",
      Position.Create(0, 0),
      Dimensions.Create(200, 200),
      Color.White,
      Opacity.Opaque,
      true,
      DateTime.UtcNow,
      null
    );

    _fenceRepositoryMock
      .Setup(x => x.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(fence);

    var result = await _handler.Handle(
                   new GetFenceByIdQuery(Guid.NewGuid().ToString()),
                   CancellationToken.None
                 );

    result.ShouldBeSameAs(fence);
  }

  [Fact]
  public async Task Handle_ShouldReturnNullWhenNotFound()
  {
    _fenceRepositoryMock
      .Setup(x => x.GetByIdAsync(It.IsAny<FenceId>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((Fence)null);

    var result = await _handler.Handle(
                   new GetFenceByIdQuery(Guid.NewGuid().ToString()),
                   CancellationToken.None
                 );

    result.ShouldBeNull();
  }

  [Fact]
  public void Constructor_WithNullUnitOfWork_ShouldThrow()
  {
    Should.Throw<ArgumentNullException>(() => new GetFenceByIdQueryHandler(null));
  }

  #endregion
}
