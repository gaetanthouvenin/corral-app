// ------------------------------------------------------------------------------------------------
// <copyright file="GetActiveFencesQueryHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;

using MediatR;

namespace Corral.Application.Queries.GetActiveFences;

/// <summary>
///   Handler for GetActiveFencesQuery.
///   Retrieves all active fences in the system.
/// </summary>
/// <remarks>
///   This handler:
///   1. Queries only active fences from the repository
///   2. Maps each fence from Domain to DTO
///   3. Returns the list of active fences
///   This is a read-only operation with no side effects.
/// </remarks>
/// <remarks>
///   Initializes the handler with required dependencies.
/// </remarks>
/// <param name="unitOfWork">Unit of Work for accessing repositories.</param>
/// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
public class GetActiveFencesQueryHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<GetActiveFencesQuery, List<Fence>>
{
  #region Fields

  private readonly IUnitOfWork _unitOfWork =
    unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

  #endregion

  #region Implementation of IRequestHandler<GetActiveFencesQuery,List<Fence>>

  /// <summary>
  ///   Handles the query to retrieve all active fences.
  /// </summary>
  /// <param name="request">The GetActiveFencesQuery request.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>A list of FenceDto representing all active fences in the system.</returns>
  /// <remarks>
  ///   The handler performs the following steps:
  ///   1. Retrieves all active fences from the repository
  ///   2. Maps each fence from Domain to DTO
  ///   3. Returns the complete list
  /// </remarks>
  public async Task<List<Fence>> Handle(
    GetActiveFencesQuery request,
    CancellationToken cancellationToken)
  {
    // Load all active fences from the repository
    var activeFences = await _unitOfWork.Fences.GetActivesAsync(cancellationToken);

    // Map each fence to DTO and return
    return activeFences.ToList();
  }

  #endregion
}
