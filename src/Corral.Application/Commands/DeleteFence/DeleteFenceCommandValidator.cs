// ------------------------------------------------------------------------------------------------
// <copyright file="DeleteFenceCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using FluentValidation;

namespace Corral.Application.Commands.DeleteFence;

/// <summary>
///   Validator for DeleteFenceCommand.
///   Ensures the fence ID is present and well-formed before attempting deletion.
/// </summary>
public class DeleteFenceCommandValidator : AbstractValidator<DeleteFenceCommand>
{
  #region Ctors

  /// <summary>
  ///   Initializes validation rules for fence deletion.
  /// </summary>
  public DeleteFenceCommandValidator()
  {
    RuleFor(cmd => cmd.FenceId)
      .NotEmpty()
      .WithMessage("Fence ID is required");

    RuleFor(cmd => cmd.FenceId)
      .Must(id => Guid.TryParse(id, out _))
      .When(cmd => !string.IsNullOrEmpty(cmd.FenceId))
      .WithMessage("Fence ID must be a valid GUID");
  }

  #endregion
}
