// ------------------------------------------------------------------------------------------------
// <copyright file="IUnitOfWork.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Contracts.Repositories;

namespace Corral.Domain.Contracts.UnitOfWork;

/// <summary>
///   Unit of Work pattern contract for managing transactions and repositories.
/// </summary>
/// <remarks>
///   This interface defines the contract for transaction management and repository access.
///   Implementation resides in the Infrastructure layer, following Dependency Inversion.
///   The Unit of Work pattern ensures that all changes to an aggregate are saved together
///   in a single transaction, maintaining data consistency.
/// </remarks>
public interface IUnitOfWork : IAsyncDisposable
{
  #region Properties

  /// <summary>
  ///   Gets the repository for Fence aggregates.
  /// </summary>
  IFenceRepository Fences { get; }

  /// <summary>
  ///   Gets the repository for the singleton UserSettings aggregate.
  /// </summary>
  IUserSettingsRepository UserSettings { get; }

  #endregion

  #region Methods

  /// <summary>
  ///   Saves all changes made to aggregates in the current unit of work to the database.
  /// </summary>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <returns>The number of aggregates persisted.</returns>
  /// <remarks>
  ///   This method commits all pending changes from added, updated, or deleted aggregates.
  /// </remarks>
  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Begins a new database transaction.
  /// </summary>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <remarks>
  ///   After calling this method, all database operations are part of a single transaction
  ///   until CommitTransactionAsync or RollbackTransactionAsync is called.
  /// </remarks>
  Task BeginTransactionAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Commits the current transaction and saves all changes.
  /// </summary>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <remarks>
  ///   If an error occurs, the transaction is automatically rolled back.
  /// </remarks>
  Task CommitTransactionAsync(CancellationToken cancellationToken = default);

  /// <summary>
  ///   Rolls back the current transaction, discarding all changes.
  /// </summary>
  /// <param name="cancellationToken">Token to cancel the operation.</param>
  /// <remarks>
  ///   Use this when you need to undo all changes made since BeginTransactionAsync was called.
  /// </remarks>
  Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

  #endregion
}
