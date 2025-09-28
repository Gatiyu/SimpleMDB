namespace SimpleMDB;

public class MoviesHtmlTemplates
{
    public static string ViewAllMoviesGet(List<Movie> movies, int movieCount, int page, int size)
    {
        int pageCount = (int)Math.Ceiling((double)movieCount / size);

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
                        <td><a href=""/movies/actors?mid={movie.Id}"">Actors</a></td>
                        <td><form action=""/movies/remove?mid={movie.Id}"" method=""POST"" onsubmit=""return confirm('Are you sure you want to remove this movie?');"">
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
                <a href=""/movies/add"">Add New Movie</a>
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
                    </thead>
                    <tbody>
                        {rows}
                    </tbody>
                </table>
            <div class=""pagination"">
                <a href=""?page=1&size={size}"" onclick=""return {pDisable};"">First</a>
                <a href=""?page={Math.Max(1, page - 1)}&size={size}"" onclick=""return {pDisable};"">Prev</a>
                <span>{page} / {Math.Max(1, pageCount)}</span>
                <a href=""?page={Math.Min(pageCount, page + 1)}&size={size}"" onclick=""return {nDisable};"">Next</a>
                <a href=""?page={pageCount}&size={size}"" onclick=""return {nDisable};"">Last</a>
            </div>
            ";

        return html;
    }
    public static string AddMovieGet(string title, string year, string description, string rating)
    {
        string html = $@"
            <form class=""addform""action=""/movies/add"" method=""POST"">
            <label for=""title"">Title:</label>
            <input id=""title"" name=""title"" type=""text"" placeholder=""Title"" value=""{title}"">
            <label for=""year"">Last Year:</label>
            <input id=""year"" name=""year"" type=""number"" min=""1888"" max=""{DateTime.Now.Year}"" step=""1""placeholder=""Year"" value=""{year}"">
            <label for=""description"">Description:</label>
            <input id=""description"" name=""description"" type=""text"" placeholder=""Description"" value=""{description}"">
            <label for=""rating"">Rating</label>
            <input id=""rating"" name=""rating""type=""number"" min=""0"" max=""10"" step=""0.1""value=""{rating}"">
            <input type=""submit"" value=""Add"">
            </form>
            ";
        return html;
    }
    public static string ViewMovieGet(Movie movie)
    {
        string html = $@"
                <table class=""view"">
                    <thead>
                        <tr>
                            <th>Id</th>
                            <th>Title</th>
                            <th>Year</th>
                            <th>Description</th>
                            <th>Rating</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>{movie.Id}</td>
                            <td>{movie.Title}</td>
                            <td>{movie.Year}</td>
                            <td>{movie.Description}</td>
                            <td>{movie.rating}</td>
                        </tr>
                    </tbody>
                </table>";
        return html;
    }
    public static string EditMovieGet(Movie movie, int mid)
    {
                string html = $@"
                <form class=""editform"" action=""/movies/edit?mid={mid}"" method=""POST"">
                <label for=""title"">Title:</label>
                <input id=""title"" name=""title"" type=""text"" placeholder=""Title"" value = ""{movie.Title}"">
                <label for=""year"">Year:</label>
                <input id=""year"" name=""year"" type=""number"" min=""1888"" max=""{DateTime.Now.Year}"" step=""1""placeholder=""Year"" value=""{movie.Year}"">
                <label for=""description"">Description:</label>
                <input id=""description"" name=""description"" type=""text"" placeholder=""Description"" value=""{movie.Description}"">
                <label for=""rating"">Rating</label>
                <input id=""rating"" name=""rating""type=""number"" min=""0"" max=""10"" step=""0.1""value=""{movie.rating}"">
                <input type=""submit"" value=""Edit"">
            </form>
            ";
        return html;
    }
}