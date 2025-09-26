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

            router = new HttpRouter();
            router.Use(HttpUtils.ReadRequestFormData);

            router.AddGet("/", authController.LandingPageGet);
            router.AddGet("/users", userController.ViewAllGet);
            router.AddGet("/users/add", userController.AddGet);
            router.AddPost("/users/add", userController.AddPost);
            router.AddGet("/users/view", userController.ViewGet);
            router.AddGet("/users/edit", userController.EditGet);
            router.AddPost("/users/edit", userController.EditPost);
            router.AddGet("/users/remove", userController.RemoveGet);
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

                string rid = req.Headers["X-Request-ID"] ?? requestId.ToString().PadLeft(6, ' ');
                TimeSpan elapsed = DateTime.UtcNow - startTime;
                Console.WriteLine($"Request {rid}: {req.HttpMethod} {req.RawUrl} from {req.UserHostName} --> {res.StatusCode} ({res.ContentLength64} bytes in {elapsed.TotalMilliseconds} ms) error: \"{error}\"");
            }
        }
    }
}