// ------------------------------------------------------------------------------------------------
// <copyright file="CreateFenceCommandHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Domain.ValueObjects;

using MediatR;

namespace Corral.Application.Commands.CreateFence;

/// <summary>
///   Handler for CreateFenceCommand.
///   Orchestrates the creation of a new fence in the system.
/// </summary>
/// <remarks>
///   This handler:
///   1. Receives a validated CreateFenceCommand
///   2. Creates Domain Value Objects and Aggregate Root
///   3. Persists the fence through the repository
///   4. Maps the result to a DTO for the response
///   All business logic resides in the Domain layer (Fence aggregate).
///   This handler focuses on orchestration and persistence.
/// </remarks>
/// <remarks>
///   Initializes the handler with required dependencies.
/// </remarks>
/// <param name="unitOfWork">Unit of Work for managing transactions and repositories.</param>
/// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
public class CreateFenceCommandHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<CreateFenceCommand, Fence>
{
  #region Implementation of IRequestHandler<CreateFenceCommand,Fence>

  /// <summary>
  ///   Handles the command to create a new fence.
  /// </summary>
  /// <param name="request">The CreateFenceCommand containing fence creation parameters.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>The newly created Fence aggregate.</returns>
  /// <remarks>
  ///   The handler performs the following steps:
  ///   1. Constructs Value Objects from command parameters
  ///   2. Uses Fence.Create() factory method to create the Aggregate Root
  ///   3. Persists the fence via the repository
  ///   4. Saves changes through Unit of Work
  ///   5. Returns the Domain Fence aggregate directly
  /// </remarks>
  public async Task<Fence> Handle(CreateFenceCommand request, CancellationToken cancellationToken)
  {
    // Create Domain Value Objects
    var position = new Position(request.PositionX, request.PositionY);
    var dimensions = new Dimensions(request.Width, request.Height);
    var color = Color.FromHexString(request.BackgroundColor);
    var opacity = new Opacity(request.Opacity);

    // Create the Aggregate Root using the factory method
    // This method encapsulates all domain validation and event creation
    var fence = Fence.Create(request.Name, position, dimensions, color, opacity);

    // Add the fence to the repository (marks as added but doesn't persist yet)
    unitOfWork.Fences.Add(fence);

    // Persist all changes to the database
    await unitOfWork.SaveChangesAsync(cancellationToken);

    // Return the Domain Fence aggregate directly
    return fence;
  }

  #endregion
}
