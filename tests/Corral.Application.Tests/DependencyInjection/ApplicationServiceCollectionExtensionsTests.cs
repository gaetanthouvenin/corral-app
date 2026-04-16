// ------------------------------------------------------------------------------------------------
// <copyright file="ApplicationServiceCollectionExtensionsTests.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Commands.CreateFence;
using Corral.Application.DependencyInjection;
using Corral.Domain.Aggregates;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Corral.Application.Tests.DependencyInjection;

public class ApplicationServiceCollectionExtensionsTests
{
  #region Methods

  [Fact]
  public void AddApplicationServices_ShouldRegisterMediatorAndValidators()
  {
    var services = new ServiceCollection();
    services.AddLogging();

    services.AddApplicationServices();

    using var provider = services.BuildServiceProvider();
    provider.GetService<IMediator>().ShouldNotBeNull();
    provider.GetServices<IValidator<CreateFenceCommand>>().ShouldNotBeEmpty();
    provider.GetServices<IPipelineBehavior<CreateFenceCommand, Fence>>()
            .Count()
            .ShouldBeGreaterThanOrEqualTo(2);
  }

  #endregion
}
