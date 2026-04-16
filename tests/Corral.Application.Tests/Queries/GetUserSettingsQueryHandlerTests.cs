// ------------------------------------------------------------------------------------------------
// <copyright file="GetUserSettingsQueryHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Queries.GetUserSettings;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;

namespace Corral.Application.Tests.Queries;

public class GetUserSettingsQueryHandlerTests
{
  #region Fields

  private readonly GetUserSettingsQueryHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
  private readonly Mock<IUserSettingsRepository> _userSettingsRepositoryMock = new();

  #endregion

  #region Ctors

  public GetUserSettingsQueryHandlerTests()
  {
    _unitOfWorkMock.Setup(x => x.UserSettings).Returns(_userSettingsRepositoryMock.Object);
    _handler = new GetUserSettingsQueryHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ShouldReturnSettingsFromRepository()
  {
    var settings = UserSettings.Reconstitute(
      UserSettings.SingletonId,
      1,
      2,
      new DateTime(2026, 4, 1, 12, 0, 0, DateTimeKind.Utc)
    );

    _userSettingsRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(settings);

    var result = await _handler.Handle(new GetUserSettingsQuery(), CancellationToken.None);

    result.ShouldBeSameAs(settings);
  }

  #endregion
}
