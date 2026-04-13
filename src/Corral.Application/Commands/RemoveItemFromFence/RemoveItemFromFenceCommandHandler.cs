// ------------------------------------------------------------------------------------------------
// <copyright file="RemoveItemFromFenceCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;

using MediatR;

namespace Corral.Application.Commands.RemoveItemFromFence;

/// <summary>
///   Handler for RemoveItemFromFenceCommand.
///   Removes an item from an existing fence.
/// </summary>
public class RemoveItemFromFenceCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<RemoveItemFromFenceCommand, Fence>
{
  #region Implementation of IRequestHandler<RemoveItemFromFenceCommand,Fence>

  /// <summary>
  ///   Handles the command to remove an item from a fence.
  /// </summary>
  /// <param name="request">The command containing the fence and item IDs.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>The updated fence without the removed item.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the fence is not found.</exception>
  public async Task<Fence> Handle(
    RemoveItemFromFenceCommand request,
    CancellationToken cancellationToken)
  {
    var fenceId = FenceId.Create(request.FenceId);
    var fence = await unitOfWork.Fences.GetByIdAsync(fenceId, cancellationToken)
                ?? throw new InvalidOperationException(
                  $"Fence with ID '{request.FenceId}' not found"
                );

    fence.RemoveItem(request.ItemId);

    await unitOfWork.Fences.UpdateAsync(fence, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return fence;
  }

  #endregion
}
