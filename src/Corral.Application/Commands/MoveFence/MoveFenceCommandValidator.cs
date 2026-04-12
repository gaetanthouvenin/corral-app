// ------------------------------------------------------------------------------------------------
// <copyright file="MoveFenceCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using FluentValidation;

namespace Corral.Application.Commands.MoveFence;

/// <summary>
///   Validator for MoveFenceCommand.
///   Ensures the new position is valid.
/// </summary>
/// <remarks>
///   Validates that:
///   - Fence ID is provided
///   - X and Y coordinates are non-negative
/// </remarks>
public class MoveFenceCommandValidator : AbstractValidator<MoveFenceCommand>
{
  #region Ctors

  /// <summary>
  ///   Initializes validation rules for fence movement.
  /// </summary>
  public MoveFenceCommandValidator()
  {
    RuleFor(cmd => cmd.FenceId).NotEmpty().WithMessage("Fence ID is required");

    RuleFor(cmd => cmd.NewPositionX)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Position X must be greater than or equal to 0");

    RuleFor(cmd => cmd.NewPositionY)
      .GreaterThanOrEqualTo(0)
      .WithMessage("Position Y must be greater than or equal to 0");
  }

  #endregion
}
