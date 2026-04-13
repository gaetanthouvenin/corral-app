// ------------------------------------------------------------------------------------------------
// <copyright file="MoveFenceCommandValidatorTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Commands.MoveFence;

using FluentValidation.TestHelper;

namespace Corral.Application.Tests.Commands;

public class MoveFenceCommandValidatorTests
{
  #region Fields

  private readonly MoveFenceCommandValidator _validator = new();

  #endregion

  #region Properties

  private static MoveFenceCommand ValidCommand => new("valid-fence-id", 100, 200);

  #endregion

  #region Methods

  [Fact]
  public void ValidCommand_ShouldHaveNoErrors()
  {
    var result = _validator.TestValidate(ValidCommand);

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void FenceId_Empty_ShouldHaveError(string fenceId)
  {
    var cmd = ValidCommand with { FenceId = fenceId };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.FenceId);
  }

  [Fact]
  public void NewPositionX_Negative_ShouldHaveError()
  {
    var cmd = ValidCommand with { NewPositionX = -1 };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.NewPositionX);
  }

  [Fact]
  public void NewPositionY_Negative_ShouldHaveError()
  {
    var cmd = ValidCommand with { NewPositionY = -1 };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.NewPositionY);
  }

  [Fact]
  public void Position_Zero_ShouldNotHaveError()
  {
    var cmd = ValidCommand with { NewPositionX = 0, NewPositionY = 0 };

    var result = _validator.TestValidate(cmd);

    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion
}
