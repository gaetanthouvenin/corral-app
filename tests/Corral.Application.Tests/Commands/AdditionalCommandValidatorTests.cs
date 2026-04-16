// ------------------------------------------------------------------------------------------------
// <copyright file="AdditionalCommandValidatorTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Commands.AddItemToFence;
using Corral.Application.Commands.DeleteFence;
using Corral.Application.Commands.RemoveItemFromFence;
using Corral.Application.Commands.ResizeFence;
using Corral.Application.Commands.UpdateUserSettings;
using Corral.Application.Queries.SearchFences;

using FluentValidation.TestHelper;

namespace Corral.Application.Tests.Commands;

public class AdditionalCommandValidatorTests
{
  #region Methods

  [Fact]
  public void AddItemToFenceValidator_ValidCommand_ShouldHaveNoErrors()
  {
    var validator = new AddItemToFenceCommandValidator();

    var result = validator.TestValidate(
      new AddItemToFenceCommand("fence-1", "Docs", "https://example.com", 2)
    );

    result.ShouldNotHaveAnyValidationErrors();
  }

  [Fact]
  public void AddItemToFenceValidator_InvalidValues_ShouldHaveErrors()
  {
    var validator = new AddItemToFenceCommandValidator();

    var result = validator.TestValidate(new AddItemToFenceCommand("", "", "", 99));

    result.ShouldHaveValidationErrorFor(x => x.FenceId);
    result.ShouldHaveValidationErrorFor(x => x.DisplayName);
    result.ShouldHaveValidationErrorFor(x => x.Path);
    result.ShouldHaveValidationErrorFor(x => x.ItemType);
  }

  [Fact]
  public void RemoveItemFromFenceValidator_InvalidValues_ShouldHaveErrors()
  {
    var validator = new RemoveItemFromFenceCommandValidator();

    var result = validator.TestValidate(new RemoveItemFromFenceCommand("", ""));

    result.ShouldHaveValidationErrorFor(x => x.FenceId);
    result.ShouldHaveValidationErrorFor(x => x.ItemId);
  }

  [Fact]
  public void ResizeFenceValidator_InvalidValues_ShouldHaveErrors()
  {
    var validator = new ResizeFenceCommandValidator();

    var result = validator.TestValidate(new ResizeFenceCommand("", 0, -1));

    result.ShouldHaveValidationErrorFor(x => x.FenceId);
    result.ShouldHaveValidationErrorFor(x => x.NewWidth);
    result.ShouldHaveValidationErrorFor(x => x.NewHeight);
  }

  [Fact]
  public void UpdateUserSettingsValidator_OutOfRangeValues_ShouldHaveErrors()
  {
    var validator = new UpdateUserSettingsCommandValidator();

    var result = validator.TestValidate(new UpdateUserSettingsCommand(9, -1));

    result.ShouldHaveValidationErrorFor(x => x.ClickMode);
    result.ShouldHaveValidationErrorFor(x => x.IconLayout);
  }

  [Fact]
  public void DeleteFenceValidator_InvalidGuid_ShouldHaveError()
  {
    var validator = new DeleteFenceCommandValidator();

    var result = validator.TestValidate(new DeleteFenceCommand("not-a-guid"));

    result.ShouldHaveValidationErrorFor(x => x.FenceId);
  }

  [Fact]
  public void DeleteFenceValidator_ValidGuid_ShouldNotHaveError()
  {
    var validator = new DeleteFenceCommandValidator();

    var result = validator.TestValidate(new DeleteFenceCommand(Guid.NewGuid().ToString()));

    result.ShouldNotHaveValidationErrorFor(x => x.FenceId);
  }

  [Fact]
  public void SearchFencesValidator_TooLongTerm_ShouldHaveError()
  {
    var validator = new SearchFencesQueryValidator();

    var result = validator.TestValidate(new SearchFencesQuery(new string('a', 101)));

    result.ShouldHaveValidationErrorFor(x => x.SearchTerm);
  }

  [Fact]
  public void SearchFencesValidator_EmptyOrShortTerm_ShouldNotHaveError()
  {
    var validator = new SearchFencesQueryValidator();

    validator.TestValidate(new SearchFencesQuery(string.Empty))
             .ShouldNotHaveValidationErrorFor(x => x.SearchTerm);

    validator.TestValidate(new SearchFencesQuery("dev"))
             .ShouldNotHaveValidationErrorFor(x => x.SearchTerm);
  }

  #endregion
}
