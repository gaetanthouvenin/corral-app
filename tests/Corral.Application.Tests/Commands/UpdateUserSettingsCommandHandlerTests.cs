// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateUserSettingsCommandHandlerTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Application.Commands.UpdateUserSettings;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;

namespace Corral.Application.Tests.Commands;

public class UpdateUserSettingsCommandHandlerTests
{
  #region Fields

  private readonly UpdateUserSettingsCommandHandler _handler;
  private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
  private readonly Mock<IUserSettingsRepository> _userSettingsRepositoryMock = new();

  #endregion

  #region Ctors

  public UpdateUserSettingsCommandHandlerTests()
  {
    _unitOfWorkMock.Setup(x => x.UserSettings).Returns(_userSettingsRepositoryMock.Object);
    _handler = new UpdateUserSettingsCommandHandler(_unitOfWorkMock.Object);
  }

  #endregion

  #region Methods

  [Fact]
  public async Task Handle_ShouldUpdateSettingsUpsertAndSave()
  {
    var settings = UserSettings.Reconstitute(
      UserSettings.SingletonId,
      0,
      0,
      new DateTime(2026, 4, 1, 9, 0, 0, DateTimeKind.Utc)
    );

    _userSettingsRepositoryMock.Setup(x => x.GetAsync(It.IsAny<CancellationToken>()))
                               .ReturnsAsync(settings);

    var result = await _handler.Handle(new UpdateUserSettingsCommand(1, 2), CancellationToken.None);

    result.ClickMode.ShouldBe(1);
    result.IconLayout.ShouldBe(2);
    _userSettingsRepositoryMock.Verify(x => x.Upsert(settings), Times.Once);
    _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
  }

  #endregion
}
