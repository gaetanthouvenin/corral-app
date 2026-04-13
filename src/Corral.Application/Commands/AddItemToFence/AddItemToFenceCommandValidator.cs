// ------------------------------------------------------------------------------------------------
// <copyright file="AddItemToFenceCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using FluentValidation;

namespace Corral.Application.Commands.AddItemToFence;

/// <summary>
///   Validator for the AddItemToFenceCommand.
/// </summary>
public class AddItemToFenceCommandValidator : AbstractValidator<AddItemToFenceCommand>
{
  #region Ctors

  /// <summary>
  ///   Initializes validation rules for AddItemToFenceCommand.
  /// </summary>
  public AddItemToFenceCommandValidator()
  {
    RuleFor(x => x.FenceId).NotEmpty().WithMessage("FenceId is required.");

    RuleFor(x => x.DisplayName)
      .NotEmpty()
      .WithMessage("DisplayName is required.")
      .MaximumLength(256)
      .WithMessage("DisplayName must not exceed 256 characters.");

    RuleFor(x => x.Path)
      .NotEmpty()
      .WithMessage("Path is required.")
      .MaximumLength(1024)
      .WithMessage("Path must not exceed 1024 characters.");

    RuleFor(x => x.ItemType)
      .InclusiveBetween(0, 2)
      .WithMessage("ItemType must be 0 (Shortcut), 1 (File), or 2 (Link).");
  }

  #endregion
}
