using System;
using System.Net;
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace SimpleMDB;

public class UsersApiController
{
    private IUserService userService;

    public UsersApiController(IUserService userService)
    {
        this.userService = userService;
    }

    // GET /api/v1/users?page=1&size=5
    public async Task List(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result = await userService.ReadAll(page, size);
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

    // POST /api/v1/users
    public async Task Create(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();

        string username = dict.ContainsKey("username") ? dict["username"] : "";
        string password = dict.ContainsKey("password") ? dict["password"] : "";
        string role = dict.ContainsKey("role") ? dict["role"] : Roles.USER;

        var newUser = new User(0, username, password, "", role);
        var result = await userService.Create(newUser);

        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, user = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // GET /api/v1/users/{id}
    public async Task Read(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        var result = await userService.Read(id);
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

    // PUT /api/v1/users/{id}
    public async Task Update(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();

        string username = dict.ContainsKey("username") ? dict["username"] : "";
        string password = dict.ContainsKey("password") ? dict["password"] : "";
        string role = dict.ContainsKey("role") ? dict["role"] : Roles.USER;

        var newUser = new User(0, username, password, "", role);
        var result = await userService.Update(id, newUser);

        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, user = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // DELETE /api/v1/users/{id}
    public async Task Delete(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, int id)
    {
        var result = await userService.Delete(id);
        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, user = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }
}
