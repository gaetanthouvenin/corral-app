// ------------------------------------------------------------------------------------------------
// <copyright file="ReorderFenceItemsCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

using MediatR;

namespace Corral.Application.Commands.ReorderFenceItems;

/// <summary>
///   Handler for ReorderFenceItemsCommand.
/// </summary>
public class ReorderFenceItemsCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<ReorderFenceItemsCommand, Fence>
{
  #region Implementation of IRequestHandler<ReorderFenceItemsCommand,Fence>

  /// <summary>
  ///   Handles the item reorder request for a fence.
  /// </summary>
  public async Task<Fence> Handle(
    ReorderFenceItemsCommand request,
    CancellationToken cancellationToken)
  {
    var fenceId = FenceId.Create(request.FenceId);
    var fence = await unitOfWork.Fences.GetByIdAsync(fenceId, cancellationToken)
                ?? throw new InvalidOperationException(
                  $"Fence with ID '{request.FenceId}' not found"
                );

    fence.ReorderItems(request.ItemId, request.TargetItemId);

    await unitOfWork.Fences.UpdateAsync(fence, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return fence;
  }

  #endregion
}
