// ------------------------------------------------------------------------------------------------
// <copyright file="GetActiveFencesQueryHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Queries.GetActiveFences;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Queries;

public class GetActiveFencesQueryHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepoMock = new();
  private readonly GetActiveFencesQueryHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public GetActiveFencesQueryHandlerTests()
  {
    _unitOfWorkMock.Setup(u => u.Fences).Returns(_fenceRepoMock.Object);
    _handler = new GetActiveFencesQueryHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ShouldReturnOnlyActiveFences()
  {
    var activeFences = new List<Fence>
    {
      Fence.Reconstitute(
        FenceId.Create("1"),
        "Active",
        Position.Create(0, 0),
        Dimensions.Create(200, 200),
        Color.White,
        Opacity.Opaque,
        true,
        DateTime.UtcNow,
        null
      )
    };

    _fenceRepoMock.Setup(r => r.GetActivesAsync(It.IsAny<CancellationToken>()))
                  .ReturnsAsync(activeFences);

    var result = await _handler.Handle(new GetActiveFencesQuery(), CancellationToken.None);

    result.Count.ShouldBe(1);
    result[0].IsActive.ShouldBeTrue();
  }

  [Fact]
  public async Task Handle_NoActiveFences_ShouldReturnEmptyList()
  {
    _fenceRepoMock.Setup(r => r.GetActivesAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

    var result = await _handler.Handle(new GetActiveFencesQuery(), CancellationToken.None);

    result.ShouldBeEmpty();
  }

  #endregion
}
