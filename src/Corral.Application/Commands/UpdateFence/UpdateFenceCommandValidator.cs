// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateFenceCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using FluentValidation;

namespace Corral.Application.Commands.UpdateFence;

/// <summary>
///   Validator for UpdateFenceCommand.
///   Ensures all update command properties are valid.
/// </summary>
/// <remarks>
///   Validates fence ID, name, color format (#AARRGGBB), and opacity range.
/// </remarks>
public class UpdateFenceCommandValidator : AbstractValidator<UpdateFenceCommand>
{
  #region Ctors

  /// <summary>
  ///   Initializes validation rules for fence updates.
  /// </summary>
  public UpdateFenceCommandValidator()
  {
    RuleFor(cmd => cmd.FenceId).NotEmpty().WithMessage("Fence ID is required");

    RuleFor(cmd => cmd.Name)
      .NotEmpty()
      .MaximumLength(255)
      .WithMessage("Fence name is required and must not exceed 255 characters");

    RuleFor(cmd => cmd.BackgroundColor)
      .NotEmpty()
      .Matches("^#[0-9A-Fa-f]{8}$")
      .WithMessage("Background color must be in #AARRGGBB format");

    RuleFor(cmd => cmd.Opacity)
      .InclusiveBetween(0, 100)
      .WithMessage("Opacity must be between 0 and 100 percent");
  }

  #endregion
}
