using Npgsql;

public class Schema
{
    public void CreateTables()
    {
        using var connection = Database.GetConnection();
        var create = @"create table if not exists genres (
        id serial primary key,
        name varchar(100) not null unique
        );";
        using var cmd = new NpgsqlCommand(create, connection);
        cmd.ExecuteNonQuery();
    }

    public void InsertGenres()
    {
        using var connection = Database.GetConnection();
        var select = @"select count(*) from genres";
        using var cmd1 = new NpgsqlCommand(select, connection);
        long count = (long)cmd1.ExecuteScalar()!;

        if (count == 0)
        {
            var insert = @"insert into genres(name) values
            ('Programming'),
            ('Software Design'),
            ('Databases'),
            ('Fiction'),
            ('History')";

            using var cmd = new NpgsqlCommand(insert, connection);
            int cnt = cmd.ExecuteNonQuery();
            System.Console.WriteLine($"Genres seeded: {cnt} row(s) inserted");
        }
        else
        {
            System.Console.WriteLine("Genres table already has data — skipping seed.");
        }

    }

    public void GetGenres()
    {
        using var connection = Database.GetConnection();
        var select = @"select * from genres";
        using var cmd = new NpgsqlCommand(select, connection);
        using var reader = cmd.ExecuteReader();
        System.Console.WriteLine("Genres: ");

        while (reader.Read())
        {
            System.Console.WriteLine(reader[0] + ". " + reader[1]);
        }
    }


    public void InsertBooks()
    {
        using var connection = Database.GetConnection();

        var select = "SELECT COUNT(*) FROM books";
        using var cmd1 = new NpgsqlCommand(select, connection);

        long count = (long)cmd1.ExecuteScalar()!;

        if (count == 0)
        {
            var insert = @"insert into books (title, author, published_year)
        values
        ('Book 1', 'Author 1', 2020),
        ('Book 2', 'Author 2', 2021)";

            using var cmd2 = new NpgsqlCommand(insert, connection);
            int inserted = cmd2.ExecuteNonQuery();

            System.Console.WriteLine($"Books seeded: {inserted} row(s) inserted.");
        }
        else
        {
            System.Console.WriteLine("Books table already has data — skipping seed.");
        }
    }


    public void ShowDashboard()
    {
        using var connection = Database.GetConnection();

        using var cmd1 = new NpgsqlCommand(
            "select count(*) from books", connection);
        long totalBooks = (long)cmd1.ExecuteScalar()!;

        using var cmd2 = new NpgsqlCommand(
            "select coalesce (SUM(total_copies), 0) from books", connection);
        long totalCopies = (long)cmd2.ExecuteScalar()!;

        using var cmd3 = new NpgsqlCommand(
            "select min(published_year) from books", connection);
        var oldestResult = cmd3.ExecuteScalar();
        string oldestBook = Convert.IsDBNull(oldestResult)
            ? "N/A"
            : oldestResult!.ToString()!;

        using var cmd4 = new NpgsqlCommand(
            "select max(published_year) from books", connection);
        var newestResult = cmd4.ExecuteScalar();
        string newestBook = Convert.IsDBNull(newestResult)
            ? "N/A"
            : newestResult!.ToString()!;

        using var cmd5 = new NpgsqlCommand(
            "select count(*) from genres", connection);
        long totalGenres = (long)cmd5.ExecuteScalar()!;

        System.Console.WriteLine("\n===== BookNest Dashboard =====");
        System.Console.WriteLine($"Total books:      {totalBooks}");
        System.Console.WriteLine($"Total copies:     {totalCopies}");
        System.Console.WriteLine($"Oldest book:      {oldestBook}");
        System.Console.WriteLine($"Newest book:      {newestBook}");
        System.Console.WriteLine($"Total genres:     {totalGenres}");
        System.Console.WriteLine("==============================");
    }

}