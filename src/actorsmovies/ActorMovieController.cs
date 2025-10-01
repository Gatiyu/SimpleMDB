using System;
using System.Net;
using System.Collections;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SimpleMDB;

public class ActorMovieController
{
    private IActorMovieService actorMovieService;
    private IActorService actorService;
    private IMovieService movieService;

    public ActorMovieController(IActorMovieService actorMovieService, IActorService actorService, IMovieService movieService)
    {
        this.actorMovieService = actorMovieService;
        this.actorService = actorService;
        this.movieService = movieService;
    }

    // GET /actors/movies?aid=18&page=1&size=5
    public async Task ViewAllMovieByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

    int aid = int.TryParse(req.QueryString["aid"], out int m) ? m : 1;
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result1 = await actorService.Read(aid);
        var result2 = await actorMovieService.ReadAllMoviesByActor(aid, page, size);

        if (result1.IsValid && result2.IsValid)
        {
            var actor = result1.Value!;
            var pagedResult = result2.Value!;
            var movies = pagedResult.Values;
            int totalCount = pagedResult.TotalCount;

            string content = ActorMovieHtmlTemplates.ViewAllMoviesByActor(actor, movies, totalCount, page, size);
            string html = HtmlTemplates.Base("SimpleMDB", "View All Movies By Actor Page", content, message);
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }

    }

    //GET /actors/movies/add?aid=1
    public async Task AddActorByMovieGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int m) ? m : 1;

        var result1 = await actorService.Read(aid);
        var result2 = await actorMovieService.ReadAllMovies();

        if (result1.IsValid && result2.IsValid)
        {
            var actor = result1.Value!;
            var movies = result2.Value!;

            string content = ActorMovieHtmlTemplates.AddMoviesByActorGet(actor, movies);
            string html = HtmlTemplates.Base("SimpleMDB", "Add Movies By Actor Page", content, message);
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }
    //GET /movies/actors/add?aid=1
    public async Task AddActorsByMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : -1;

        var result1 = await movieService.Read(mid);
        var result2 = await actorMovieService.ReadAllActors();

        if (result1.IsValid && result2.IsValid)
        {
            var movie = result1.Value!;
            var actors = result2.Value!;

            string content = ActorMovieHtmlTemplates.AddActorsByMovie(movie, actors);
            string html = HtmlTemplates.Base("SimpleMDB", "Add Actors By Movie Page", content, message);
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }

    // POST /actors/movies/add (create association)
    public async Task AddMoviesByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        int aid = int.TryParse(formData["aid"], out int a) ? a : 0;
        int mid = int.TryParse(formData["mid"], out int m) ? m : 0;
        string role = formData["rolename"] ?? "";

        var result = await actorMovieService.Create(aid, mid, role);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Movie added to actor successfully");
            await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={aid}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);
            await HttpUtils.Redirect(req, res, options, $"/actors/movies/add?aid={aid}");
        }
    }
    // POST /actors/movies/add 
    public async Task AddMoviesByActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        int aid = int.TryParse(formData["aid"], out int a) ? a : 0;
        int mid = int.TryParse(formData["mid"], out int m) ? m : 0;
        string role = formData["rolename"] ?? "";

        var result = await actorMovieService.Create(aid, mid, role);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Movie added to actor successfully");
            await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={aid}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);
            await HttpUtils.Redirect(req, res, options, $"/actors/movies/add?aid={aid}");
        }
    }

    // POST /movies/actors/add 
    public async Task AddActorsByPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];

        int mid = int.TryParse(formData["mid"], out int m) ? m : -1;
        int aid = int.TryParse(formData["aid"], out int a) ? a : -1;
        string role = formData["rolename"] ?? "Popo";
        // ensure the movie exists
        var movieResult = await movieService.Read(mid);
        if (!movieResult.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", movieResult.Error!.Message);
            await HttpUtils.Redirect(req, res, options, "/");
            return;
        }

        var result = await actorMovieService.Create(aid, mid, role);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Movie added to actor successfully");
            await HttpUtils.Redirect(req, res, options, $"/movies/actors?mid={mid}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);
            await HttpUtils.Redirect(req, res, options, $"/movies/actors/add?mid={mid}");
        }
    }

    // GET/actors/add
    public async Task AddActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string firstname = req.QueryString["firstname"] ?? "";
        string lastname = req.QueryString["lastname"] ?? "";
        string bio = req.QueryString["bio"] ?? "";
        string rating = req.QueryString["rating"] ?? "";
        string message = req.QueryString["message"] ?? "";

        string html = ActorsHtmlTemplates.AddActorGet(firstname, lastname, bio, rating);
        html = HtmlTemplates.Base("SimpleMDB", "Actors Add Page", html, message);

        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.Created, html);
    }

    // POST /actors/add
    public async Task AddActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];

    string firstname = formData["firstname"] ?? "";
    string lastname = formData["lastname"] ?? "";
        string bio = formData["bio"] ?? "";
        float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

        Console.WriteLine($"firstname={firstname}");
        Actor newActor = new Actor(0, firstname, lastname, bio, rating);

        var result = await actorService.Create(newActor);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor added successfully");

            await HttpUtils.Redirect(req, res, options, "/actors"); //PRG
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);

            await HttpUtils.Redirect(req, res, options, "/actors/add");
        }
    }

    //GET /actors/view?aid
    public async Task ViewActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;

        Result<Actor> result = await actorService.Read(aid);

        if (result.IsValid)
        {
            Actor actor = result.Value!;

            string html = ActorsHtmlTemplates.ViewActorGet(actor);

            html = HtmlTemplates.Base("SimpleMDB", "Actors View Page", html, message);
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
        }
    }

    // GET /actors/edit?aid=1
    public async Task EditActorGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;

        Result<Actor> result = await actorService.Read(aid);

        if (result.IsValid)
        {
            Actor actor = result.Value!;

            string html = ActorsHtmlTemplates.EditActorGet(actor, aid);

            html = HtmlTemplates.Base("SimpleMDB", "Actors Edit Page", html, message);
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
        }

    }

    // POST /actors/edit?aid=1
    public async Task EditActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : 1;

        var formData = (NameValueCollection?)options["req.form"] ?? [];

        string firstname = formData["firstname"] ?? "";
        string lastname = formData["lastname"] ?? "";
        string bio = formData["bio"] ?? "";
        float rating = float.TryParse(formData["rating"], out float r) ? r : 5F;

        Console.WriteLine($"firstname={firstname}");
        Actor newActor = new Actor(0, firstname, lastname, bio, rating);

        var result = await actorService.Update(aid, newActor);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor edited successfully!");
            await HttpUtils.Redirect(req, res, options, "/actors");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);

            await HttpUtils.Redirect(req, res, options, "/actors/edit");
        }
    }

    //POST /actors/movies/remove?amid=1
    public async Task RemoveActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {

        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : 1;

        Result<ActorMovie> result = await actorMovieService.Delete(amid);


        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Actor removed successfully!");
            await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={result.Value!.ActorId}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, $"/actors");
        }
    }

    // POST /actors/movies/add?aid=1
    public async Task AddMoviesActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var formData = (NameValueCollection?)options["req.form"] ?? [];
        var aid = int.TryParse(formData["aid"], out int a) ? a : 1;
        var mid = int.TryParse(formData["mid"], out int m) ? m : 1;
        var rolename = formData["rolename"] ?? "";

        var result = await actorMovieService.Create(aid, mid, rolename);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "ActorMovie added successfully");
            await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={aid}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", formData);
            await HttpUtils.Redirect(req, res, options, $"/actors/movies/add?aid={aid}");
        }
    }

    // POST /actors/movies/remove?amid=1
    public async Task RemoveMoviesByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : 0;

        var result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Movie removed from actor successfully");
            await HttpUtils.Redirect(req, res, options, $"/actors/movies?aid={result.Value!.ActorId}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, $"/actors");
        }
    }

    // POST /movies/actors/remove?amid=1
    public async Task RemoveActorsByMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        int amid = int.TryParse(req.QueryString["amid"], out int a) ? a : -1;

        var result = await actorMovieService.Delete(amid);

        if (result.IsValid)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Movie removed from actor successfully");
            await HttpUtils.Redirect(req, res, options, "/movies");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            await HttpUtils.Redirect(req, res, options, $"/movies");
        }
    }

    // GET /movies/actors?mid=1&page=1&size=5
    public async Task ViewAllActorsByMovie(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        int mid = int.TryParse(req.QueryString["mid"], out int m) ? m : 1;
        int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
        int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

        var result1 = await movieService.Read(mid);
        var result2 = await actorMovieService.ReadAllActorsByMovie(mid, page, size);

        if (result1.IsValid && result2.IsValid)
        {
            var movie = result1.Value!;
            var pagedResult = result2.Value!;
            var actorMovies = pagedResult.Values;
            int totalCount = pagedResult.TotalCount;

            string content = ActorMovieHtmlTemplates.ViewAllActorsByMovie(movie, actorMovies, totalCount, page, size);
            string html = HtmlTemplates.Base("SimpleMDB", "View All Actors By Movie Page", content, message);
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
        }
        else
        {
            string error = result1.IsValid ? "" : result1.Error!.Message;
            error += result2.IsValid ? "" : result2.Error!.Message;

            HttpUtils.AddOptions(options, "redirect", "message", error);
            await HttpUtils.Redirect(req, res, options, "/");
        }
    }
}