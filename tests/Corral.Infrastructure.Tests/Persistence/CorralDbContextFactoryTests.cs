// ------------------------------------------------------------------------------------------------
// <copyright file="CorralDbContextFactoryTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Infrastructure.Persistence;

namespace Corral.Infrastructure.Tests.Persistence;

public class CorralDbContextFactoryTests
{
  #region Methods

  [Fact]
  public void CreateDbContext_ShouldCreateSqliteContext()
  {
    var factory = new CorralDbContextFactory();

    using var dbContext = factory.CreateDbContext([]);

    dbContext.ShouldNotBeNull();
    dbContext.Database.ProviderName.ShouldBe("Microsoft.EntityFrameworkCore.Sqlite");
  }

  #endregion
}
