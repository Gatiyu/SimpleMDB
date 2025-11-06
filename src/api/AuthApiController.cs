using System;
using System.Net;
using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace SimpleMDB;

public class AuthApiController
{
    private IUserService userService;

    public AuthApiController(IUserService userService)
    {
        this.userService = userService;
    }

    // POST /api/v1/auth/register
    public async Task Register(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var data = JsonSerializer.Deserialize<NameValueCollection?>(body) ?? new NameValueCollection();

        // fallback to parsing as dictionary if needed
        if (data.Count == 0)
        {
            try
            {
                var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();
                foreach (var kv in dict) data[kv.Key] = kv.Value;
            }
            catch {}
        }

        var username = data["username"] ?? "";
        var password = data["password"] ?? "";

        User newUser = new User(0, username, password, "", Roles.USER);
        var result = await userService.Create(newUser);

        if (result.IsValid)
        {
            var json = JsonSerializer.Serialize(new { success = true, message = "User registered", user = result.Value });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
        }
    }

    // POST /api/v1/auth/login
    public async Task Login(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string body = await HttpUtils.GetRequestBody(req);
        var dict = JsonSerializer.Deserialize<Dictionary<string,string>>(body) ?? new Dictionary<string,string>();
        var username = dict.ContainsKey("username") ? dict["username"] : "";
        var password = dict.ContainsKey("password") ? dict["password"] : "";

        var result = await userService.GetToken(username, password);

        if (result.IsValid)
        {
            var token = result.Value!;
            var json = JsonSerializer.Serialize(new { success = true, token = token });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
        }
        else
        {
            var json = JsonSerializer.Serialize(new { success = false, error = result.Error!.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Unauthorized, json);
        }
    }

    // POST /api/v1/auth/logout
    public async Task Logout(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        // For the simple API, just return success. Token invalidation is not implemented in the service.
        var json = JsonSerializer.Serialize(new { success = true, message = "Logged out" });
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
    }
}
