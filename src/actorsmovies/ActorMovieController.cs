using System.Collections;
using System.Net;

namespace SimpleMDB;

public class ActorMovieController
{
    private IActorMovieService actorMovieService;
    private IActorService actorService;
    private IMovieService movieService;
    // GET /actors/movies?aid=18&page=1&size=5
    public ActorMovieController(IActorMovieService actorMovieService, IActorService actorService, IMovieService movieService)
    {
        this.actorMovieService = actorMovieService;
        this.actorService = actorService;
        this.movieService = movieService;
    }
    public async Task ViewAllMovieByActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";
        int aid = int.TryParse(req.QueryString["aid"], out int a) ? a : 1;
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

            string html = ActorMovieHtmlTemplates.ViewAllMovieByActor(actor, movies, totalCount, page, size);
            string content = HtmlTemplates.Base("SimpleMDB", "View All Movies By Actor Page", html, message);

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
    