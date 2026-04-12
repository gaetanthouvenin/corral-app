using Microsoft.Data.Sqlite;

string dbPath = @"C:\Users\gatan\Sources\GitHub\corral-app\src\Corral.Desktop\Corral.db";
string connectionString = $"Data Source={dbPath}";

try
{
    using (var connection = new SqliteConnection(connectionString))
    {
        connection.Open();

        // Check if Fences table exists
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='Fences';";
            var result = command.ExecuteScalar();

            if (result != null)
            {
                Console.WriteLine("✓ Fences table exists");

                // Count fences
                command.CommandText = "SELECT COUNT(*) FROM Fences;";
                int count = (int)(long)command.ExecuteScalar();
                Console.WriteLine($"✓ Fences count: {count}");

                // List fences
                if (count > 0)
                {
                    command.CommandText = "SELECT Id, Name, BackgroundColor, Opacity FROM Fences;";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"  - {reader.GetString(1)} ({reader.GetString(2)}, {reader.GetInt32(3)}%)");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("✗ Fences table NOT found");
            }
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"✗ Error: {ex.Message}");
}
