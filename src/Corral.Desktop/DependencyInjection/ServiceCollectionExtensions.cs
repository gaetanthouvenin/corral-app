// ------------------------------------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using Corral.Desktop.Mappers;
using Corral.Desktop.Services;
using Corral.Desktop.ViewModels;
using Corral.Desktop.Views;
using Corral.Domain.Aggregates;
using Corral.Domain.Contracts.Mappers;

using Microsoft.Extensions.DependencyInjection;

namespace Corral.Desktop.DependencyInjection;

/// <summary>
///   Extension methods for registering Presentation layer services in the dependency injection
///   container.
/// </summary>
public static class ServiceCollectionExtensions
{
  #region Methods

  /// <summary>
  ///   Registers all Presentation layer services including mappers, view models, and windows.
  /// </summary>
  /// <param name="services">The service collection to extend.</param>
  /// <returns>The extended service collection for method chaining.</returns>
  public static IServiceCollection AddPresentationServices(this IServiceCollection services)
  {
    // Register services
    services.AddScoped<IDialogService, DialogService>();
    services.AddSingleton<IOverlayService, OverlayService>();
    services.AddSingleton<ITrayIconService, TrayIconService>();
    services.AddSingleton<IIconService, IconService>();
    services.AddSingleton<IUserPreferencesService, UserPreferencesService>();

    // Register mappers
    services.AddScoped<IMapper<Fence, FenceViewModel>, FenceToViewModelMapper>();

    // Register view models
    services.AddScoped<MainWindowViewModel>();
    services.AddScoped<ZonesViewModel>();
    services.AddTransient<SettingsViewModel>();
    services.AddTransient<CreateZoneDialogViewModel>();
    services.AddTransient<EditZoneDialogViewModel>();

    // Register windows and views with dependency injection
    services.AddScoped<MainWindow>();
    services.AddScoped<ZonesView>();
    services.AddTransient<SettingsView>();
    services.AddTransient<CreateZoneDialog>();
    services.AddTransient<EditZoneDialog>();

    return services;
  }

  #endregion
}
