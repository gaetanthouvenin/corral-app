// ------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWork.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
#nullable enable

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Mappers;
using Corral.Domain.Contracts.Repositories;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Infrastructure.Persistence;
using Corral.Infrastructure.Persistence.Entities;
using Corral.Infrastructure.Repositories;

namespace Corral.Infrastructure.UnitOfWork;

/// <summary>
///   Provides an implementation of the Unit of Work pattern.
/// </summary>
/// <remarks>
///   The Unit of Work pattern is used to group multiple operations into a single transaction.
///   This implementation ensures that changes are committed or rolled back as a single unit.
/// </remarks>
public class UnitOfWork(
  CorralDbContext dbContext,
  IMapper<FenceEntity, Fence> fenceMapper) : IUnitOfWork
{
  #region Properties

  /// <summary>
  ///   Gets the repository for managing fences.
  /// </summary>
  public IFenceRepository Fences => field ??= new FenceRepository(dbContext, fenceMapper);

  #endregion

  #region Implementation of IAsyncDisposable

  /// <summary>
  ///   Disposes the database context asynchronously.
  /// </summary>
  /// <returns>A task that represents the asynchronous dispose operation.</returns>
  public async ValueTask DisposeAsync()
  {
    await dbContext.DisposeAsync();
  }

  #endregion

  #region Implementation of IUnitOfWork

  /// <summary>
  ///   Saves all changes made in the current transaction.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>
  ///   A task that represents the asynchronous save operation. The task result contains the
  ///   number of state entries written to the database.
  /// </returns>
  public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await dbContext.SaveChangesAsync(cancellationToken);
  }

  /// <summary>
  ///   Begins a new database transaction.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
  {
    await dbContext.Database.BeginTransactionAsync(cancellationToken);
  }

  /// <summary>
  ///   Commits the current database transaction.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  /// <remarks>
  ///   If an error occurs during the commit, the transaction is rolled back.
  /// </remarks>
  public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      await dbContext.SaveChangesAsync(cancellationToken);
      await dbContext.Database.CommitTransactionAsync(cancellationToken);
    }
    catch
    {
      await RollbackTransactionAsync(cancellationToken);
      throw;
    }
  }

  /// <summary>
  ///   Rolls back the current database transaction.
  /// </summary>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>A task that represents the asynchronous operation.</returns>
  public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
  {
    await dbContext.Database.RollbackTransactionAsync(cancellationToken);
  }

  #endregion
}
