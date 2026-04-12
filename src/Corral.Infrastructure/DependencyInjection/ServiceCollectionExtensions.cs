// ------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Mappers;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Infrastructure.Mappers;
using Corral.Infrastructure.Persistence;
using Corral.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnitOfWorkImpl = Corral.Infrastructure.UnitOfWork.UnitOfWork;

namespace Corral.Infrastructure.DependencyInjection;

/// <summary>
///   Extension methods for registering Infrastructure layer services in the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
  /// <summary>
  ///   Registers all Infrastructure layer services including database context, mappers, and unit of work.
  /// </summary>
  /// <param name="services">The service collection to extend.</param>
  /// <param name="connectionString">The database connection string.</param>
  /// <returns>The extended service collection for method chaining.</returns>
  public static IServiceCollection AddInfrastructureServices(
    this IServiceCollection services,
    string connectionString)
  {
    // Register database context
    services.AddDbContext<CorralDbContext>(options =>
      options.UseSqlite(connectionString));

    // Register mappers
    services.AddScoped<IMapper<FenceEntity, Fence>, FenceEntityToDomainMapper>();

    // Register Unit of Work
    services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();

    return services;
  }
}
