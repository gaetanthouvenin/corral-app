using Microsoft.Data.Sqlite;

string dbPath = @"C:\Users\gatan\Sources\GitHub\corral-app\src\Corral.Desktop\Corral.db";
string connectionString = $"Data Source={dbPath}";

Console.WriteLine("=== Checking Main Application Database ===\n");

try
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM Fences;";
            int count = (int)(long)command.ExecuteScalar();
            Console.WriteLine($"Total fences in database: {count}\n");

            if (count > 0)
            {
                command.CommandText = "SELECT Id, Name, BackgroundColor, Opacity, CreatedAt FROM Fences ORDER BY CreatedAt;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetString(0);
                        var name = reader.GetString(1);
                        var color = reader.GetString(2);
                        var opacity = reader.GetInt32(3);
                        var createdAt = reader.GetString(4);

                        Console.WriteLine($"  ID: {id.Substring(0, 8)}...");
                        Console.WriteLine($"  Name: {name}");
                        Console.WriteLine($"  Color: {color} ({opacity}%)");
                        Console.WriteLine($"  Created: {createdAt}\n");
                    }
                }
            }
            else
            {
                Console.WriteLine("⚠️  No fences found in database!\n");
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
