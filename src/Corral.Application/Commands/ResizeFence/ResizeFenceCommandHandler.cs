// ------------------------------------------------------------------------------------------------
// <copyright file="ResizeFenceCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

using MediatR;

namespace Corral.Application.Commands.ResizeFence;

/// <summary>
///   Handler for ResizeFenceCommand.
///   Orchestrates the resize of a fence to new dimensions.
/// </summary>
/// <remarks>
///   This handler:
///   1. Loads the fence by ID
///   2. Calls Resize() to update dimensions
///   3. Persists the changes
///   4. Returns the updated fence
/// </remarks>
/// <param name="unitOfWork">Unit of Work for managing transactions and repositories.</param>
public class ResizeFenceCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<ResizeFenceCommand, Fence>
{
  #region Implementation of IRequestHandler<ResizeFenceCommand,Fence>

  /// <summary>
  ///   Handles the command to resize a fence.
  /// </summary>
  /// <param name="request">The ResizeFenceCommand containing fence ID and new dimensions.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>The resized fence.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the fence is not found.</exception>
  public async Task<Fence> Handle(ResizeFenceCommand request, CancellationToken cancellationToken)
  {
    var fenceId = FenceId.Create(request.FenceId);
    var fence = await unitOfWork.Fences.GetByIdAsync(fenceId, cancellationToken)
                ?? throw new InvalidOperationException(
                  $"Fence with ID '{request.FenceId}' not found"
                );

    var newDimensions = Dimensions.Create(request.NewWidth, request.NewHeight);
    fence.Resize(newDimensions);

    await unitOfWork.Fences.UpdateAsync(fence, cancellationToken);
    await unitOfWork.SaveChangesAsync(cancellationToken);

    return fence;
  }

  #endregion
}
