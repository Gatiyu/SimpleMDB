using System.Net;
using System.Text;
using System.Collections;
using System.Threading.Tasks;

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
        string html = $@"
        <nav>
            <ul>
                <li><a href=""/register"">Register</a></li>
                <li><a href=""/login"">Login</a></li>
                <li><a href=""/logout"">Logout</a></li>
                <li><a href=""/users"">users</a></li>
                <li><a href=""/actors"">Actors</a></li>
                <li><a href=""/movies"">Movies</a></li>
            </ul>
        </nav>
        ";
        html = HtmlTemplates.Base("SimpleMDB", "Landing Page", html);

        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
    }
}