// ------------------------------------------------------------------------------------------------
// <copyright file="UpdateFenceCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

using MediatR;

namespace Corral.Application.Commands.UpdateFence;

/// <summary>
///   Handler for UpdateFenceCommand.
///   Orchestrates the update of an existing fence's properties.
/// </summary>
/// <remarks>
///   This handler:
///   1. Loads the existing fence from the repository
///   2. Applies updates to the fence via its domain methods
///   3. Persists the changes through Unit of Work
///   4. Returns the updated fence
/// </remarks>
public class UpdateFenceCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<UpdateFenceCommand, Fence>
{
  #region Implementation of IRequestHandler<UpdateFenceCommand,Fence>

  /// <summary>
  ///   Handles the command to update a fence.
  /// </summary>
  /// <param name="request">The UpdateFenceCommand containing the fence ID and update parameters.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>The updated fence.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the fence is not found.</exception>
  public async Task<Fence> Handle(UpdateFenceCommand request, CancellationToken cancellationToken)
  {
    // Load the existing fence
    var fenceId = FenceId.Create(request.FenceId);
    var fence = await unitOfWork.Fences.GetByIdAsync(fenceId, cancellationToken)
                ?? throw new InvalidOperationException(
                  $"Fence with ID '{request.FenceId}' not found"
                );

    // Apply updates to the fence
    fence.Rename(request.Name);

    var color = Color.FromHexString(request.BackgroundColor);
    fence.ChangeColor(color);

    var opacity = new Opacity(request.Opacity);
    fence.ChangeOpacity(opacity);

    // Update the fence in the repository
    unitOfWork.Fences.Update(fence);

    // Persist changes
    await unitOfWork.SaveChangesAsync(cancellationToken);

    // Return the updated fence
    return fence;
  }

  #endregion
}
