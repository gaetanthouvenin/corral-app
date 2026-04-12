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
                command.CommandText = "SELECT Id, Name, BackgroundColor, Opacity FROM Fences ORDER BY Name;";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var name = reader.GetString(1);
                        var color = reader.GetString(2);
                        var opacity = reader.GetInt32(3);

                        Console.WriteLine($"  {name}: {color} ({opacity}%)");
                    }
                }
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
