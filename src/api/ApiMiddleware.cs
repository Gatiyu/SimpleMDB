using System;
using System.Net;
using System.Collections;
using System.Threading.Tasks;

namespace SimpleMDB;

public class ApiMiddleware
{
    private AuthApiController authApi;
    private UsersApiController usersApi;
    private ActorsApiController actorsApi;
    private MoviesApiController moviesApi;
    private ActorsMoviesApiController actorsMoviesApi;
    private IActorMovieService actorMovieService;

    public ApiMiddleware(IUserService userService, IActorService actorService, IMovieService movieService, IActorMovieService actorMovieService)
    {
        this.authApi = new AuthApiController(userService);
        this.usersApi = new UsersApiController(userService);
        this.actorsApi = new ActorsApiController(actorService);
        this.moviesApi = new MoviesApiController(movieService);
        this.actorsMoviesApi = new ActorsMoviesApiController(actorMovieService);
        this.actorMovieService = actorMovieService;
    }

    public async Task Handle(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var path = req.Url?.AbsolutePath ?? "";
        if (!path.StartsWith("/api/v1/")) return; // not an API request - let other handlers run

        // Stop default not-found behavior - we'll respond here
        // route: /api/v1/{domain}/... 
        var seg = path.Trim('/').Split('/');
        // seg[0] == "api", seg[1] == "v1", seg[2] == domain
        if (seg.Length < 3)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(new { success = false, error = "Invalid API route" });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
            return;
        }

        var domain = seg[2];
        // helper to parse id from segment 3 if present
        int id = -1;
        if (seg.Length >= 4 && int.TryParse(seg[3], out int parsed)) id = parsed;

        try
        {
            switch (domain)
            {
                case "auth":
                    await HandleAuth(req, res, options, seg);
                    return;
                case "users":
                    await HandleUsers(req, res, options, seg, id);
                    return;
                case "actors":
                    await HandleActors(req, res, options, seg, id);
                    return;
                case "movies":
                    await HandleMovies(req, res, options, seg, id);
                    return;
                case "actors-movies":
                    await HandleActorsMovies(req, res, options, seg, id);
                    return;
                default:
                    var json = System.Text.Json.JsonSerializer.Serialize(new { success = false, error = "Unknown API domain" });
                    await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotFound, json);
                    return;
            }
        }
        catch (Exception ex)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(new { success = false, error = ex.Message });
            await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.InternalServerError, json);
            return;
        }
    }

    private async Task HandleAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, string[] seg)
    {
        // possible /api/v1/auth/register, /api/v1/auth/login, /api/v1/auth/logout
        if (req.HttpMethod == "POST")
        {
            if (seg.Length >= 4 && seg[3] == "register") { await authApi.Register(req, res, options); return; }
            if (seg.Length >= 4 && seg[3] == "login") { await authApi.Login(req, res, options); return; }
            if (seg.Length >= 4 && seg[3] == "logout") { await authApi.Logout(req, res, options); return; }
        }

        var json = System.Text.Json.JsonSerializer.Serialize(new { success = false, error = "Method not allowed" });
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.MethodNotAllowed, json);
    }

    private async Task HandleUsers(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, string[] seg, int id)
    {
        if (req.HttpMethod == "GET")
        {
            if (seg.Length == 3) { await usersApi.List(req, res, options); return; }
            // support legacy style: /api/v1/users/view?uid=45
            if (seg.Length >= 4 && seg[3] == "view")
            {
                int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;
                if (uid >= 0) { await usersApi.Read(req, res, options, uid); return; }
            }
            if (seg.Length >= 4 && id >= 0) { await usersApi.Read(req, res, options, id); return; }
        }
        if (req.HttpMethod == "POST" && seg.Length == 3) { await usersApi.Create(req, res, options); return; }
        if (req.HttpMethod == "PUT" && seg.Length >= 4 && id >= 0) { await usersApi.Update(req, res, options, id); return; }
        if (req.HttpMethod == "DELETE" && seg.Length >= 4 && id >= 0) { await usersApi.Delete(req, res, options, id); return; }

        var json = System.Text.Json.JsonSerializer.Serialize(new { success = false, error = "Method not allowed or bad path" });
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.MethodNotAllowed, json);
    }

    private async Task HandleActors(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, string[] seg, int id)
    {
        // /api/v1/actors or /api/v1/actors/{id} or /api/v1/actors/{id}/movies
        if (req.HttpMethod == "GET")
        {
            if (seg.Length == 3) { await actorsApi.List(req, res, options); return; }
            // support legacy view: /api/v1/actors/view?aid=12
            if (seg.Length >= 4 && seg[3] == "view")
            {
                int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : -1;
                if (aid >= 0) { await actorsApi.Read(req, res, options, aid); return; }
            }
            if (seg.Length >= 4 && id >= 0)
            {
                if (seg.Length >= 5 && seg[4] == "movies") { await actorsApi.MoviesByActor(req, res, options, id); return; }
                await actorsApi.Read(req, res, options, id); return;
            }
        }
        if (req.HttpMethod == "POST" && seg.Length == 3) { await actorsApi.Create(req, res, options); return; }
        if (req.HttpMethod == "PUT" && seg.Length >= 4 && id >= 0) { await actorsApi.Update(req, res, options, id); return; }
        if (req.HttpMethod == "DELETE" && seg.Length >= 4 && id >= 0) { await actorsApi.Delete(req, res, options, id); return; }

        var json = System.Text.Json.JsonSerializer.Serialize(new { success = false, error = "Method not allowed or bad path" });
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.MethodNotAllowed, json);
    }

    private async Task HandleMovies(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, string[] seg, int id)
    {
        // /api/v1/movies or /api/v1/movies/{id} or /api/v1/movies/{id}/actors
        if (req.HttpMethod == "GET")
        {
            if (seg.Length == 3) { await moviesApi.List(req, res, options); return; }
            // support legacy view: /api/v1/movies/view?mid=5
            if (seg.Length >= 4 && seg[3] == "view")
            {
                int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;
                if (mid >= 0) { await moviesApi.Read(req, res, options, mid); return; }
            }
            if (seg.Length >= 4 && id >= 0)
            {
                if (seg.Length >= 5 && seg[4] == "actors") { await moviesApi.ActorsByMovie(req, res, options, id, actorMovieService); return; }
                await moviesApi.Read(req, res, options, id); return;
            }
        }
        if (req.HttpMethod == "POST" && seg.Length == 3) { await moviesApi.Create(req, res, options); return; }
        if (req.HttpMethod == "PUT" && seg.Length >= 4 && id >= 0) { await moviesApi.Update(req, res, options, id); return; }
        if (req.HttpMethod == "DELETE" && seg.Length >= 4 && id >= 0) { await moviesApi.Delete(req, res, options, id); return; }

        var json = System.Text.Json.JsonSerializer.Serialize(new { success = false, error = "Method not allowed or bad path" });
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.MethodNotAllowed, json);
    }

    private async Task HandleActorsMovies(HttpListenerRequest req, HttpListenerResponse res, Hashtable options, string[] seg, int id)
    {
        if (req.HttpMethod == "POST" && seg.Length == 3) { await actorsMoviesApi.Create(req, res, options); return; }
        if (req.HttpMethod == "GET" && seg.Length >= 4 && id >= 0) { await actorsMoviesApi.Read(req, res, options, id); return; }
        if (req.HttpMethod == "PUT" && seg.Length >= 4 && id >= 0) { await actorsMoviesApi.Update(req, res, options, id); return; }
        if (req.HttpMethod == "DELETE" && seg.Length >= 4 && id >= 0) { await actorsMoviesApi.Delete(req, res, options, id); return; }

        var json = System.Text.Json.JsonSerializer.Serialize(new { success = false, error = "Method not allowed or bad path" });
        await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.MethodNotAllowed, json);
    }
}
