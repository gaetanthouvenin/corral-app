// ------------------------------------------------------------------------------------------------
// <copyright file="IFenceRepository.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;

namespace Corral.Domain.Contracts.Repositories;

/// <summary>
///   Defines the contract for a repository managing <see cref="Fence" /> entities (Aggregate Root).
/// </summary>
/// <remarks>
///   This repository is responsible for the persistence and retrieval of <see cref="Fence" />
///   entities.
///   Implementations should handle communication with the data layer.
/// </remarks>
public interface IFenceRepository
{
  #region Methods

  /// <summary>
  ///   Retrieves a fence by its unique identifier.
  /// </summary>
  /// <param name="id">The unique identifier of the fence.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The fence if found; otherwise, null.</returns>
  Task<Fence> GetByIdAsync(FenceId id, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Retrieves all fences.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A list of all fences.</returns>
  Task<List<Fence>> GetAllAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Retrieves all active fences.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A list of active fences.</returns>
  Task<List<Fence>> GetActivesAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Searches fences by name using a case-insensitive substring match, server-side.
  /// </summary>
  /// <param name="searchTerm">The term to search for in fence names.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A list of fences whose name contains the search term.</returns>
  Task<List<Fence>> SearchByNameAsync(
    string searchTerm,
    CancellationToken cancellationToken = default);

  /// <summary>
  ///   Adds a new fence to the repository.
  /// </summary>
  /// <param name="fence">The fence to add.</param>
  /// <remarks>
  ///   This method does not immediately persist the fence to the database.
  ///   Persistence is handled via the Unit of Work.
  /// </remarks>
  void Add(Fence fence);

  /// <summary>
  ///   Updates an existing fence, including its items collection.
  /// </summary>
  /// <param name="fence">The fence to update.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <remarks>
  ///   Loads the tracked entity from the database and synchronizes scalar properties
  ///   and the items collection (insertions, updates, deletions). The actual SQL
  ///   is emitted when the Unit of Work calls <c>SaveChangesAsync</c>.
  /// </remarks>
  Task UpdateAsync(Fence fence, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Deletes a fence from the repository.
  /// </summary>
  /// <param name="fence">The fence to delete.</param>
  /// <remarks>
  ///   This method does not immediately persist the deletion to the database.
  ///   Persistence is handled via the Unit of Work.
  /// </remarks>
  void Delete(Fence fence);

  #endregion
}
