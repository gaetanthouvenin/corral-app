// ------------------------------------------------------------------------------------------------
// <copyright file="DeleteFenceCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.DeleteFence;

/// <summary>
///   Command to delete an existing fence from the system.
/// </summary>
/// <remarks>
///   This command marks a fence for deletion by raising a FenceDeletedEvent.
///   The physical removal from the database depends on the persistence strategy.
/// </remarks>
/// <param name="FenceId">The ID of the fence to delete.</param>
public record DeleteFenceCommand(string FenceId) : ICommand<Fence>, IRequest<Fence>;
