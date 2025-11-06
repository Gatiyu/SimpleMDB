using System;
using System.Net;
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace SimpleMDB;

public class ActorsApiController
{
    private IActorService actorService;

    public ActorsApiController(IActorService actorService)
    {
        this.actorService = actorService;
    }

    // GET /api/v1/actors?page=1&size=5
    public async Task List(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result = await actorService.ReadAll(page, size);
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

    // POST /api/v1/actors
    public async Task Create(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();

        string firstname = dict.ContainsKey("firstname") ? dict["firstname"] : "";
        string lastname = dict.ContainsKey("lastname") ? dict["lastname"] : "";
        string bio = dict.ContainsKey("bio") ? dict["bio"] : "";
        float rating = dict.ContainsKey("rating") && float.TryParse(dict["rating"], out float r) ? r : 5F;

        var actor = new Actor(0, firstname, lastname, bio, rating);
        var result = await actorService.Create(actor);

        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, actor = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // GET /api/v1/actors/{id}
    public async Task Read(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        var result = await actorService.Read(id);
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

    // PUT /api/v1/actors/{id}
    public async Task Update(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();

        string firstname = dict.ContainsKey("firstname") ? dict["firstname"] : "";
        string lastname = dict.ContainsKey("lastname") ? dict["lastname"] : "";
        string bio = dict.ContainsKey("bio") ? dict["bio"] : "";
        float rating = dict.ContainsKey("rating") && float.TryParse(dict["rating"], out float r) ? r : 5F;

        var actor = new Actor(id, firstname, lastname, bio, rating);
        var result = await actorService.Update(id, actor);

        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, actor = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // DELETE /api/v1/actors/{id}
    public async Task Delete(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        var result = await actorService.Delete(id);
        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, actor = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // GET /api/v1/actors/{id}/movies?page=1&size=5
    public async Task MoviesByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int actorId)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result = await actorService.GetMoviesByActorId(actorId, page, size);
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
