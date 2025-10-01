using System;
using System.Net;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace SimpleMDB
{
    public class ActorController
    {
        private IActorService actorService;

        public ActorController(IActorService actorService)
        {
            this.actorService = actorService;
        }

        //GET /actor?page=1&size=5
        public async Task ViewAllActorsGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string message = req.QueryString["message"] ?? "";

            int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
            int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

            var result = await actorService.ReadAll(page, size);

            if (result.IsValid)
            {
                PagedResult<Actor> pagedResult = result.Value!;
                List<Actor> actors = pagedResult.Values;
                int actorCount = pagedResult.TotalCount;

                string html = ActorsHtmlTemplates.ViewAllActorsGet(actors, actorCount, page, size);
                html = HtmlTemplates.Base("SimpleMDB", "Actors View All Page", html, message);
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                await HttpUtils.Redirect(req, res, options, "/");
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
            string lastname = formData["firstname"] ?? "";
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

            int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

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

            int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

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
            int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

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

                await HttpUtils.Redirect(req, res, options, $"/actors/edit?uid={aid}");
            }
        }

        //GET /actors/remove?aid=1
        
          public async Task RemoveActorPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {

            int aid = int.TryParse(req.QueryString["aid"], out int u) ? u : -1;

            Result<Actor> result = await actorService.Delete(aid);

            if (result.IsValid)
            {
                HttpUtils.AddOptions(options, "redirect", "message", "Actor removed successfully!");
                await HttpUtils.Redirect(req, res, options, "/actors");
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                await HttpUtils.Redirect(req, res, options, "/actors");
            }
        }
    }
}