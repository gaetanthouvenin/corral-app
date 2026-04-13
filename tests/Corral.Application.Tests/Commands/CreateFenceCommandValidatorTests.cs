// ------------------------------------------------------------------------------------------------
// <copyright file="CreateFenceCommandValidatorTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Commands.CreateFence;

using FluentValidation.TestHelper;

namespace Corral.Application.Tests.Commands;

public class CreateFenceCommandValidatorTests
{
  #region Fields

  private readonly CreateFenceCommandValidator _validator = new();

  #endregion

  #region Properties

  private static CreateFenceCommand ValidCommand
    => new("Test Fence", 100, 100, 200, 200, "#FF0078D4", 75);

  #endregion

  #region Methods

  [Fact]
  public void ValidCommand_ShouldHaveNoErrors()
  {
    var result = _validator.TestValidate(ValidCommand);

    result.ShouldNotHaveAnyValidationErrors();
  }

  #endregion

  #region Name

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void Name_Empty_ShouldHaveError(string name)
  {
    var cmd = ValidCommand with { Name = name };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Name);
  }

  [Fact]
  public void Name_TooLong_ShouldHaveError()
  {
    var cmd = ValidCommand with { Name = new string('A', 256) };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Name);
  }

  [Fact]
  public void Name_MaxLength_ShouldNotHaveError()
  {
    var cmd = ValidCommand with { Name = new string('A', 255) };

    var result = _validator.TestValidate(cmd);

    result.ShouldNotHaveValidationErrorFor(c => c.Name);
  }

  #endregion

  #region Position

  [Fact]
  public void PositionX_Negative_ShouldHaveError()
  {
    var cmd = ValidCommand with { PositionX = -1 };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.PositionX);
  }

  [Fact]
  public void PositionY_Negative_ShouldHaveError()
  {
    var cmd = ValidCommand with { PositionY = -1 };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.PositionY);
  }

  [Fact]
  public void Position_Zero_ShouldNotHaveError()
  {
    var cmd = ValidCommand with { PositionX = 0, PositionY = 0 };

    var result = _validator.TestValidate(cmd);

    result.ShouldNotHaveValidationErrorFor(c => c.PositionX);
    result.ShouldNotHaveValidationErrorFor(c => c.PositionY);
  }

  #endregion

  #region Dimensions

  [Theory]
  [InlineData(49)]
  [InlineData(0)]
  [InlineData(-1)]
  public void Width_BelowMinimum_ShouldHaveError(int width)
  {
    var cmd = ValidCommand with { Width = width };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Width);
  }

  [Fact]
  public void Width_AboveMaximum_ShouldHaveError()
  {
    var cmd = ValidCommand with { Width = 2561 };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Width);
  }

  [Theory]
  [InlineData(49)]
  [InlineData(0)]
  [InlineData(-1)]
  public void Height_BelowMinimum_ShouldHaveError(int height)
  {
    var cmd = ValidCommand with { Height = height };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Height);
  }

  [Fact]
  public void Height_AboveMaximum_ShouldHaveError()
  {
    var cmd = ValidCommand with { Height = 1441 };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.Height);
  }

  [Fact]
  public void Dimensions_AtBoundaries_ShouldNotHaveError()
  {
    var cmd = ValidCommand with { Width = 50, Height = 50 };

    var result = _validator.TestValidate(cmd);

    result.ShouldNotHaveValidationErrorFor(c => c.Width);
    result.ShouldNotHaveValidationErrorFor(c => c.Height);
  }

  #endregion

  #region BackgroundColor

  [Theory]
  [InlineData(null)]
  [InlineData("")]
  public void BackgroundColor_Empty_ShouldHaveError(string color)
  {
    var cmd = ValidCommand with { BackgroundColor = color };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.BackgroundColor);
  }

  [Theory]
  [InlineData("FF0078D4")]
  [InlineData("#FF007")]
  [InlineData("#ZZZZZZZZ")]
  [InlineData("not-a-color")]
  public void BackgroundColor_InvalidFormat_ShouldHaveError(string color)
  {
    var cmd = ValidCommand with { BackgroundColor = color };

    var result = _validator.TestValidate(cmd);

    result.ShouldHaveValidationErrorFor(c => c.BackgroundColor);
  }

  [Theory]
  [InlineData("#FF0078D4")]
  [InlineData("#00FFFFFF")]
  [InlineData("#ff00aabb")]
  public void BackgroundColor_ValidFormat_ShouldNotHaveError(string color)
  {
    var cmd = ValidCommand with { BackgroundColor = color };

    var result = _validator.TestValidate(cmd);

    result.ShouldNotHaveValidationErrorFor(c => c.BackgroundColor);
  }

  #endregion

  #region Opacity

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

  [Theory]
  [InlineData(0)]
  [InlineData(50)]
  [InlineData(100)]
  public void Opacity_ValidRange_ShouldNotHaveError(int opacity)
  {
    var cmd = ValidCommand with { Opacity = opacity };

    var result = _validator.TestValidate(cmd);

    result.ShouldNotHaveValidationErrorFor(c => c.Opacity);
  }

  #endregion
}
