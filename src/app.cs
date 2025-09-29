using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace SimpleMDB
{
    public class App
    {
        private HttpListener server;
        private HttpRouter router;
        private int requestId;

        public App()
        {
            string host = "http://127.0.0.1:8080/";
            server = new HttpListener();
            server.Prefixes.Add(host);
            requestId = 0;

            Console.WriteLine("Server listening on... " + host);

            var userRepository = new MockUserRepository();
            var userService = new MockUserService(userRepository);
            var userController = new UserController(userService);
            var authController = new AuthController(userService);

            var actorRepository = new MockActorRepository();
            var actorService = new MockActorService(actorRepository);
            var actorController = new ActorController(actorService);

            var movieRepository = new MockMovieRepository();
            var movieService = new MockMovieService(movieRepository);
            var movieController = new MovieController(movieService);

            var actorMovieRepository = new MockActorMovieRepository(actorRepository, movieRepository);
            var actorMovieService = new MockActorMovieService(actorMovieRepository);
            var actorMovieController = new ActorMovieController(actorMovieService, actorService, movieService);

            router = new HttpRouter();
            router.Use(HttpUtils.ServeStaticFile);
            router.Use(HttpUtils.ReadRequestFormData);

            router.AddGet("/", authController.LandingPageGet);

            router.AddGet("/users", userController.ViewAllUsersGet);
            router.AddGet("/users/add", userController.AddUserGet);
            router.AddPost("/users/add", userController.AddUserPost);
            router.AddGet("/users/view", userController.ViewUserGet);
            router.AddGet("/users/edit", userController.EditUserGet);
            router.AddPost("/users/edit", userController.EditUserPost);
            router.AddPost("/users/remove", userController.RemoveUserPost);

            router.AddGet("/actors", actorController.ViewAllActorsGet);
            router.AddGet("/actors/add", actorController.AddActorGet);
            router.AddPost("/actors/add", actorController.AddActorPost);
            router.AddGet("/actors/view", actorController.ViewActorGet);
            router.AddGet("/actors/edit", actorController.EditActorGet);
            router.AddPost("/actors/edit", actorController.EditActorPost);
            router.AddPost("/actors/remove", actorController.RemoveActorPost);

            router.AddGet("/movies", movieController.ViewAllMoviesGet);
            router.AddGet("/movies/add", movieController.AddMovieGet);
            router.AddPost("/movies/add", movieController.AddMoviePost);
            router.AddGet("/movies/view", movieController.ViewMovieGet);
            router.AddGet("/movies/edit", movieController.EditMovieGet);
            router.AddPost("/movies/edit", movieController.EditMoviePost);
            router.AddPost("/movies/remove", movieController.RemoveMoviePost);

            router.AddGet("/actors/movies", actorMovieController.ViewAllMovieByActor);
            router.AddGet("/actors/movies/add", actorMovieController.AddActorByMovieGet);
            router.AddPost("/actors/movies/add", actorMovieController.AddMoviesByActor);
            router.AddPost("/actors/movies/remove", actorMovieController.RemoveMoviesByActor);

            router.AddGet("/movies/actors", actorMovieController.ViewAllActorsByMovie);
            router.AddGet("/movies/actors/add", actorMovieController.AddActorsByMovie);
            router.AddPost("/movies/actors/add", actorMovieController.AddActorsByPost);
            router.AddPost("/movies/actors/remove", actorMovieController.RemoveActorsByMovie);

        }

        public async Task Start()
        {
            server.Start();

            while (server.IsListening)
            {
                var ctx = await server.GetContextAsync();
                _ = HandleContextAsync(ctx);
            }
        }

        public void Stop()
        {
            server.Stop();
            server.Close();
        }

        private async Task HandleContextAsync(HttpListenerContext ctx)
        {
            var req = ctx.Request;
            var res = ctx.Response;
            var options = new Hashtable();

            var rid = req.Headers["X-Request-ID"] ?? requestId.ToString().PadLeft(6, ' ');
            var method = req.HttpMethod;
            var url = req.RawUrl;
            var remoteEndpoint = req.RemoteEndPoint;

            res.StatusCode = HttpRouter.RESPONSE_NOT_SENT_YET;
            DateTime startTime = DateTime.UtcNow;
            requestId++;
            string error = "";

            try
            {
                await router.Handle(req, res, options);
            }
            catch (Exception ex)
            {
                error = ex.ToString();

                if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
                {
                    if (Environment.GetEnvironmentVariable("DEVELOPMENT_MODE") != "Production")
                    {
                        string html = HtmlTemplates.Base("SimpleMDB", "Error Page", ex.ToString());
                        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, html);
                    }
                    else
                    {
                        string html = HtmlTemplates.Base("SimpleMDB", "Error Page", "An eror occurred.");
                        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.InternalServerError, html);
                    }
                }
            }
            finally
            {
                if (res.StatusCode == HttpRouter.RESPONSE_NOT_SENT_YET)
                {
                    string html = HtmlTemplates.Base("SimpleMDB", "Not Found page", "Resource was not found.");
                    await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.NotFound, html);
                }

                TimeSpan elapsed = DateTime.UtcNow - startTime;

                Console.WriteLine($"Request {rid}: {req.HttpMethod} {req.RawUrl} from {req.UserHostName} --> {res.StatusCode} ({res.ContentLength64} bytes [{res.ContentType}]in {elapsed.TotalMilliseconds} ms) error: \"{error}\"");
            }
        }
    }
}
