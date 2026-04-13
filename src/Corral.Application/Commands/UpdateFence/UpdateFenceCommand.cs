// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateFenceCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.UpdateFence;

/// <summary>
///   Command to update the properties of an existing fence.
/// </summary>
/// <remarks>
///   This command is used to update name, color, and opacity of a fence.
///   Position changes should use MoveFenceCommand.
///   Dimension changes should be handled separately (future enhancement).
/// </remarks>
/// <param name="FenceId">The ID of the fence to update.</param>
/// <param name="Name">The new name for the fence.</param>
/// <param name="BackgroundColor">The new background color in hexadecimal format.</param>
/// <param name="Opacity">The new opacity percentage.</param>
public record UpdateFenceCommand(string FenceId, string Name, string BackgroundColor, int Opacity)
  : ICommand<Fence>, IRequest<Fence>;
