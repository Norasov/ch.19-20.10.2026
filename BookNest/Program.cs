using Npgsql;
using System.Diagnostics;

// var builder = new NpgsqlConnectionStringBuilder();
// builder.Host = "localhost";
// builder.Port = 5432;
// builder.Database = "BookNest";
// builder.Username = "postgres";
// builder.Password = "p000";

try
{
    // System.Console.WriteLine($"Host: {builder.Host}");
    // System.Console.WriteLine($"Database: {builder.Database}");

    using var connection = Database.GetConnection();
    // Stopwatch stopwatch = new();
    // stopwatch.Start();
    // connection.Open();
    // stopwatch.Stop();
    // System.Console.WriteLine($"Connection opened in {stopwatch.ElapsedMilliseconds} ms");



    System.Console.WriteLine("Connected to Postgresql. State: " + connection.State);

    if (!Database.TestConnection())
    {
        System.Console.WriteLine("Cannot continue without a database.");
        return;
    }

    var command = @"create table if not exists books(
    id serial primary key,
    title varchar(200) not null,
    published_year int,
    total_copies int not null default 1

)";

    var command1 = new NpgsqlCommand(command, connection);
    command1.ExecuteNonQuery();
    System.Console.WriteLine("Table 'books' is ready.");
    /*
        var insert = @"insert into books(title,published_year,total_copies) values
        ('Clean Code',2008,3),
        ('The Pragmatic Programmer',1999,2),
        ('Refactoring',1999,1)";

        var command2 = new NpgsqlCommand(insert,connection);
        int addRows = command2.ExecuteNonQuery();
        System.Console.WriteLine("inserted "+addRows+" book(s)");
    */

    var insert = @"select count(*) from books";
    var command3 = new NpgsqlCommand(insert, connection);
    long count = (long)command3.ExecuteScalar()!;
    System.Console.WriteLine($"Total books in database: {count}");

    string delete = @"delete from books where id = 4";
    var cmd = new NpgsqlCommand(delete,connection);
    cmd.ExecuteNonQuery();

    var select = @"select id, title, published_year, total_copies from books order by id";
    var command4 = new NpgsqlCommand(select, connection);
    var reader = command4.ExecuteReader();

    System.Console.WriteLine();
    for (int i = 0; i < reader.FieldCount; i++)
    {
        System.Console.WriteLine(reader.GetName(i));
    }
    System.Console.WriteLine();

    while (reader.Read())
    {
        int id = reader.GetInt32(0);
        string title = reader.GetString(1);
        int copies = reader.GetInt32(3);
        string year;
        if (reader.IsDBNull(2))
        {
            year = "unknown";
        }
        else
        {
            year = reader.GetInt32(2).ToString();
        }
        System.Console.WriteLine($"#{id} - {title} ({year}) - copies: {copies}");
    }

    Schema schema = new();
    schema.CreateTables();
    schema.InsertBooks();
    schema.InsertGenres();

    schema.ShowDashboard();

}
catch (NpgsqlException)
{
    System.Console.WriteLine("Не удалось подключиться к серверу");
}

