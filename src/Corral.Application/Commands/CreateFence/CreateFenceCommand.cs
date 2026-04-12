// ------------------------------------------------------------------------------------------------
// <copyright file="CreateFenceCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.CreateFence;

/// <summary>
///   Command to create a new fence in the system.
/// </summary>
/// <remarks>
///   This command contains all the required information to create a fence.
///   Validation ensures all properties are within acceptable business constraints.
///   The record implements both ICommand (Domain contract) and IRequest (MediatR integration).
/// </remarks>
/// <param name="Name">Name of the fence.</param>
/// <param name="PositionX">Horizontal position (X coordinate) in pixels.</param>
/// <param name="PositionY">Vertical position (Y coordinate) in pixels.</param>
/// <param name="Width">Width of the fence in pixels.</param>
/// <param name="Height">Height of the fence in pixels.</param>
/// <param name="BackgroundColor">Background color in hexadecimal format (ARGB).</param>
/// <param name="Opacity">Opacity percentage (0-100).</param>
public record CreateFenceCommand(
  string Name,
  int PositionX,
  int PositionY,
  int Width,
  int Height,
  string BackgroundColor,
  int Opacity) : ICommand<Fence>, IRequest<Fence>;
