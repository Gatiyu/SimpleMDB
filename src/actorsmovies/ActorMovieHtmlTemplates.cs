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
                        <td><form action=""/actors/remove?aid={actor.Id}&mid{movie.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure you want to remove this actor?');"">
                            <input type=""submit"" value=""Remove"">
                            </form>
                        </td>
                    </tr>
                    ";
        }

        string html = $@"
            <div class-""add"">
                <a href=""/actors/add"">Add New Actor</a>
            </div>        
                <table class=""viewall"">
                    <thead>
                        <th>Id</th>
                        <th>Title</th>
                        <th>Year</th>
                        <th>Description</th>
                        <th>Rating</th>
                        <th>View</th>
                        <th>Edit</th>
                        <th>Remove</th>
                    </thead>
                    <tbody>
                        {rows}
                    </tbody>
                </table>
                <div class""pagination"">
                    <a href=""?page=1&size={size}"">First</a>
                    <a href=""?page={page - 1}&size={size}"">Prev</a>
                    <span>{page} / {pageCount}</span>
                    <a href=""?page={page + 1}&size={size}"">Next</a>
                    <a href=""?page={pageCount}&size={size}"">Last</a>
                </div>

                ";
        return html;
    }
}