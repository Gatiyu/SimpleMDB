using System.Net;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Web;

namespace SimpleMDB;

public class AuthController
{
    private IUserService userService;

    public AuthController(IUserService userService)
    {
        this.userService = userService;
    }


    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string message = req.QueryString["message"] ?? "";

        string html = $@"
        <nav>
            <ul>
                <li><a href=""/register"">Register</a></li>
                <li><a href=""/login"">Login</a></li>
                <li>
                <form action=""/logout"" method=""POST"">
                   <input type=""submit"" value=""Logout"">
                   </form>
                </li>
                <li><a href=""/users"">users</a></li>
                <li><a href=""/actors"">Actors</a></li>
                <li><a href=""/movies"">Movies</a></li>
            </ul>
        </nav>
        ";

        html = HtmlTemplates.Base("SimpleMDB", "Landing Page", html, message);
        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
    }

    // GET /register
    public async Task RegisterGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var returnUrl = req.QueryString["returnUrl"] ?? "";
        var rQuery = string.IsNullOrWhiteSpace(returnUrl) ? "" : $"?returnUrl={HttpUtility.UrlEncode(returnUrl)}";

        string message = req.QueryString["message"] ?? "";
        string username = req.QueryString["username"] ?? "";

        string html = $@"
        <h2>Register</h2>
        <form action=""/register{rQuery}"" method=""POST"">
            <label for=""username"">Username:</label><br>
            <input id=""username"" name=""username"" type=""text"" placeholder=""Username"" value=""{username}""><br>
            <label for=""password"">Password:</label><br>
            <input id=""password"" name=""password"" type=""password"" placeholder=""Password""required>
            <label for=""cpassword"">Confirm Password:</label><br>
            <input id=""cpassword"" name=""cpassword"" type=""cpassword"" placeholder=""ConfirmPassword"">
            <input type=""submit"" value=""Register"">
        </form>
        ";

        html = HtmlTemplates.Base("SimpleMDB", "Register Page", html, message);
        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
    }

    //POST /register
    public async Task RegisterPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var returnUrl = req.QueryString["returnUrl"] ?? "";

        var formdata = (NameValueCollection?)options["req.form"] ?? [];
        var username = formdata["username"] ?? "";
        var password = formdata["password"] ?? "";
        var cpassword = formdata["cpassword"] ?? "";

        if (password != cpassword)
        {
            HttpUtils.AddOptions(options, "redirect", "message", "Passwords do not match");
            HttpUtils.AddOptions(options, "redirect", "username", username);
            HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);


            await HttpUtils.Redirect(req, res, options, $"/register");
        }
        else
        {
            User newUser = new User(0, username, password, "", "");
            var result = await userService.Create(newUser);

            if (result.IsValid)
            {

                HttpUtils.AddOptions(options, "redirect", "message", "User registred successfully. Please login.");
                HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);

                await HttpUtils.Redirect(req, res, options, $"/login");
            }
            else
            {
                HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
                HttpUtils.AddOptions(options, "redirect", "username", username);
                HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);

                await HttpUtils.Redirect(req, res, options, $"/register");
            }
        }
    }
    //GET /login
    public async Task LoginGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var returnUrl = req.QueryString["returnUrl"] ?? "";
        var rQuery = string.IsNullOrWhiteSpace(returnUrl) ? "" : $"?returnUrl={HttpUtility.UrlEncode(returnUrl)}";

        string message = req.QueryString["message"] ?? "";
        string username = req.QueryString["username"] ?? "";

        string html = $@"
        <h2>Register</h2>
        <form action=""/login{rQuery}"" method=""POST"">
            <label for=""username"">Username:</label><br>
            <input id=""username"" name=""username"" type=""text"" placeholder=""Username"" value=""{username}""><br>
            <label for=""password"">Password:</label><br>
            <input id=""password"" name=""password"" type=""password"" placeholder=""Password""required>
            <input type=""submit"" value=""Login"">
        </form>
        ";

        html = HtmlTemplates.Base("SimpleMDB", "Login Page", html, message);
        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
    }

    //POST /login
    public async Task LoginPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        var returnUrl = req.QueryString["returnUrl"] ?? "/";

        var formdata = (NameValueCollection?)options["req.form"] ?? [];
        var username = formdata["username"] ?? "";
        var password = formdata["password"] ?? "";

        var result = await userService.GetToken(username, password);

        if (result.IsValid)
        {
            string token = result.Value!;
            HttpUtils.AddOptions(options, "redirect", "message", "User login successfully.");

            // Create a cookie and set some common attributes. Use AppendCookie so multiple Set-Cookie headers
            // are preserved even when we redirect the response.
            var cookie = new Cookie("token", token, "/")
            {
                HttpOnly = true,
                // You can set Secure = true when running over HTTPS
                Expires = DateTime.Now.AddMinutes(60)
            };

            res.AppendCookie(cookie);
            // Also add Authorization header for immediate use if needed.
            res.AddHeader("Authorization", $"Bearer {token}");

            await HttpUtils.Redirect(req, res, options, $"{returnUrl}");
        }
        else
        {
            HttpUtils.AddOptions(options, "redirect", "message", result.Error!.Message);
            HttpUtils.AddOptions(options, "redirect", "username", username);
            HttpUtils.AddOptions(options, "redirect", "returnUrl", returnUrl);

            await HttpUtils.Redirect(req, res, options, "/login");
        }
    }

    //POST /logout
    public async Task LogoutPost(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
    // Clear the token cookie by setting an expired cookie. Use AppendCookie so redirect keeps the header.
    var expired = new Cookie("token", "", "/") { Expires = DateTime.Now.AddDays(-1) };
    res.AppendCookie(expired);
    res.AddHeader("WWW-Authenticate", @"Bearer error=""invalid_token"", error_description=""The usr logged out.""");

        HttpUtils.AddOptions(options, "redirect", "message", "User logged out sucessfully.");

        await HttpUtils.Redirect(req, res, options, "/login");
    }
    public async Task CheckAuth(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        await Task.CompletedTask;
    }

    public async Task CheckAdmin(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        await Task.CompletedTask;
    }
}
