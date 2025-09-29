using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace SimpleMDB;

public class ActorMovieHtmlTemplates
{
    private readonly IActorMovieService actorMovieService;

    public ActorMovieHtmlTemplates(IActorMovieService actorMovieService)
    {
        this.actorMovieService = actorMovieService;
    }
    public static string ViewAllMoviesByActor(Actor actor, List<(ActorMovie, Movie)> actorMovies, int totalCount, int page, int size)
    {
        // defensive: ensure we never iterate a null list or access a null actor
        if (actor == null) actor = new Actor();
        if (actorMovies == null) actorMovies = new List<(ActorMovie, Movie)>();

        int pageCount = (int)Math.Ceiling((double)totalCount / size);

        string rows = "";

        foreach (var (am, movie) in actorMovies)
        {
            if (movie == null) continue;
            rows += @$"
                    <tr>
                        <td>{movie.Id}</td>
                        <td>{movie.Title}</td>
                        <td>{movie.Year}</td>
                        <td>{movie.Description}</td>
                        <td>{movie.rating}</td>
                        <td>{am.RoleName}</td>
                        <td>
                            <form action=""/actors/movies/remove?amid={movie.Id}&mid={actor.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure you want to remove this movie from the actor?');"">
                                <input type=""submit"" value=""Remove"">
                            </form>
                        </td>
                    </tr>
                    ";
        }

        string pDisable = (page > 1).ToString().ToLower();
        string nDisable = (page < pageCount).ToString().ToLower();

        string html = $@"
            <div class=""add"">
                 <a href=""/actors/movies/add?aid={actor.Id}"">Add New ActorMovie</a>
            </div>        
            <table class=""viewall"">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Title</th>
                        <th>Year</th>
                        <th>Description</th>
                        <th>Rating</th>
                        <th>Role Name</th>
                        <th>View</th>
                        <th>Edit</th>
                        <th>Remove</th>   
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>
            <div class=""pagination"">
                <a href=""?aid={actor.Id}&page=1&size={size}"" onclick=""return {pDisable};"">First</a>
                <a href=""?aid={actor.Id}&page={Math.Max(1, page - 1)}&size={size}"" onclick=""return {pDisable};"">Prev</a>
                <span>{page} / {Math.Max(1, pageCount)}</span>
                <a href=""?aid={actor.Id}&page={Math.Min(pageCount, page + 1)}&size={size}"" onclick=""return {nDisable};"">Next</a>
                <a href=""?aid={actor.Id}&page={pageCount}&size={size}"" onclick=""return {nDisable};"">Last</a>
            </div>";
        return html;
    }

    public static string ViewAllActorsByMovie(Movie movie, List<(ActorMovie, Actor)> amas, int totalCount, int page, int size)
    {
        int pageCount = (int)Math.Ceiling((double)totalCount / size);

        string rows = "";

            foreach (var (am, actor) in amas)
        {
            if (actor == null) continue;
            rows += @$"
                    <tr>
                        <td>{actor.Id}</td>
                        <td>{actor.FirstName}</td>
                        <td>{actor.LastName}</td>
                        <td>{actor.Bio}</td>
                        <td>{actor.rating}</td>
                        <td>{am.RoleName}</td>
                        <td>
                            <form action=""/movies/actors/remove?amid={am.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure you want to remove this actor from the movie?');"">
                                <input type=""submit"" value=""Remove"">
                            </form>
                        </td>
                    </tr>
                    ";
        }

        string pDisable = (page > 1).ToString().ToLower();
        string nDisable = (page < pageCount).ToString().ToLower();

        string html = $@"
            <div class=""add"">
                <a href=""/movies/actors/add?mid={movie.Id}"">Add New Actor</a>
            </div>        
            <table class=""viewall"">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>Bio</th>
                        <th>Rating</th>
                        <th>Role Name</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {rows}
                </tbody>
            </table>
            <div class=""pagination"">
                <a href=""?mid={movie.Id}&page=1&size={size}"" onclick=""return {pDisable};"">First</a>
                <a href=""?mid={movie.Id}&page={Math.Max(1, page - 1)}&size={size}"" onclick=""return {pDisable};"">Prev</a>
                <span>{page} / {Math.Max(1, pageCount)}</span>
                <a href=""?mid={movie.Id}&page={Math.Min(pageCount, page + 1)}&size={size}"" onclick=""return {nDisable};"">Next</a>
                <a href=""?mid={movie.Id}&page={pageCount}&size={size}"" onclick=""return {nDisable};"">Last</a>
            </div>";

        return html;
    }

    public static string AddMoviesByActorGet(Actor actor, List<Movie> movies)
    {
        string movieOptions = "";

        foreach (var movie in movies)
        {
            movieOptions += $@"<option value=""{movie.Id}"">{movie.Title} ({movie.Year})</option>";
        }

        string html = $@"
            <form action=""/actors/movies/add"" method=""POST"">
                
                <select name=""aid"" id=""mid"">
                    <option value=""{actor.Id}"">{actor.FirstName} {actor.LastName}</option>
                </select>
                <label for=""mid"">Movies</label>
                <select name=""mid"" id=""mid"">
                    {movieOptions}
                </select>
                <label for=""rolename"">Role Name</label>
                <input id=""rolename"" name=""rolename"" type=""text"" placeholder=""Role Name"">
                <input type=""submit"" value=""Add Movie to Actor"">
            </form>";

        return html;
    }
    public static string AddActorsByMovie(Movie movie, List<Actor> actors)
    {
        string actorOptions = "";

        foreach (var actor in actors)
        {
            actorOptions += $@"<option value=""{actor.Id}"">{actor.FirstName} {actor.LastName}</option>";
        }

        string html = $@"
            <form action=""/movies/actors/add"" method=""POST"">
                
                <label for=""mid"">Movie</label>
                <select name=""mid"" id=""mid"">
                    <option value=""{movie.Id}"">{movie.Title} {movie.Year}</option>
                </select>
                <label for=""aid"">Actors</label>
                <select name=""aid"" id=""aid"">
                    {actorOptions}
                </select>
                <label for=""rolename"">Role Name</label>
                <input id=""rolename"" name=""rolename"" type=""text"" placeholder=""Role Name"">
                <input type=""submit"" value=""Add Movie to Actor"">
            </form>";

        return html;
    }
}