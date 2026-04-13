// ------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Application.Behaviors;
using Corral.Application.Commands.CreateFence;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Corral.Application.DependencyInjection;

/// <summary>
///   Extension methods pour enregistrer les services de la couche Application.
/// </summary>
public static class ServiceCollectionExtensions
{
  #region Methods

  /// <summary>
  ///   Enregistre MediatR, les behaviors de pipeline, et les validators FluentValidation.
  /// </summary>
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    // MediatR — scan de l'assembly Application pour commands, queries et handlers
    services.AddMediatR(cfg =>
                        {
                          cfg.RegisterServicesFromAssemblyContaining<CreateFenceCommand>();

                          // Ordre d'exécution : LoggingBehavior → ValidationBehavior → Handler
                          cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
                          cfg.AddBehavior(
                            typeof(IPipelineBehavior<,>),
                            typeof(ValidationBehavior<,>)
                          );
                        }
    );

    // FluentValidation — scan de l'assembly Application pour tous les validators
    services.AddValidatorsFromAssemblyContaining<CreateFenceCommand>();

    return services;
  }

  #endregion
}
