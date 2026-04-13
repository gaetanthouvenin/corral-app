// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateFenceCommandValidatorTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Commands.UpdateFence;

using FluentValidation.TestHelper;

namespace Corral.Application.Tests.Commands;

public class UpdateFenceCommandValidatorTests
{
  #region Fields

  private readonly UpdateFenceCommandValidator _validator = new();

  #endregion

  #region Properties

  private static UpdateFenceCommand ValidCommand
    => new("valid-fence-id", "Updated Fence", "#FF0078D4", 50);

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

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void Name_Empty_ShouldHaveError(string name)
  {
    var cmd = ValidCommand with { Name = name };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Name);
  }

  [Theory]
  [InlineData("FF0078D4")]
  [InlineData("#FF007")]
  [InlineData("#ZZZZZZZZ")]
  public void BackgroundColor_InvalidFormat_ShouldHaveError(string color)
  {
    var cmd = ValidCommand with { BackgroundColor = color };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.BackgroundColor);
  }

  [Fact]
  public void Opacity_BelowZero_ShouldHaveError()
  {
    var cmd = ValidCommand with { Opacity = -1 };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Opacity);
  }

  [Fact]
  public void Opacity_AboveHundred_ShouldHaveError()
  {
    var cmd = ValidCommand with { Opacity = 101 };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Opacity);
  }

  #endregion
}
