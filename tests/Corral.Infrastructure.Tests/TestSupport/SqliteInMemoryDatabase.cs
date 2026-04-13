// ------------------------------------------------------------------------------------------------
// <copyright file="SqliteInMemoryDatabase.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Infrastructure.Persistence;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Corral.Infrastructure.Tests.TestSupport;

public sealed class SqliteInMemoryDatabase : IDisposable, IAsyncDisposable
{
  #region Fields

  private readonly SqliteConnection _connection = new("Data Source=:memory:");

  #endregion

  #region Ctors

  public SqliteInMemoryDatabase()
  {
    _connection.Open();

    using var dbContext = CreateDbContext();
    dbContext.Database.EnsureCreated();
  }

  #endregion

  #region Methods

  public CorralDbContext CreateDbContext()
  {
    var options = new DbContextOptionsBuilder<CorralDbContext>().UseSqlite(_connection).Options;

    var dbContext = new CorralDbContext(options);
    dbContext.Database.EnsureCreated();
    return dbContext;
  }

  #endregion

  #region Implementation of IAsyncDisposable

  public async ValueTask DisposeAsync()
  {
    await _connection.DisposeAsync();
  }

  #endregion

  #region Implementation of IDisposable

  public void Dispose()
  {
    _connection.Dispose();
  }

  #endregion
}
