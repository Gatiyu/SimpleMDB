using System;
using System.Net;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace SimpleMDB
{
    public class MovieController
    {
        private IMovieService movieService;

        public MovieController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        //GET /movie?page=1&size=5
        public async Task ViewAllMoviesGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string message = req.QueryString["message"] ?? "";

            int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
            int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

            var result = await movieService.ReadAll(page, size);

            if (result.IsValid)
            {
                PagedResult<Movie> pagedResult = result.Value!;
                List<Movie> movies = pagedResult.Values;
                int movieCount = pagedResult.TotalCount;

                string html = MoviesHtmlTemplates.ViewAllMoviesGet(movies, movieCount, page, size);
                html = HtmlTemplates.Base("SimpleMDB", "Movies View All Page", html, message);
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                await HttpUtils.Redirect(req, res, options, "/");
            }
        }

        // GET/movies/add
        public async Task AddMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string title = req.QueryString["title"] ?? "";
            string year = req.QueryString["year"] ?? "";
            string description = req.QueryString["description"] ?? "";
            string rating = req.QueryString["rating"] ?? "";
            string message = req.QueryString["message"] ?? "";

            string html = MoviesHtmlTemplates.AddMovieGet(title, year, description, rating);
            html = HtmlTemplates.Base("SimpleMDB", "Movies Add Page", html, message);

            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.Created, html);
        }

        // POST /movies/add

        public async Task AddMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            var formData = (NameValueCollection?)options["req.form"] ?? [];

            string title = formData["title"] ?? "";
            int year = int.TryParse(formData["year"], out int y) ? y : DateTime.Now.Year;
            string description = formData["description"] ?? "";
            float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

            Console.WriteLine($"title={title}");
            Movie newMovie = new Movie(0, title, year, description, rating);

            var result = await movieService.Create(newMovie);

            if (result.IsValid)
            {
                HttpUtils.AddOptions(options, "redirect", "message", "Movie added successfully");

                await HttpUtils.Redirect(req, res, options, "/movies"); //PRG
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                HttpUtils.AddOptions(options, "redirect", formData);

                await HttpUtils.Redirect(req, res, options, "/movies/add");
            }
        }

        //GET /movies/view?mid
        public async Task ViewMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string message = req.QueryString["message"] ?? "";

            int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : 1;

            Result<Movie> result = await movieService.Read(mid);

            if (result.IsValid)
            {
                Movie movie = result.Value!;

                string html = MoviesHtmlTemplates.ViewMovieGet(movie);

                html = HtmlTemplates.Base("SimpleMDB", "Movies View Page", html, message);
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
            }
        }

        // GET /movies/edit?mid=1
        public async Task EditMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string message = req.QueryString["message"] ?? "";

            int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : 1;

            Result<Movie> result = await movieService.Read(mid);

            if (result.IsValid)
            {
                Movie movie = result.Value!;

                string html = MoviesHtmlTemplates.EditMovieGet(movie, mid);

                html = HtmlTemplates.Base("SimpleMDB", "Movies Edit Page", html, message);
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
            }

        }

        // POST /movies/eedit?mid=1

        public async Task EditMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : 1;

            var formData = (NameValueCollection?)options["req.form"] ?? [];

            string title = formData["title"] ?? "";
            int year = int.TryParse(formData["year"], out int y) ? y : DateTime.Now.Year;
            string description = formData["description"] ?? "";
            float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

            Console.WriteLine($"title={title}");
            Movie newMovie = new Movie(0, title, year, description, rating);

            var result = await movieService.Update(mid, newMovie);

            if (result.IsValid)
            {
                HttpUtils.AddOptions(options, "redirect", "message", "Movie edited successfully!");
                await HttpUtils.Redirect(req, res, options, "/movies");
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

                await HttpUtils.Redirect(req, res, options, "/movies/edit");
            }
        }

        //GET /movies/remove?mid=1
        
          public async Task RemoveMoviePost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {

            int mid = int.TryParse(req.QueryString["mid"], out int u) ? u : 1;

            Result<Movie> result = await movieService.Delete(mid);

            if (result.IsValid)
            {
                HttpUtils.AddOptions(options, "redirect", "message", "Movie removed successfully!");
                await HttpUtils.Redirect(req, res, options, "/movies");
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                await HttpUtils.Redirect(req, res, options, "/movies");
            }
        }
    }
}