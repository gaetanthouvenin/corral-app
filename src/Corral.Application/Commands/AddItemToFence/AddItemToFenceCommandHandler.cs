// ------------------------------------------------------------------------------------------------
// <copyright file="AddItemToFenceCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

using MediatR;

namespace Corral.Application.Commands.AddItemToFence;

/// <summary>
///   Handler for AddItemToFenceCommand.
///   Adds an item (shortcut, file, or link) to an existing fence.
/// </summary>
public class AddItemToFenceCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<AddItemToFenceCommand, Fence>
{
  #region Implementation of IRequestHandler<AddItemToFenceCommand,Fence>

  /// <summary>
  ///   Handles the command to add an item to a fence.
  /// </summary>
  /// <param name="request">The command containing the fence ID and item details.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>The updated fence with the new item.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the fence is not found.</exception>
  public async Task<Fence> Handle(
    AddItemToFenceCommand request,
    CancellationToken cancellationToken)
  {
    var fenceId = FenceId.Create(request.FenceId);
    var fence = await unitOfWork.Fences.GetByIdAsync(fenceId, cancellationToken)
                ?? throw new InvalidOperationException(
                  $"Fence with ID '{request.FenceId}' not found"
                );

    fence.AddItem(request.DisplayName, request.Path, (FenceItemType)request.ItemType);

    await unitOfWork.Fences.UpdateAsync(fence, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return fence;
  }

  #endregion
}
