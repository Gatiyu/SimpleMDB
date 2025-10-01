using System;
using System.Net;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace SimpleMDB
{
    public class UserController
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        //GET /user?page=1&size=5
        public async Task ViewAllUsersGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string message = req.QueryString["message"] ?? "";

            int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
            int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

            var result = await userService.ReadAll(page, size);

            if (result.IsValid)
            {
                PagedResult<User> pagedResult = result.Value!;
                List<User> users = pagedResult.Values;
                int userCount = pagedResult.TotalCount;

                string html = UsersHtmlTemplates.ViewAllUsersGet(users, userCount, page, size);
                html = HtmlTemplates.Base("SimpleMDB", "Users View All Page", html, message);
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                await HttpUtils.Redirect(req, res, options, "/");
            }
        }

        // GET/users/add
        public async Task AddUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string username = req.QueryString["username"] ?? "";
            string message = req.QueryString["message"] ?? "";
            string role = req.QueryString["role"] ?? "";

            string html = UsersHtmlTemplates.AddUserGet(username, role);

            html = HtmlTemplates.Base("SimpleMDB", "Users Add Page", html, message);
            await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.Created, html);
        }

        // POST /users/add

        public async Task AddUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            var formData = (NameValueCollection?)options["req.form"] ?? [];

            string username = formData["username"] ?? "";
            string password = formData["password"] ?? "";
            string role = formData["role"] ?? Roles.USER;

            Console.WriteLine($"username={username}");
            User newUser = new User(0, username, password, "", role);

            var result = await userService.Create(newUser);

            if (result.IsValid)
            {
                HttpUtils.AddOptions(options, "redirect", "message", "User added successfully");

                await HttpUtils.Redirect(req, res, options, "/users"); //PRG
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                HttpUtils.AddOptions(options, "redirect", "username", username);
                HttpUtils.AddOptions(options, "redirect", "role", role);


                await HttpUtils.Redirect(req, res, options, "/users/add");
            }
        }

        //GET /users/view?uid
        public async Task ViewUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string message = req.QueryString["message"] ?? "";

            int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

            Result<User> result = await userService.Read(uid);

            if (result.IsValid)
            {
                User user = result.Value!;

                string html = UsersHtmlTemplates.ViewUserGet(user);

                html = HtmlTemplates.Base("SimpleMDB", "Users View Page", html, message);
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
            }
        }

        // GET /users/edit?uid=1
        public async Task EditUserGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            string message = req.QueryString["message"] ?? "";

            int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

            Result<User> result = await userService.Read(uid);

            if (result.IsValid)
            {
                User user = result.Value!;

                string html = UsersHtmlTemplates.EditUserGet(user, uid);

                html = HtmlTemplates.Base("SimpleMDB", "Users Edit Page", html, message);
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
            }

        }

        // POST /users/edit?uid=1

        public async Task EditUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

            var formData = (NameValueCollection?)options["req.form"] ?? [];

            string username = formData["username"] ?? "";
            string password = formData["password"] ?? "";
            string role = formData["role"] ?? Roles.USER;

            Console.WriteLine($"username={username}");
            User newUser = new User(0, username, password, "", role);

            var result = await userService.Update(uid, newUser);

            Console.WriteLine("#######");
            Console.WriteLine(newUser);
            Console.WriteLine(result);

            if (result.IsValid)
            {
                HttpUtils.AddOptions(options, "redirect", "message", "User edited successfully!");
                await HttpUtils.Redirect(req, res, options, "/users");
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                await HttpUtils.Redirect(req, res, options, $"/users/edit?uid={uid}");
            }
        }

        //GET /users/remove?uid=1
        
          public async Task RemoveUserPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {

            int uid = int.TryParse(req.QueryString["uid"], out int u) ? u : -1;

            Result<User> result = await userService.Delete(uid);

            if (result.IsValid)
            {
                HttpUtils.AddOptions(options, "redirect", "message", "User removed successfully!");
                await HttpUtils.Redirect(req, res, options, "/users");
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                await HttpUtils.Redirect(req, res, options, "/users");
            }
        }
    }
}