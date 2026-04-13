// ------------------------------------------------------------------------------------------------
// <copyright file="RemoveItemFromFenceCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using FluentValidation;

namespace Corral.Application.Commands.RemoveItemFromFence;

/// <summary>
///   Validator for the RemoveItemFromFenceCommand.
/// </summary>
public class RemoveItemFromFenceCommandValidator : AbstractValidator<RemoveItemFromFenceCommand>
{
  #region Ctors

  /// <summary>
  ///   Initializes validation rules for RemoveItemFromFenceCommand.
  /// </summary>
  public RemoveItemFromFenceCommandValidator()
  {
    RuleFor(x => x.FenceId).NotEmpty().WithMessage("FenceId is required.");

    RuleFor(x => x.ItemId).NotEmpty().WithMessage("ItemId is required.");
  }

  #endregion
}
