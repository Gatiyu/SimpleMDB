namespace SimpleMDB;

public class ActorMovieHtmlTemplates
{
    public static string ViewAllMovieByActor(Actor actor, List<Movie> movies, int totalCount, int page, int size)
    {
        int pageCount = (int)Math.Ceiling((double)totalCount / size);

        string rows = "";

        foreach (var movie in movies)
        {
            rows += @$"
                    <tr>
                        <td>{movie.Id}</td>
                        <td>{movie.Title}</td>
                        <td>{movie.Year}</td>
                        <td>{movie.Description}</td>
                        <td>{movie.rating}</td>
                        <td><a href=""/movies/view?mid={movie.Id}"">View</a></td>
                        <td><a href=""/movies/edit?mid={movie.Id}"">Edit</a></td>
                        <td>
                            <form action=""/actors/movies/remove?aid={actor.Id}&mid={movie.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure you want to remove this movie from the actor?');"">
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
                <a href=""/actors/add?aid={actor.Id}"">Add New Actor</a>
            </div>        
            <table class=""viewall"">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Title</th>
                        <th>Year</th>
                        <th>Description</th>
                        <th>Rating</th>
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
                <a href=""?mid={actor.Id}&page=1&size={size}"" onclick=""return {pDisable};"">First</a>
                <a href=""?mid={actor.Id}&page={Math.Max(1, page - 1)}&size={size}"" onclick=""return {pDisable};"">Prev</a>
                <span>{page} / {Math.Max(1, pageCount)}</span>
                <a href=""?mid={actor.Id}&page={Math.Min(pageCount, page + 1)}&size={size}"" onclick=""return {nDisable};"">Next</a>
                <a href=""?mid={actor.Id}&page={pageCount}&size={size}"" onclick=""return {nDisable};"">Last</a>
            </div>";
        return html;
    }
}