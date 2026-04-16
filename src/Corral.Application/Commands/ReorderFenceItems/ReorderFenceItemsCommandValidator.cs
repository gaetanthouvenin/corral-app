// ------------------------------------------------------------------------------------------------
// <copyright file="ReorderFenceItemsCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using FluentValidation;

namespace Corral.Application.Commands.ReorderFenceItems;

/// <summary>
///   Validator for ReorderFenceItemsCommand.
/// </summary>
public class ReorderFenceItemsCommandValidator : AbstractValidator<ReorderFenceItemsCommand>
{
  #region Ctors

  /// <summary>
  ///   Initializes validation rules for item reordering.
  /// </summary>
  public ReorderFenceItemsCommandValidator()
  {
    RuleFor(x => x.FenceId).NotEmpty().WithMessage("Fence ID is required");
    RuleFor(x => x.ItemId).NotEmpty().WithMessage("Item ID is required");
  }

  #endregion
}
