// ------------------------------------------------------------------------------------------------
// <copyright file="GetActiveFencesQuery.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Queries.GetActiveFences;

/// <summary>
///   Query to retrieve all active fences in the system.
/// </summary>
/// <remarks>
///   This query returns only fences where IsActive is true.
///   Use GetAllFencesQuery to retrieve all fences regardless of status.
/// </remarks>
public record GetActiveFencesQuery : IQuery<List<Fence>>, IRequest<List<Fence>>;
