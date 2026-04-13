// ------------------------------------------------------------------------------------------------
// <copyright file="AddItemToFenceCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.AddItemToFence;

/// <summary>
///   Command to add an item (shortcut, file, or link) to an existing fence.
/// </summary>
/// <param name="FenceId">The ID of the fence to add the item to.</param>
/// <param name="DisplayName">The display name for the item.</param>
/// <param name="Path">The file path, shortcut target, or URL.</param>
/// <param name="ItemType">The type of item (0=Shortcut, 1=File, 2=Link).</param>
public record AddItemToFenceCommand(string FenceId, string DisplayName, string Path, int ItemType)
  : ICommand<Fence>, IRequest<Fence>;
