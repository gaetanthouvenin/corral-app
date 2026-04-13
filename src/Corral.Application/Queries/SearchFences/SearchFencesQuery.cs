// ------------------------------------------------------------------------------------------------
// <copyright file="SearchFencesQuery.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Queries.SearchFences;

/// <summary>
///   Query to search for fences by name.
/// </summary>
/// <remarks>
///   Performs a case-insensitive search on fence names.
///   Returns all fences whose names contain the search term.
/// </remarks>
/// <param name="SearchTerm">The search term to match against fence names.</param>
public record SearchFencesQuery(string SearchTerm) : IQuery<List<Fence>>, IRequest<List<Fence>>;
