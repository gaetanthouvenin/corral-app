// ------------------------------------------------------------------------------------------------
// <copyright file="SearchFencesQueryValidator.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using FluentValidation;

namespace Corral.Application.Queries.SearchFences;

/// <summary>
///   Validator for SearchFencesQuery.
///   Ensures the search term is within acceptable bounds.
/// </summary>
public class SearchFencesQueryValidator : AbstractValidator<SearchFencesQuery>
{
  #region Ctors

  /// <summary>
  ///   Initializes validation rules for the search query.
  /// </summary>
  public SearchFencesQueryValidator()
  {
    RuleFor(q => q.SearchTerm)
      .MaximumLength(100)
      .WithMessage("Search term must not exceed 100 characters");
  }

  #endregion
}
