// ------------------------------------------------------------------------------------------------
// <copyright file="SearchFencesQueryHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;

using MediatR;

namespace Corral.Application.Queries.SearchFences;

/// <summary>
///   Handler for SearchFencesQuery.
///   Searches for fences by name using case-insensitive matching.
/// </summary>
public class SearchFencesQueryHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<SearchFencesQuery, List<Fence>>
{
  #region Implementation of IRequestHandler<SearchFencesQuery,List<Fence>>

  /// <summary>
  ///   Handles the search query.
  /// </summary>
  /// <param name="request">The SearchFencesQuery containing the search term.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>A list of fences matching the search term.</returns>
  public async Task<List<Fence>> Handle(
    SearchFencesQuery request,
    CancellationToken cancellationToken)
  {
    var searchTerm = request.SearchTerm?.Trim() ?? string.Empty;

    return await unitOfWork.Fences.SearchByNameAsync(searchTerm, cancellationToken);
  }

  #endregion
}
