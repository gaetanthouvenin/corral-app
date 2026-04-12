// ------------------------------------------------------------------------------------------------
// <copyright file="DeleteFenceCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;

using MediatR;

namespace Corral.Application.Commands.DeleteFence;

/// <summary>
///   Handler for DeleteFenceCommand.
///   Orchestrates the deletion of a fence from the system.
/// </summary>
/// <remarks>
///   This handler:
///   1. Loads the fence by ID
///   2. Calls Delete() on the fence to raise the FenceDeletedEvent
///   3. Removes the fence from persistence
///   4. Saves changes through Unit of Work
///   5. Returns the deleted fence as a DTO
/// </remarks>
/// <remarks>
///   Initializes the handler with required dependencies.
/// </remarks>
/// <param name="unitOfWork">Unit of Work for managing transactions and repositories.</param>
/// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
public class DeleteFenceCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<DeleteFenceCommand, Fence>
{
  #region Implementation of IRequestHandler<DeleteFenceCommand,Fence>

  /// <summary>
  ///   Handles the command to delete a fence.
  /// </summary>
  /// <param name="request">The DeleteFenceCommand containing the fence ID.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>A FenceDto representing the deleted fence.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the fence is not found.</exception>
  /// <remarks>
  ///   The handler performs the following steps:
  ///   1. Retrieves the fence from the repository
  ///   2. Calls Delete() to raise the FenceDeletedEvent
  ///   3. Removes the fence from the repository
  ///   4. Saves changes through Unit of Work
  ///   5. Maps the result to DTO and returns
  /// </remarks>
  public async Task<Fence> Handle(DeleteFenceCommand request, CancellationToken cancellationToken)
  {
    // Load the fence to delete
    var fenceId = FenceId.Create(request.FenceId);
    var fence = await unitOfWork.Fences.GetByIdAsync(fenceId, cancellationToken)
                ?? throw new InvalidOperationException(
                  $"Fence with ID '{request.FenceId}' not found"
                );

    // Create a DTO copy before deletion (for response)
    var fenceDto = fence;

    // Call Delete on the fence to raise the FenceDeletedEvent
    fence.Delete();

    // Remove the fence from persistence
    unitOfWork.Fences.Delete(fence);

    // Persist changes (including the domain event)
    await unitOfWork.SaveChangesAsync(cancellationToken);

    // Return the deleted fence as DTO
    return fenceDto;
  }

  #endregion
}
