using System;
using System.Net;
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleMDB;

public class MoviesApiController
{
    private IMovieService movieService;

    public MoviesApiController(IMovieService movieService)
    {
        this.movieService = movieService;
    }

    // GET /api/v1/movies?page=1&size=5
    public async Task List(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result = await movieService.ReadAll(page, size);
        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(result.Value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // POST /api/v1/movies
    public async Task Create(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();

        string title = dict.ContainsKey("title") ? dict["title"] : "";
        int year = dict.ContainsKey("year") && int.TryParse(dict["year"], out int y) ? y : DateTime.Now.Year;
        string description = dict.ContainsKey("description") ? dict["description"] : "";
        float rating = dict.ContainsKey("rating") && float.TryParse(dict["rating"], out float r) ? r : 5F;

        var movie = new Movie(0, title, year, description, rating);
        var result = await movieService.Create(movie);

        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, movie = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // GET /api/v1/movies/{id}
    public async Task Read(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        var result = await movieService.Read(id);
        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(result.Value);
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotFound, json);
        }
    }

    // PUT /api/v1/movies/{id}
    public async Task Update(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();

        string title = dict.ContainsKey("title") ? dict["title"] : "";
        int year = dict.ContainsKey("year") && int.TryParse(dict["year"], out int y) ? y : DateTime.Now.Year;
        string description = dict.ContainsKey("description") ? dict["description"] : "";
        float rating = dict.ContainsKey("rating") && float.TryParse(dict["rating"], out float r) ? r : 5F;

        var movie = new Movie(0, title, year, description, rating);
        var result = await movieService.Update(id, movie);

        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, movie = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // DELETE /api/v1/movies/{id}
    public async Task Delete(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        var result = await movieService.Delete(id);
        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, movie = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // GET /api/v1/movies/{id}/actors?page=1&size=5
    public async Task ActorsByMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int movieId, IActorMovieService actorMovieService)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result = await actorMovieService.ReadAllActorsByMovie(movieId, page, size);
        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(result.Value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }
}
