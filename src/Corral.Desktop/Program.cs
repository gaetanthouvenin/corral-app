// ------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Gaëtan THOUVENIN">
//   Copyright (c) Gaëtan THOUVENIN. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------------------------------

using System.Diagnostics;

using Corral.Application.Commands.CreateFence;
using Corral.Application.DependencyInjection;
using Corral.Desktop.DependencyInjection;
using Corral.Domain.Contracts.UnitOfWork;
using Corral.Infrastructure.DependencyInjection;
using Corral.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Serilog;
using Serilog.Events;

namespace Corral.Desktop;

/// <summary>
///   Entry point for the Corral WPF application.
/// </summary>
internal static class Program
{
  #region Methods

  /// <summary>
  ///   Main entry point for WPF application.
  /// </summary>
  [STAThread]
  public static void Main()
  {
    Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
                                          .MinimumLevel
                                          .Override(
                                            "Microsoft.EntityFrameworkCore",
                                            LogEventLevel.Warning
                                          )
                                          .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                          .Enrich.FromLogContext()
                                          .WriteTo
                                          .Console(
                                            outputTemplate:
                                            "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}  {Message:lj}{NewLine}{Exception}"
                                          )
                                          .WriteTo.File(
                                            "logs/corral-.log",
                                            rollingInterval: RollingInterval.Day,
                                            outputTemplate:
                                            "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}  {Message:lj}{NewLine}{Exception}"
                                          )
                                          .CreateLogger();

    try
    {
      Log.Information("Démarrage de Corral Manager");

      var services = new ServiceCollection();
      services.AddLogging(logging => logging.AddSerilog(Log.Logger, true));
      ConfigureServices(services);
      var serviceProvider = services.BuildServiceProvider();

      ApplyMigrations(serviceProvider);
      SeedTestData(serviceProvider);

      var app = new App { ServiceProvider = serviceProvider };
      app.Run();
    }
    catch (Exception ex)
    {
      Log.Fatal(ex, "Arrêt inattendu de l'application");
    }
    finally
    {
      Log.CloseAndFlush();
    }
  }

  /// <summary>
  ///   Applies all pending EF Core migrations to the database.
  /// </summary>
  private static void ApplyMigrations(IServiceProvider serviceProvider)
  {
    try
    {
      var dbContext = serviceProvider.GetRequiredService<CorralDbContext>();
      dbContext.Database.Migrate();
      Debug.WriteLine("[Migration] Database migrations applied successfully.");
    }
    catch (Exception ex)
    {
      Debug.WriteLine($"[Migration] ERROR: {ex.GetType().Name}: {ex.Message}");
      if (ex.InnerException != null)
      {
        Debug.WriteLine($"[Migration] Inner: {ex.InnerException.Message}");
      }
    }
  }

  /// <summary>
  ///   Seeds test data into the database.
  /// </summary>
#pragma warning disable VSTHRD002
  private static void SeedTestData(IServiceProvider serviceProvider)
  {
    try
    {
      var mediator = serviceProvider.GetRequiredService<IMediator>();
      var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

      // Check if data already exists
      var existingFences =
        unitOfWork.Fences.GetAllAsync(CancellationToken.None).GetAwaiter().GetResult();

      if (existingFences.Count > 0)
      {
        Debug.WriteLine(
          $"[Seed] Database already contains {existingFences.Count} fence(s). Skipping seed."
        );

        return; // Data already seeded
      }

      Debug.WriteLine("[Seed] Database is empty. Creating test zones...");

      // Create test zones
      // Note: Colors must be in #AARRGGBB format (9 chars with alpha channel)
      var testZones = new[]
      {
        new CreateFenceCommand("Development", 100, 100, 800, 600, "#FF0078D4", 85),
        new CreateFenceCommand("Broken-Form", 950, 100, 300, 400, "#FFFF8C00", 75),
        new CreateFenceCommand("Creative Suite", 950, 520, 300, 180, "#FF9966CC", 90),
      };

      foreach (var command in testZones)
      {
        try
        {
          var result = mediator.Send(command, CancellationToken.None).GetAwaiter().GetResult();
          Debug.WriteLine($"[Seed] Created zone: {result.Name} (ID: {result.Id.Value})");
        }
        catch (Exception cmdEx)
        {
          Debug.WriteLine(
            $"[Seed] ERROR creating zone '{command.Name}': {cmdEx.GetType().Name}: {cmdEx.Message}"
          );

          Debug.WriteLine($"[Seed] Stack trace: {cmdEx.StackTrace}");
          throw;
        }
      }

      Debug.WriteLine($"[Seed] Successfully created {testZones.Length} test zones.");
    }
    catch (Exception ex)
    {
      // Log error but don't crash - seed data is not critical
      Debug.WriteLine($"[Seed] FATAL ERROR: {ex.GetType().Name}: {ex.Message}");
      Debug.WriteLine($"[Seed] Stack trace: {ex.StackTrace}");
      if (ex.InnerException != null)
      {
        Debug.WriteLine(
          $"[Seed] Inner exception: {ex.InnerException.GetType().Name}: {ex.InnerException.Message}"
        );

        Debug.WriteLine($"[Seed] Inner stack trace: {ex.InnerException.StackTrace}");
      }
    }
  }
#pragma warning restore VSTHRD002

  /// <summary>
  ///   Configures all dependency injection services for the application.
  /// </summary>
  /// <param name="services">The service collection to configure.</param>
  private static void ConfigureServices(IServiceCollection services)
  {
    // Infrastructure — base de données, repositories, unit of work
    const string connectionString = "Data Source=Corral.db";
    services.AddInfrastructureServices(connectionString);

    // Application — MediatR, behaviors (logging + validation), validators FluentValidation
    services.AddApplicationServices();

    // Présentation — ViewModels, mappers, fenêtres, services dialog
    services.AddPresentationServices();
  }

  #endregion
}
