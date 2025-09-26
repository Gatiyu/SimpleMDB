using System.Collections;
using System.Net;

namespace SimpleMDB;

public delegate Task HttpMiddleware(HttpListenerRequest req, HttpListenerResponse res, Hashtable options);
