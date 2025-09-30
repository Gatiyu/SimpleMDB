
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;
namespace SimpleMDB;


public class MySqlActorRepository : IActorRepository
{
    private string connectionString;

    public MySqlActorRepository(string connectionString)
    {
        this.connectionString = connectionString;
        Init();
    }

    private void Init()
    {
         using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS Actors
        (
            id int AUTO_INCREMENT PRIMARY KEY,
            firstname NVARCHAR(64) NOT NULL,
            lastname NVARCHAR(64) NOT NULL,
            bio NVARCHAR(4096),
            rating float
        )
        ";
        cmd.ExecuteNonQuery();
    }

    public MySqlConnection OpenDb()
    {
        var dbc = new MySqlConnection(connectionString);
        dbc.Open();
        return dbc;
    }

    public async Task<PagedResult<Actor>> ReadAll(int page, int size)
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM Actors";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Actors LIMIT @offset, @limit";
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();
        var actors = new List<Actor>();

        while (await rows.ReadAsync())
        {
            actors.Add(new Actor
            {
                Id = rows.GetInt32("id"),
                FirstName = rows.GetString("firstname"),
                LastName = rows.GetString("lastname"),
                Bio = rows.GetString("bio"),
                rating = rows.GetFloat("rating"),
            });
        }

        return new PagedResult<Actor>(actors, totalCount);
    }

    public async Task<Actor?> Create(Actor actor)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
    cmd.CommandText = @"
    INSERT INTO Actors (firstname, lastname, bio, rating)
    VALUES(@firstname, @lastname, @bio, @rating); 
    SELECT LAST_INSERT_ID();";
        cmd.Parameters.AddWithValue("@firstname", actor.FirstName);
        cmd.Parameters.AddWithValue("@lastname", actor.LastName);
        cmd.Parameters.AddWithValue("@bio", actor.Bio);
        cmd.Parameters.AddWithValue("@rating", actor.rating);

        actor.Id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return actor;
    }

    public async Task<Actor?> Read(int id)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "SELECT * FROM Actors WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        using var rows = await cmd.ExecuteReaderAsync();
        if (await rows.ReadAsync())
        {
            return new Actor
            {
                Id = rows.GetInt32("id"),
                FirstName = rows.GetString("firstname"),
                LastName = rows.GetString("lastname"),
                Bio = rows.GetString("bio"),
                rating = rows.GetInt32("rating"),
            };
        }
        return null;
    }

    public async Task<Actor?> Update(int id, Actor newActor)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        UPDATE Actors SET 
            firstname = @firstname, 
            lastname = @lastname, 
            bio = @bio, 
            rating = @rating 
        WHERE id = @id";

        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@firstname", newActor.FirstName);
        cmd.Parameters.AddWithValue("@lastname", newActor.LastName);
        cmd.Parameters.AddWithValue("@bio", newActor.Bio);
        cmd.Parameters.AddWithValue("@rating", newActor.rating);

        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? newActor : null;
    }

    public async Task<Actor?> Delete(int id)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = "DELETE FROM Actors WHERE id = @id";
        cmd.Parameters.AddWithValue("@id", id);

        Actor? actor = await Read(id);
        return Convert.ToInt32(await cmd.ExecuteNonQueryAsync()) > 0 ? actor : null;
    }
}