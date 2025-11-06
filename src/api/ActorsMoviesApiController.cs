using System;
using System.Net;
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;

namespace SimpleMDB;

public class ActorsMoviesApiController
{
    private IActorMovieService actorMovieService;

    public ActorsMoviesApiController(IActorMovieService actorMovieService)
    {
        this.actorMovieService = actorMovieService;
    }

    // POST /api/v1/actors-movies
    public async Task Create(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();

        int actorId = dict.ContainsKey("actorId") && int.TryParse(dict["actorId"], out int a) ? a : 0;
        int movieId = dict.ContainsKey("movieId") && int.TryParse(dict["movieId"], out int m) ? m : 0;
        string role = dict.ContainsKey("role") ? dict["role"] : "";

        var result = await actorMovieService.Create(actorId, movieId, role);
        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, actorMovie = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // GET /api/v1/actors-movies/{id}
    public async Task Read(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        // Not implemented in service/repository - return 501 Not Implemented
        var json = JsonSerializer.Serialize(new { success = false, error = "Read by id is not implemented for actor-movie associations." });
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotImplemented, json);
    }

    // PUT /api/v1/actors-movies/{id}
    public async Task Update(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        // Not implemented in service/repository - return 501 Not Implemented
        var json = JsonSerializer.Serialize(new { success = false, error = "Update is not implemented for actor-movie associations." });
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotImplemented, json);
    }

    // DELETE /api/v1/actors-movies/{id}
    public async Task Delete(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        var result = await actorMovieService.Delete(id);
        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, actorMovie = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }
}
