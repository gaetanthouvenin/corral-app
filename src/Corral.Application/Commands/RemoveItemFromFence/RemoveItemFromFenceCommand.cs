// ------------------------------------------------------------------------------------------------
// <copyright file="RemoveItemFromFenceCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.RemoveItemFromFence;

/// <summary>
///   Command to remove an item from an existing fence.
/// </summary>
/// <param name="FenceId">The ID of the fence containing the item.</param>
/// <param name="ItemId">The ID of the item to remove.</param>
public record RemoveItemFromFenceCommand(string FenceId, string ItemId)
  : ICommand<Fence>, IRequest<Fence>;
