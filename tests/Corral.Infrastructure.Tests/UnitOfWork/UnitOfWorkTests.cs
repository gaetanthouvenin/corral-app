// ------------------------------------------------------------------------------------------------
// <copyright file="UnitOfWorkTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Contracts.Repositories;
using Corral.Infrastructure.Mappers;
using Corral.Infrastructure.Tests.TestSupport;

using Microsoft.EntityFrameworkCore;

using UnitOfWorkImpl = Corral.Infrastructure.UnitOfWork.UnitOfWork;

namespace Corral.Infrastructure.Tests.UnitOfWork;

public class UnitOfWorkTests
{
  #region Fields

  private readonly FenceEntityToDomainMapper _mapper = new();

  #endregion

  #region Methods

  [Fact]
  public async Task SaveChangesAsync_ShouldPersistFenceAddedThroughRepository()
  {
    await using var database = new SqliteInMemoryDatabase();
    await using var dbContext = database.CreateDbContext();
    var unitOfWork = new UnitOfWorkImpl(dbContext, _mapper);

    unitOfWork.Fences.Add(FenceTestData.CreateFence());
    await unitOfWork.SaveChangesAsync(TestContext.Current.CancellationToken);

    (await dbContext.Fences.CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
  }

  [Fact]
  public void Fences_ShouldCacheRepositoryInstance()
  {
    using var database = new SqliteInMemoryDatabase();
    using var dbContext = database.CreateDbContext();
    var unitOfWork = new UnitOfWorkImpl(dbContext, _mapper);

    var firstRepository = unitOfWork.Fences;
    var secondRepository = unitOfWork.Fences;

    firstRepository.ShouldBeSameAs(secondRepository);
    firstRepository.ShouldBeAssignableTo<IFenceRepository>();
  }

  [Fact]
  public async Task BeginTransactionAsync_ShouldStartTransaction()
  {
    await using var database = new SqliteInMemoryDatabase();
    await using var dbContext = database.CreateDbContext();
    var unitOfWork = new UnitOfWorkImpl(dbContext, _mapper);

    await unitOfWork.BeginTransactionAsync(TestContext.Current.CancellationToken);

    dbContext.Database.CurrentTransaction.ShouldNotBeNull();
    await unitOfWork.RollbackTransactionAsync(TestContext.Current.CancellationToken);
  }

  [Fact]
  public void UserSettings_ShouldCacheRepositoryInstance()
  {
    using var database = new SqliteInMemoryDatabase();
    using var dbContext = database.CreateDbContext();
    var unitOfWork = new UnitOfWorkImpl(dbContext, _mapper);

    var firstRepository = unitOfWork.UserSettings;
    var secondRepository = unitOfWork.UserSettings;

    firstRepository.ShouldBeSameAs(secondRepository);
  }

  [Fact]
  public async Task CommitTransactionAsync_ShouldPersistPendingChanges()
  {
    await using var database = new SqliteInMemoryDatabase();
    await using var dbContext = database.CreateDbContext();
    var unitOfWork = new UnitOfWorkImpl(dbContext, _mapper);

    await unitOfWork.BeginTransactionAsync(TestContext.Current.CancellationToken);
    unitOfWork.Fences.Add(FenceTestData.CreateFence());

    await unitOfWork.CommitTransactionAsync(TestContext.Current.CancellationToken);

    (await dbContext.Fences.CountAsync(TestContext.Current.CancellationToken)).ShouldBe(1);
  }

  [Fact]
  public async Task DisposeAsync_ShouldDisposeDbContext()
  {
    await using var database = new SqliteInMemoryDatabase();
    var dbContext = database.CreateDbContext();
    var unitOfWork = new UnitOfWorkImpl(dbContext, _mapper);

    await unitOfWork.DisposeAsync();

    await Should.ThrowAsync<ObjectDisposedException>(() => dbContext.Fences.CountAsync());
  }

  #endregion
}
