namespace SimpleMDB;
using System.Net;
using System.Collections;
using System.Text;
public class AuthController
{
    public AuthController()
    {

    }
    public async Task LandingPageGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
    {
        string html = HtmlTemplates.Base("SimpleMDB", "Landing Page", "Hello World!2");
        await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
    }
    
}