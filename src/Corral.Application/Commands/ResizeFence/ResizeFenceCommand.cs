// ------------------------------------------------------------------------------------------------
// <copyright file="ResizeFenceCommand.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.CQRS;

using MediatR;

namespace Corral.Application.Commands.ResizeFence;

/// <summary>
///   Command to resize a fence to new dimensions.
/// </summary>
/// <remarks>
///   This command changes only the dimensions of an existing fence.
///   For position changes, use MoveFenceCommand.
/// </remarks>
/// <param name="FenceId">The ID of the fence to resize.</param>
/// <param name="NewWidth">The new width in pixels.</param>
/// <param name="NewHeight">The new height in pixels.</param>
public record ResizeFenceCommand(string FenceId, int NewWidth, int NewHeight)
  : ICommand<Fence>, IRequest<Fence>;
