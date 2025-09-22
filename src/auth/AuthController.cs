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
        string html = HtmlTemplates.Base("SimpleMDB","Landing Page", "Hello World");
        byte[] content = Encoding.UTF8.GetBytes(html);
        res.StatusCode = (int)HttpStatusCode.OK;
        res.ContentEncoding = Encoding.UTF8;
        res.ContentType = "text/html";
        res.ContentLength64 = content.LongLength;
        await res.OutputStream.WriteAsync(content);
        res.Close();
    }
    
}