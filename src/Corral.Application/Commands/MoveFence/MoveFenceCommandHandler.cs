// ------------------------------------------------------------------------------------------------
// <copyright file="MoveFenceCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

using MediatR;

namespace Corral.Application.Commands.MoveFence;

/// <summary>
///   Handler for MoveFenceCommand.
///   Orchestrates the movement of a fence to a new position.
/// </summary>
/// <remarks>
///   This handler:
///   1. Loads the fence by ID
///   2. Calls Move() to update position and raise FencePositionChangedEvent
///   3. Persists the changes
///   4. Returns the updated fence as a DTO
/// </remarks>
/// <remarks>
///   Initializes the handler with required dependencies.
/// </remarks>
/// <param name="unitOfWork">Unit of Work for managing transactions and repositories.</param>
/// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
public class MoveFenceCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<MoveFenceCommand, Fence>
{
  #region Implementation of IRequestHandler<MoveFenceCommand,Fence>

  /// <summary>
  ///   Handles the command to move a fence.
  /// </summary>
  /// <param name="request">The MoveFenceCommand containing fence ID and new position.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>A FenceDto representing the moved fence.</returns>
  /// <exception cref="InvalidOperationException">Thrown when the fence is not found.</exception>
  /// <remarks>
  ///   The handler performs the following steps:
  ///   1. Retrieves the fence from the repository
  ///   2. Creates a new Position value object with the new coordinates
  ///   3. Calls Move() to update the position and raise the domain event
  ///   4. Saves changes through Unit of Work
  ///   5. Maps the result to DTO and returns
  /// </remarks>
  public async Task<Fence> Handle(MoveFenceCommand request, CancellationToken cancellationToken)
  {
    // Load the fence to move
    var fenceId = FenceId.Create(request.FenceId);
    var fence = await unitOfWork.Fences.GetByIdAsync(fenceId, cancellationToken)
                ?? throw new InvalidOperationException(
                  $"Fence with ID '{request.FenceId}' not found"
                );

    // Create the new position
    var newPosition = new Position(request.NewPositionX, request.NewPositionY);

    // Move the fence (this raises FencePositionChangedEvent)
    fence.Move(newPosition);

    // Update in the repository
    unitOfWork.Fences.Update(fence);

    // Persist changes
    await unitOfWork.SaveChangesAsync(cancellationToken);

    // Map and return the moved fence
    return fence;
  }

  #endregion
}
