// ------------------------------------------------------------------------------------------------
// <copyright file="ReorderFenceItemsCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.ReorderFenceItems;

/// <summary>
///   Command to reorder items inside a fence.
/// </summary>
/// <param name="FenceId">The ID of the fence that owns the items.</param>
/// <param name="ItemId">The ID of the item being moved.</param>
/// <param name="TargetItemId">
///   The ID of the item before which the moved item is inserted. Empty means
///   move to the end.
/// </param>
public record ReorderFenceItemsCommand(string FenceId, string ItemId, string TargetItemId)
  : ICommand<Fence>, IRequest<Fence>;
