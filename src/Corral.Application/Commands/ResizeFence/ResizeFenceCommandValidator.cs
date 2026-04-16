// ------------------------------------------------------------------------------------------------
// <copyright file="ResizeFenceCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using FluentValidation;

namespace Corral.Application.Commands.ResizeFence;

/// <summary>
///   Validator for ResizeFenceCommand.
/// </summary>
public class ResizeFenceCommandValidator : AbstractValidator<ResizeFenceCommand>
{
  /// <summary>
  ///   Initializes validation rules for fence resize.
  /// </summary>
  public ResizeFenceCommandValidator()
  {
    RuleFor(cmd => cmd.FenceId).NotEmpty().WithMessage("Fence ID is required");

    RuleFor(cmd => cmd.NewWidth)
      .GreaterThan(0)
      .WithMessage("Width must be greater than 0");

    RuleFor(cmd => cmd.NewHeight)
      .GreaterThan(0)
      .WithMessage("Height must be greater than 0");
  }
}
