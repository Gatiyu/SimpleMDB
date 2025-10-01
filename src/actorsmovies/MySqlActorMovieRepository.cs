
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SimpleMDB;

public class MySqlActorMovieRepository : IActorMovieRepository
{
    private string connectionString;

    public MySqlActorMovieRepository(string connectionString)
    {
        this.connectionString = connectionString;
        //Init();
    }

    private void Init()
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS ActorMovies
        (
            id int AUTO_INCREMENT PRIMARY KEY,
            actorId INT NOT NULL,
            movieId INT NOT NULL,
            FOREIGN KEY (actorId) REFERENCES Actors(id) ON DELETE CASCADE,
            FOREIGN KEY (movieId) REFERENCES Movies(id) ON DELETE CASCADE
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

    public async Task<PagedResult<(ActorMovie, Movie)>> ReadAllMoviesByActor(int actorId, int page, int size)
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"
            SELECT COUNT(*) FROM ActorsMovies as am
            JOIN Movies as m ON am.movieId = m.id
            WHERE am.actorId = @actorId
        ";
        countCmd.Parameters.AddWithValue("@actorId", actorId);
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM ActorsMovies as am
            JOIN Movies as m ON am.movieId = m.id
            WHERE am.actorId = @actorId
            LIMIT @offset, @limit
        ";
        cmd.Parameters.AddWithValue("@actorId", actorId);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();

        var amms = new List<(ActorMovie, Movie)>();
        while (await rows.ReadAsync())
        {
            ActorMovie am = new ActorMovie(
            rows.GetInt32(0),   // id
            rows.GetInt32(1),   // actorId
            rows.GetInt32(2),   // movieId
            rows.GetString(3)); // roleName

            Movie m = new Movie(
            rows.GetInt32(4),  // id
            rows.GetString(5), // title
            rows.GetInt32(6),  // year
            rows.GetString(7), // description
            rows.GetFloat(8)   // rating
            );

            amms.Add((am, m));
        }

        return new PagedResult<(ActorMovie, Movie)>(amms, totalCount);
    }

    public async Task<PagedResult<(ActorMovie, Actor)>> ReadAllActorsByMovie(int movieId, int page, int size)
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"
            SELECT COUNT(*) FROM ActorsMovies as am
            JOIN Actors as m ON am.actorId = m.id
            WHERE am.movieId = @movieId
        ";
        countCmd.Parameters.AddWithValue("@movieId", movieId);
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM ActorsMovies as am
            JOIN Actors as m ON am.actorId = m.id
            WHERE am.movieId = @movieId
            LIMIT @offset, @limit
        ";
        cmd.Parameters.AddWithValue("@movieId", movieId);
        cmd.Parameters.AddWithValue("@offset", (page - 1) * size);
        cmd.Parameters.AddWithValue("@limit", size);

        using var rows = await cmd.ExecuteReaderAsync();

        var amms = new List<(ActorMovie, Actor)>();
        while (await rows.ReadAsync())
        {
            ActorMovie am = new ActorMovie(
            rows.GetInt32(0),   // id
            rows.GetInt32(1),   // actorId
            rows.GetInt32(2),   // movieId
            rows.GetString(3)); // roleName

            Actor a = new Actor(
            rows.GetInt32(4),  // id
            rows.GetString(5), // firstname
            rows.GetString(6), // lastname
            rows.GetString(7), // bio
            rows.GetFloat(8)   // rating
            );

            amms.Add((am, a));
        }

        return new PagedResult<(ActorMovie, Actor)>(amms, totalCount);
    }

    public async Task<List<Actor>> ReadAllActors()
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"
            SELECT COUNT(*) FROM Actors
        ";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM Actors
        ";

        using var rows = await cmd.ExecuteReaderAsync();

        var actors = new List<Actor>();
        while (await rows.ReadAsync())
        {
            Actor a = new Actor(
            rows.GetInt32(0),  // id
            rows.GetString(1), // firstname
            rows.GetString(2), // lastname
            rows.GetString(3), // bio
            rows.GetFloat(4)   // rating
            );

            actors.Add(a);
        }

        return actors;
    }
    public async Task<List<Movie>> ReadAllMovies()
    {
        using var dbc = OpenDb();

        using var countCmd = dbc.CreateCommand();
        countCmd.CommandText = @"
            SELECT COUNT(*) FROM Movies
        ";
        int totalCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
            SELECT * FROM Movies
        ";

        using var rows = await cmd.ExecuteReaderAsync();

        var movies = new List<Movie>();
        while (await rows.ReadAsync())
        {
            Movie movie = new Movie(
            rows.GetInt32(0),  // id
            rows.GetString(1), // title
            rows.GetInt32(2),  // year
            rows.GetString(3), // description
            rows.GetFloat(4)   // rating
            );

            movies.Add(movie);
        }

        return movies;
    }
    public async Task<ActorMovie?> Create(int actorId, int movieId, string roleName)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        INSERT INTO ActorsMovies (actorId, movieId, roleName)
        VALUES (@actorId, @movieId, @roleName);
        SELECT LAST_INSERT_ID();
        ";

        cmd.Parameters.AddWithValue("@actorId", actorId);
        cmd.Parameters.AddWithValue("@movieId", movieId);
        cmd.Parameters.AddWithValue("@roleName", roleName);

        int id = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        var actorMovie = new ActorMovie(id, actorId, movieId, roleName);
        return actorMovie;
    }

    public async Task<ActorMovie?> Read(int id)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        SELECT * FROM ActorsMovies WHERE id = @id;
        ";
        cmd.Parameters.AddWithValue("@id", id);

        using var row = await cmd.ExecuteReaderAsync();

        if (await row.ReadAsync())
        {
            return new ActorMovie(
            row.GetInt32(0),   // id
            row.GetInt32(1),   // actorId
            row.GetInt32(2),   // movieId
            row.GetString(3)   // roleName
            );
        }
        return null;
    }
    public async Task<ActorMovie?> Delete(int id)
    {
        using var dbc = OpenDb();

        using var cmd = dbc.CreateCommand();
        cmd.CommandText = @"
        DELETE FROM ActorsMovies WHERE id = @id;
        ";
        cmd.Parameters.AddWithValue ("@id", id);
        var actorMovie = await Read(id);
        return (await cmd.ExecuteNonQueryAsync() > 0) ? actorMovie : null;
    }  
}