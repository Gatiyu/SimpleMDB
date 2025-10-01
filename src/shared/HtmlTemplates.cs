namespace SimpleMDB;

public class HtmlTemplates
{
    public static string Base(string title, string header, string content, string message = "")
    {
        return $@"
        <html>
         <head>
            <title>{title}</title>
            <link rel=""icon"" type=""image/x-icon"" href=""/favicon.ico"">
            <link rel=""stylesheet"" type=""text/css"" href=""/styles/main.css"">
            <script type=""text/javascript"" src=""/scripts/main.js"" defer></script>
        </head>
        <body>
            <nav class=""nav-menu"">
                <ul>
                    <li><a href=""/"">Home</a></li>
                    <li><a href=""/users"">Users</a></li>
                    <li><a href=""/actors"">Actors</a></li>
                    <li><a href=""/movies"">Movies</a></li>
                </ul>
            </nav>
            <h1 class=""header"">{header}</h1>
            <div class=""content"">{content}</div>
            <div class=""message"">{message}</div>                
        </body>
        </html>";
    }
}