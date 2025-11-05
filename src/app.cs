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
            string host = "http://127.0.0.1:8081/";
            server = new HttpListener();
            server.Prefixes.Add(host);
            requestId = 0;

            Console.WriteLine("Server listening on... " + host);

            var userRepository = new MySqlUserRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=12345;");
            var userService = new MockUserService(userRepository);
            var userController = new UserController(userService);
            var authController = new AuthController(userService);

            //var actorRepository = new MockActorRepository();
            var actorRepository = new MySqlActorRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=12345;");
            var actorService = new MockActorService(actorRepository);
            var actorController = new ActorController(actorService);
            var actorApiController = new ActorApiController(actorService);

            //var movieRepository = new MockMovieRepository();
            var movieRepository = new MySqlMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=12345;");
            var movieService = new MockMovieService(movieRepository);
            var movieController = new MovieController(movieService);

            //var actorMovieRepository = new MockActorMovieRepository(actorRepository, movieRepository);
            var actorMovieRepository = new MySqlActorMovieRepository("Server=localhost;Database=simplemdb;Uid=root;Pwd=12345;");
            var actorMovieService = new MockActorMovieService(actorMovieRepository);
            var actorMovieController = new ActorMovieController(actorMovieService, actorService, movieService);

            router = new HttpRouter();
            router.Use(HttpUtils.ServeStaticFile);
            router.Use(HttpUtils.ReadRequestFormData);

            router.AddGet("/", authController.LandingPageGet);
            router.AddGet("/register", authController.RegisterGet);
            router.AddPost("/register", authController.RegisterPost);
            router.AddGet("/login", authController.LoginGet);
            router.AddPost("/login", authController.LoginPost);
            router.AddPost("/logout", authController.LogoutPost);

            router.AddGet("/users", authController.CheckAdmin, userController.ViewAllUsersGet);
            router.AddGet("/users/add", authController.CheckAdmin, userController.AddUserGet);
            router.AddPost("/users/add", authController.CheckAdmin, userController.AddUserPost);
            router.AddGet("/users/view", authController.CheckAdmin, userController.ViewUserGet);
            router.AddGet("/users/edit", authController.CheckAdmin, userController.EditUserGet);
            router.AddPost("/users/edit", authController.CheckAdmin, userController.EditUserPost);
            router.AddPost("/users/remove", authController.CheckAdmin, userController.RemoveUserPost);

            // API v1 Routes
            router.AddGet("/api/v1/actors", actorApiController.GetActors);
            router.AddPost("/api/v1/actors", authController.CheckAuth, actorApiController.CreateActor);
            router.AddGet("/api/v1/actors/{id}", actorApiController.GetActor);
            router.AddPut("/api/v1/actors/{id}", authController.CheckAuth, actorApiController.UpdateActor);
            router.AddDelete("/api/v1/actors/{id}", authController.CheckAuth, actorApiController.DeleteActor);
            router.AddGet("/api/v1/actors/{id}/movies", actorApiController.GetActorMovies);

            // Web Routes
            router.AddGet("/actors", actorController.ViewAllActorsGet);
            router.AddGet("/actors/add", authController.CheckAuth, actorController.AddActorGet);
            router.AddPost("/actors/add", authController.CheckAuth, actorController.AddActorPost);
            router.AddGet("/actors/view", authController.CheckAuth, actorController.ViewActorGet);
            router.AddGet("/actors/edit", authController.CheckAuth, actorController.EditActorGet);
            router.AddPost("/actors/edit", authController.CheckAuth, actorController.EditActorPost);
            router.AddPost("/actors/remove", authController.CheckAuth, actorController.RemoveActorPost);

            router.AddGet("/movies", movieController.ViewAllMoviesGet);
            router.AddGet("/movies/add", authController.CheckAuth, movieController.AddMovieGet);
            router.AddPost("/movies/add", authController.CheckAuth, authController.CheckAuth, movieController.AddMoviePost);
            router.AddGet("/movies/view", authController.CheckAuth, movieController.ViewMovieGet);
            router.AddGet("/movies/edit", authController.CheckAuth, movieController.EditMovieGet);
            router.AddPost("/movies/edit", authController.CheckAuth, movieController.EditMoviePost);
            router.AddPost("/movies/remove", authController.CheckAuth, movieController.RemoveMoviePost);

            router.AddGet("/actors/movies", authController.CheckAuth, actorMovieController.ViewAllMovieByActor);
            router.AddGet("/actors/movies/add", authController.CheckAuth, actorMovieController.AddActorByMovieGet);
            router.AddPost("/actors/movies/add", authController.CheckAuth, actorMovieController.AddMoviesByActor);
            router.AddPost("/actors/movies/remove", authController.CheckAuth, actorMovieController.RemoveMoviesByActor);

            router.AddGet("/movies/actors", authController.CheckAuth, actorMovieController.ViewAllActorsByMovie);
            router.AddGet("/movies/actors/add", authController.CheckAuth, actorMovieController.AddActorsByMovie);
            router.AddPost("/movies/actors/add", authController.CheckAuth, actorMovieController.AddActorsByPost);
            router.AddPost("/movies/actors/remove", authController.CheckAuth, actorMovieController.RemoveActorsByMovie);

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