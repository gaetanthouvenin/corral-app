// ------------------------------------------------------------------------------------------------
// <copyright file="GetAllFencesQueryHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Queries.GetAllFences;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Queries;

public class GetAllFencesQueryHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepoMock = new();
  private readonly GetAllFencesQueryHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public GetAllFencesQueryHandlerTests()
  {
    _unitOfWorkMock.Setup(u => u.Fences).Returns(_fenceRepoMock.Object);
    _handler = new GetAllFencesQueryHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ShouldReturnAllFences()
  {
    var fences = new List<Fence>
    {
      Fence.Reconstitute(
        FenceId.Create("1"),
        "Fence1",
        Position.Create(0, 0),
        Dimensions.Create(200, 200),
        Color.White,
        Opacity.Opaque,
        true,
        DateTime.UtcNow,
        null
      ),
      Fence.Reconstitute(
        FenceId.Create("2"),
        "Fence2",
        Position.Create(100, 100),
        Dimensions.Create(300, 300),
        Color.Blue,
        Opacity.SemiTransparent,
        false,
        DateTime.UtcNow,
        null
      )
    };

    _fenceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(fences);

    var result = await _handler.Handle(new GetAllFencesQuery(), CancellationToken.None);

    result.Count.ShouldBe(2);
  }

  [Fact]
  public async Task Handle_EmptyRepository_ShouldReturnEmptyList()
  {
    _fenceRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

    var result = await _handler.Handle(new GetAllFencesQuery(), CancellationToken.None);

    result.ShouldBeEmpty();
  }

  #endregion
}
