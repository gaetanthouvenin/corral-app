// ------------------------------------------------------------------------------------------------
// <copyright file="GetAllFencesQueryHandler.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.UnitOfWork;

using MediatR;

namespace Corral.Application.Queries.GetAllFences;

/// <summary>
///   Handler for GetAllFencesQuery.
///   Retrieves all fences in the system.
/// </summary>
/// <remarks>
///   This handler:
///   1. Queries all fences from the repository
///   2. Maps each fence from Domain to DTO
///   3. Returns the list of fences
///   This is a read-only operation with no side effects.
/// </remarks>
/// <remarks>
///   Initializes the handler with required dependencies.
/// </remarks>
/// <param name="unitOfWork">Unit of Work for accessing repositories.</param>
/// <exception cref="ArgumentNullException">Thrown when unitOfWork is null.</exception>
public class GetAllFencesQueryHandler(IUnitOfWork unitOfWork)
  : IRequestHandler<GetAllFencesQuery, List<Fence>>
{
  #region Fields

  private readonly IUnitOfWork _unitOfWork =
    unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

  #endregion

  #region Implementation of IRequestHandler<GetAllFencesQuery,List<Fence>>

  /// <summary>
  ///   Handles the query to retrieve all fences.
  /// </summary>
  /// <param name="request">The GetAllFencesQuery request.</param>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>A list of FenceDto representing all fences in the system.</returns>
  /// <remarks>
  ///   The handler performs the following steps:
  ///   1. Retrieves all fences from the repository
  ///   2. Maps each fence from Domain to DTO
  ///   3. Returns the complete list
  /// </remarks>
  public async Task<List<Fence>> Handle(
    GetAllFencesQuery request,
    CancellationToken cancellationToken)
  {
    // Load all fences from the repository
    var fences = await _unitOfWork.Fences.GetAllAsync(cancellationToken);

    // Map each fence to DTO and return
    return fences.ToList();
  }

  #endregion
}
