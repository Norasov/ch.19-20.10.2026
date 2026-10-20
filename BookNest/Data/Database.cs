using Npgsql;

public static class Database
{
    private const string connectionString = "Host=localhost;Port=5432;Database=BookNest;Username=postgres;Password=p000";
    public static NpgsqlConnection GetConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }
    public static bool TestConnection()
    {
        try
        {
            using var connection = GetConnection();
            var cmd = new NpgsqlCommand("select version()",connection);
            var version = cmd.ExecuteScalar();
            System.Console.WriteLine("PostgreSQL version: "+version);

            var cmd1 = new NpgsqlCommand("select current_database();",connection);
            var database = cmd1.ExecuteScalar();
            System.Console.WriteLine("Database: "+database);

            return true;
        }
        catch (NpgsqlException)
        {
            return false;
        }
    }
}