using System;
using System.Net;
using System.Collections;
using System.Threading.Tasks;

namespace SimpleMDB
{
    public class UserController
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task ViewAllGet(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        { 
            int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
            int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

            var result = await userService.ReadAll(page, size);

            if(result.IsValid)
            {
                PagedResult<User> pagedResult = result.Value!;
                List<User> users = pagedResult.Values;
                int userCount = pagedResult.TotalCount;
                int pageCount = (int)Math.Ceiling((double)userCount / size);

                string rows = "";

                foreach(var user in users)
                {
                    rows += @$"
                    <tr>
                        <td>{user.Id}</td>
                        <td>{user.Username}</td>
                        <td>{user.Password}</td>
                        <td>{user.Salt}</td>
                        <td>{user.Role}</td>
                    </tr>
                    ";
                }

                string html = $@"
                <table border=""1"">
                    <thead>
                        <th>Id</th>
                        <th>Username</th>
                        <th>Password</th>
                        <th>Salt</th>
                        <th>Role</th>
                    </thead>
                    <tbody>
                        {rows}
                    </tbody>
                </table>
                <div>
                    <a href=""?page=1&size={size}"">First</a>
                    <a href=""?page={page-1}&size={size}"">Prev</a>
                    <span>{page} / {pageCount}</span>
                    <a href=""?page={page+1}&size={size}"">Next</a>
                    <a href=""?page={pageCount}&size={size}"">Last</a>
                </div>
                ";
                html = HtmlTemplates.Base("SimpleMDB", "Users View All Page", html);
                await HttpUtils.Respond(req, res, options, (int)HttpStatusCode.OK, html);
            }
        }
    }
}