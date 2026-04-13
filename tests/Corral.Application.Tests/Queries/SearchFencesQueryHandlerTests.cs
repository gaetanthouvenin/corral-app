// ------------------------------------------------------------------------------------------------
// <copyright file="SearchFencesQueryHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Queries.SearchFences;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

namespace Corral.Application.Tests.Queries;

public class SearchFencesQueryHandlerTests
{
  #region Fields

  private readonly Mock<IFenceRepository> _fenceRepoMock = new();
  private readonly SearchFencesQueryHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

  #endregion

  #region Ctors

  public SearchFencesQueryHandlerTests()
  {
    _unitOfWorkMock.Setup(u => u.Fences).Returns(_fenceRepoMock.Object);
    _handler = new SearchFencesQueryHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  private void SetupFences()
  {
    var fences = new List<Fence>
    {
      Fence.Reconstitute(
        FenceId.Create("1"),
        "Development Zone",
        Position.Create(0, 0),
        Dimensions.Create(200, 200),
        Color.Blue,
        Opacity.Opaque,
        true,
        DateTime.UtcNow,
        null
      ),
      Fence.Reconstitute(
        FenceId.Create("2"),
        "Communication Area",
        Position.Create(100, 100),
        Dimensions.Create(200, 200),
        Color.Red,
        Opacity.Opaque,
        true,
        DateTime.UtcNow,
        null
      ),
      Fence.Reconstitute(
        FenceId.Create("3"),
        "Dev Tools",
        Position.Create(200, 200),
        Dimensions.Create(200, 200),
        Color.Green,
        Opacity.Opaque,
        true,
        DateTime.UtcNow,
        null
      )
    };

    _fenceRepoMock
      .Setup(r => r.SearchByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((string term, CancellationToken _) => fences
                                                          .Where(f => f.Name.Contains(
                                                                   term,
                                                                   StringComparison
                                                                     .OrdinalIgnoreCase
                                                                 )
                                                          )
                                                          .ToList()
      );
  }

  [Fact]
  public async Task Handle_MatchingTerm_ShouldReturnFilteredResults()
  {
    SetupFences();

    var result = await _handler.Handle(new SearchFencesQuery("dev"), CancellationToken.None);

    result.Count.ShouldBe(2);
  }

  [Fact]
  public async Task Handle_NoMatch_ShouldReturnEmptyList()
  {
    SetupFences();

    var result = await _handler.Handle(new SearchFencesQuery("xyz"), CancellationToken.None);

    result.ShouldBeEmpty();
  }

  [Fact]
  public async Task Handle_CaseInsensitive_ShouldMatch()
  {
    SetupFences();

    var result = await _handler.Handle(new SearchFencesQuery("DEV"), CancellationToken.None);

    result.Count.ShouldBe(2);
  }

  [Fact]
  public async Task Handle_EmptySearchTerm_ShouldReturnAll()
  {
    SetupFences();

    var result = await _handler.Handle(new SearchFencesQuery(""), CancellationToken.None);

    result.Count.ShouldBe(3);
  }

  #endregion
}
