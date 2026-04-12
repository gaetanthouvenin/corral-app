// ------------------------------------------------------------------------------------------------
// <copyright file="GetFenceByIdQuery.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Queries.GetFenceById;

/// <summary>
///   Query to retrieve a specific fence by its identifier.
/// </summary>
/// <remarks>
///   This query returns a single fence if found, or null if not found.
/// </remarks>
/// <param name="FenceId">The ID of the fence to retrieve.</param>
public record GetFenceByIdQuery(string FenceId) : IQuery<Fence>, IRequest<Fence>;
