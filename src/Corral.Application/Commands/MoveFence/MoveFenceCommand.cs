// ------------------------------------------------------------------------------------------------
// <copyright file="MoveFenceCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.MoveFence;

/// <summary>
///   Command to move a fence to a new position on the screen.
/// </summary>
/// <remarks>
///   This command changes only the position of an existing fence.
///   For other property updates (color, opacity, etc.), use UpdateFenceCommand.
/// </remarks>
/// <param name="FenceId">The ID of the fence to move.</param>
/// <param name="NewPositionX">The new horizontal position (X coordinate) in pixels.</param>
/// <param name="NewPositionY">The new vertical position (Y coordinate) in pixels.</param>
public record MoveFenceCommand(string FenceId, int NewPositionX, int NewPositionY)
  : ICommand<Fence>, IRequest<Fence>;
