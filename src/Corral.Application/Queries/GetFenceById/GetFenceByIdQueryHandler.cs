// ------------------------------------------------------------------------------------------------
// <copyright file="GetFenceByIdQueryHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;

using MediatR;

namespace Corral.Application.Queries.GetFenceById;

/// <summary>
///   Handler for GetFenceByIdQuery.
///   Retrieves a single fence by its identifier.
/// </summary>
/// <remarks>
///   This handler:
///   1. Queries a specific fence by ID from the repository
///   2. Maps it from Domain to DTO
///   3. Returns the fence or null if not found
///   This is a read-only operation with no side effects.
/// </remarks>
/// <remarks>
///   Initializes the handler with required dependencies.
/// </remarks>
/// <param name="unitOfWork">Unit of Work for accessing repositories.</param>
/// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
public class GetFenceByIdQueryHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<GetFenceByIdQuery, Fence>
{
  #region Fields

  private readonly IUnitOfWork _unitOfWork =
    unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

  #endregion

  #region Implementation of IRequestHandler<GetFenceByIdQuery,Fence>

  /// <summary>
  ///   Handles the query to retrieve a fence by ID.
  /// </summary>
  /// <param name="request">The GetFenceByIdQuery containing the fence ID.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>The Fence aggregate if found; otherwise null.</returns>
  /// <remarks>
  ///   The handler performs the following steps:
  ///   1. Creates a FenceId from the string provided in the query
  ///   2. Retrieves the fence from the repository
  ///   3. Returns the Domain aggregate directly
  /// </remarks>
  public async Task<Fence> Handle(GetFenceByIdQuery request, CancellationToken cancellationToken)
  {
    // Create the strongly-typed ID
    var fenceId = FenceId.Create(request.FenceId);

    // Load the fence from the repository
    var fence = await _unitOfWork.Fences.GetByIdAsync(fenceId, cancellationToken);

    // Map to DTO if found, return null otherwise
    return fence == null ? null : fence;
  }

  #endregion
}
