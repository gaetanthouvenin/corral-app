using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Corral.Application.Commands.CreateFence;
using Corral.Infrastructure.DependencyInjection;
using Corral.Infrastructure.Persistence;
using Corral.Domain.Contracts.UnitOfWork;
using MediatR;

Console.WriteLine("=== Corral Database Diagnostic Tool ===\n");

// Setup DI
var services = new ServiceCollection();
services.AddDbContext<CorralDbContext>(options =>
  options.UseSqlite("Data Source=Corral-test.db"));
services.AddMediatR(cfg =>
{
  cfg.RegisterServicesFromAssemblyContaining<CreateFenceCommand>();
});
services.AddInfrastructureServices("Data Source=Corral-test.db");

var serviceProvider = services.BuildServiceProvider();

// Apply migrations
Console.WriteLine("1. Applying migrations...");
try
{
  var dbContext = serviceProvider.GetRequiredService<CorralDbContext>();
  dbContext.Database.Migrate();
  Console.WriteLine("   ✓ Migrations applied successfully\n");
}
catch (Exception ex)
{
  Console.WriteLine($"   ✗ Error: {ex.Message}\n");
  Environment.Exit(1);
}

// Test seeding
Console.WriteLine("2. Testing fence creation...");
try
{
  var mediator = serviceProvider.GetRequiredService<IMediator>();
  var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

  var command = new CreateFenceCommand("Test Zone", 100, 100, 500, 400, "#FF0078D4", 85);
  Console.WriteLine($"   Creating fence: {command.Name}...");
  var result = mediator.Send(command, CancellationToken.None).GetAwaiter().GetResult();
  Console.WriteLine($"   ✓ Fence created: {result.Name} (ID: {result.Id.Value})\n");
}
catch (Exception ex)
{
  Console.WriteLine($"   ✗ Error: {ex.GetType().Name}: {ex.Message}");
  Console.WriteLine($"   Stack trace: {ex.StackTrace}\n");
  if (ex.InnerException != null)
  {
    Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
    Console.WriteLine($"   Inner stack trace: {ex.InnerException.StackTrace}\n");
  }
  Environment.Exit(1);
}

// Verify data
Console.WriteLine("3. Verifying data in database...");
try
{
  using (var connection = new SqliteConnection("Data Source=Corral-test.db"))
  {
    connection.Open();

    using (var command = connection.CreateCommand())
    {
      command.CommandText = "SELECT COUNT(*) FROM Fences;";
      int count = (int)(long)command.ExecuteScalar();
      Console.WriteLine($"   ✓ Total fences in database: {count}\n");

      if (count > 0)
      {
        command.CommandText = "SELECT Id, Name, BackgroundColor, Opacity FROM Fences;";
        using (var reader = command.ExecuteReader())
        {
          while (reader.Read())
          {
            Console.WriteLine($"   - {reader.GetString(1)} ({reader.GetString(2)}, {reader.GetInt32(3)}%)");
          }
        }
      }
    }
  }
}
catch (Exception ex)
{
  Console.WriteLine($"   ✗ Error: {ex.Message}");
}

Console.WriteLine("\n=== Diagnostic Complete ===");
