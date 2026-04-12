// ------------------------------------------------------------------------------------------------
// <copyright file="CorralDbContextFactory.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Corral.Infrastructure.Persistence;

/// <summary>
///   Design-time DbContext factory for EF Core migrations.
/// </summary>
public class CorralDbContextFactory : IDesignTimeDbContextFactory<CorralDbContext>
{
  /// <summary>
  ///   Creates a new instance of the CorralDbContext.
  /// </summary>
  public CorralDbContext CreateDbContext(string[] args)
  {
    const string connectionString = "Data Source=Corral.db";
    var optionsBuilder = new DbContextOptionsBuilder<CorralDbContext>();
    optionsBuilder.UseSqlite(connectionString);

    return new CorralDbContext(optionsBuilder.Options);
  }
}
