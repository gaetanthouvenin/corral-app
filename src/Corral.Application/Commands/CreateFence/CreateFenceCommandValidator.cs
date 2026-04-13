// ------------------------------------------------------------------------------------------------
// <copyright file="CreateFenceCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using FluentValidation;

namespace Corral.Application.Commands.CreateFence;

/// <summary>
///   Validator for CreateFenceCommand.
///   Ensures all command properties meet business requirements and constraints.
/// </summary>
/// <remarks>
///   This validator enforces:
///   - Name is not empty and not too long
///   - Position coordinates are valid
///   - Dimensions are within acceptable ranges
///   - Color is in valid hexadecimal format
///   - Opacity is a valid percentage
/// </remarks>
public class CreateFenceCommandValidator : AbstractValidator<CreateFenceCommand>
{
  #region Ctors

  /// <summary>
  ///   Initializes validation rules for fence creation.
  /// </summary>
  public CreateFenceCommandValidator()
  {
    RuleFor(cmd => cmd.Name).NotEmpty().WithMessage("Fence name is required");

    RuleFor(cmd => cmd.Name)
      .MaximumLength(255)
      .WithMessage("Fence name must not exceed 255 characters");

    RuleFor(cmd => cmd.PositionX)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Position X must be greater than or equal to 0");

    RuleFor(cmd => cmd.PositionY)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Position Y must be greater than or equal to 0");

    RuleFor(cmd => cmd.Width)
      .InclusiveBetween(50, 2560)
      .WithMessage("Width must be between 50 and 2560 pixels");

    RuleFor(cmd => cmd.Height)
      .InclusiveBetween(50, 1440)
      .WithMessage("Height must be between 50 and 1440 pixels");

    RuleFor(cmd => cmd.BackgroundColor).NotEmpty().WithMessage("Background color is required");

    RuleFor(cmd => cmd.BackgroundColor)
      .Matches(@"^#[0-9A-Fa-f]{8}$")
      .WithMessage("Background color must be in #AARRGGBB format (e.g. #FF0078D4)");

    RuleFor(cmd => cmd.Opacity)
      .InclusiveBetween(0, 100)
      .WithMessage("Opacity must be between 0 and 100 percent");
  }

  #endregion
}
