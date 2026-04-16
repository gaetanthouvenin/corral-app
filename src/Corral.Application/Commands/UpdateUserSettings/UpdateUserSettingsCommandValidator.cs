// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateUserSettingsCommandValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using FluentValidation;

namespace Corral.Application.Commands.UpdateUserSettings;

/// <summary>
///   Validates <see cref="UpdateUserSettingsCommand" />.
/// </summary>
public class UpdateUserSettingsCommandValidator : AbstractValidator<UpdateUserSettingsCommand>
{
  #region Ctors

  public UpdateUserSettingsCommandValidator()
  {
    RuleFor(c => c.ClickMode)
      .InclusiveBetween(0, 1)
      .WithMessage("ClickMode must be 0 (SingleClick) or 1 (DoubleClick).");

    RuleFor(c => c.IconLayout)
      .InclusiveBetween(0, 2)
      .WithMessage("IconLayout must be 0 (LargeGrid), 1 (SmallGrid) or 2 (List).");
  }

  #endregion
}
