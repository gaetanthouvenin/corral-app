// ------------------------------------------------------------------------------------------------
// <copyright file="GetAllFencesQuery.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Queries.GetAllFences;

/// <summary>
///   Query to retrieve all fences in the system.
/// </summary>
/// <remarks>
///   This query returns all fences regardless of their active status.
///   Use GetActiveFencesQuery to retrieve only active fences.
/// </remarks>
public record GetAllFencesQuery : IQuery<List<Fence>>, IRequest<List<Fence>>;
