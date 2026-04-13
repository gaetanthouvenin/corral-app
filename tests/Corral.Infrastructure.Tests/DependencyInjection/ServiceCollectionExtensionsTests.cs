// ------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensionsTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Mappers;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Infrastructure.DependencyInjection;
using Corral.Infrastructure.Mappers;
using Corral.Infrastructure.Persistence;
using Corral.Infrastructure.Persistence.Entities;

using Microsoft.Extensions.DependencyInjection;

using UnitOfWorkImpl = Corral.Infrastructure.UnitOfWork.UnitOfWork;

namespace Corral.Infrastructure.Tests.DependencyInjection;

public class ServiceCollectionExtensionsTests
{
  #region Methods

  [Fact]
  public async Task AddInfrastructureServices_ShouldRegisterInfrastructureDependencies()
  {
    var services = new ServiceCollection();

    services.AddInfrastructureServices("Data Source=:memory:");

    await using var provider = services.BuildServiceProvider();
    await using var scope = provider.CreateAsyncScope();

    scope.ServiceProvider.GetRequiredService<CorralDbContext>().ShouldNotBeNull();
    scope.ServiceProvider.GetRequiredService<IMapper<FenceEntity, Fence>>()
         .ShouldBeOfType<FenceEntityToDomainMapper>();

    scope.ServiceProvider.GetRequiredService<IUnitOfWork>().ShouldBeOfType<UnitOfWorkImpl>();
  }

  #endregion
}
