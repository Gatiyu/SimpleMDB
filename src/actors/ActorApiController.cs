using System;
using System.Net;
using System.Text.Json;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace SimpleMDB
{
    /// <summary>
    /// REST API controller for managing actors in the SimpleMDB system.
    /// Implements a RESTful interface for CRUD operations on actors and their movie relationships.
    /// </summary>
    public class ActorApiController
    {
        private IActorService actorService;
        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions 
        { 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true 
        };

        public ActorApiController(IActorService actorService)
        {
            this.actorService = actorService;
        }

        /// <summary>
        /// GET /api/v1/actors?page=1&size=5
        /// Retrieves a paginated list of actors.
        /// </summary>
        /// <param name="req">The HTTP request containing query parameters for pagination</param>
        /// <param name="res">The HTTP response to write to</param>
        /// <param name="options">Additional options for request processing</param>
        /// <returns>
        /// 200 OK with JSON response containing:
        /// {
        ///   "data": Actor[],
        ///   "totalCount": number,
        ///   "page": number,
        ///   "size": number
        /// }
        /// 400 Bad Request if the request is invalid
        /// </returns>
        public async Task GetActors(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
            int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

            var result = await actorService.ReadAll(page, size);

            if (result.IsValid)
            {
                PagedResult<Actor> pagedResult = result.Value!;
                var response = new
                {
                    data = pagedResult.Values,
                    totalCount = pagedResult.TotalCount,
                    page,
                    size
                };
                
                string json = JsonSerializer.Serialize(response, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
            }
            else
            {
                var error = new { error = result.Error!.Message };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
            }
        }

        /// <summary>
        /// POST /api/v1/actors
        /// Creates a new actor in the system.
        /// </summary>
        /// <param name="req">The HTTP request containing the actor data in JSON format</param>
        /// <param name="res">The HTTP response to write to</param>
        /// <param name="options">Additional options for request processing</param>
        /// <returns>
        /// 201 Created with the created actor data
        /// 400 Bad Request if the actor data is invalid or malformed
        /// Request body format:
        /// {
        ///   "firstName": string,
        ///   "lastName": string,
        ///   "bio": string,
        ///   "rating": number
        /// }
        /// </returns>
        public async Task CreateActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            try
            {
                string requestBody = await HttpUtils.GetRequestBody(req);
                var actorData = JsonSerializer.Deserialize<Actor>(requestBody, jsonOptions);

                if (actorData == null)
                {
                    var error = new { error = "Invalid actor data" };
                    string json = JsonSerializer.Serialize(error, jsonOptions);
                    await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
                    return;
                }

                var result = await actorService.Create(actorData);

                if (result.IsValid)
                {
                    string json = JsonSerializer.Serialize(result.Value, jsonOptions);
                    await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.Created, json);
                }
                else
                {
                    var error = new { error = result.Error!.Message };
                    string json = JsonSerializer.Serialize(error, jsonOptions);
                    await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
                }
            }
            catch (JsonException)
            {
                var error = new { error = "Invalid JSON format" };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
            }
        }

        /// <summary>
        /// GET /api/v1/actors/{id}
        /// Retrieves a specific actor by their ID.
        /// </summary>
        /// <param name="req">The HTTP request</param>
        /// <param name="res">The HTTP response to write to</param>
        /// <param name="options">Additional options containing the actor ID</param>
        /// <returns>
        /// 200 OK with the actor data
        /// 404 Not Found if the actor doesn't exist
        /// 400 Bad Request if the ID is invalid
        /// Response format:
        /// {
        ///   "id": number,
        ///   "firstName": string,
        ///   "lastName": string,
        ///   "bio": string,
        ///   "rating": number
        /// }
        /// </returns>
        public async Task GetActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            int actorId = int.TryParse((string?)options["id"], out int id) ? id : -1;

            if (actorId == -1)
            {
                var error = new { error = "Invalid actor ID" };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
                return;
            }

            var result = await actorService.Read(actorId);

            if (result.IsValid)
            {
                string json = JsonSerializer.Serialize(result.Value, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
            }
            else
            {
                var error = new { error = result.Error!.Message };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotFound, json);
            }
        }

        /// <summary>
        /// PUT /api/v1/actors/{id}
        /// Updates an existing actor's information.
        /// </summary>
        /// <param name="req">The HTTP request containing updated actor data</param>
        /// <param name="res">The HTTP response to write to</param>
        /// <param name="options">Additional options containing the actor ID</param>
        /// <returns>
        /// 200 OK with the updated actor data
        /// 404 Not Found if the actor doesn't exist
        /// 400 Bad Request if the request body is invalid or the ID is invalid
        /// Request body format:
        /// {
        ///   "firstName": string,
        ///   "lastName": string,
        ///   "bio": string,
        ///   "rating": number
        /// }
        /// </returns>
        public async Task UpdateActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            int actorId = int.TryParse((string?)options["id"], out int id) ? id : -1;

            if (actorId == -1)
            {
                var error = new { error = "Invalid actor ID" };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
                return;
            }

            try
            {
                string requestBody = await HttpUtils.GetRequestBody(req);
                var actorData = JsonSerializer.Deserialize<Actor>(requestBody, jsonOptions);

                if (actorData == null)
                {
                    var error = new { error = "Invalid actor data" };
                    string json = JsonSerializer.Serialize(error, jsonOptions);
                    await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
                    return;
                }

                var result = await actorService.Update(actorId, actorData);

                if (result.IsValid)
                {
                    string json = JsonSerializer.Serialize(result.Value, jsonOptions);
                    await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
                }
                else
                {
                    var error = new { error = result.Error!.Message };
                    string json = JsonSerializer.Serialize(error, jsonOptions);
                    await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
                }
            }
            catch (JsonException)
            {
                var error = new { error = "Invalid JSON format" };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
            }
        }

        /// <summary>
        /// DELETE /api/v1/actors/{id}
        /// Deletes an actor from the system.
        /// </summary>
        /// <param name="req">The HTTP request</param>
        /// <param name="res">The HTTP response to write to</param>
        /// <param name="options">Additional options containing the actor ID</param>
        /// <returns>
        /// 204 No Content on successful deletion
        /// 404 Not Found if the actor doesn't exist
        /// 400 Bad Request if the ID is invalid
        /// </returns>
        public async Task DeleteActor(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            int actorId = int.TryParse((string?)options["id"], out int id) ? id : -1;

            if (actorId == -1)
            {
                var error = new { error = "Invalid actor ID" };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
                return;
            }

            var result = await actorService.Delete(actorId);

            if (result.IsValid)
            {
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NoContent, "");
            }
            else
            {
                var error = new { error = result.Error!.Message };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.NotFound, json);
            }
        }

        /// <summary>
        /// GET /api/v1/actors/{id}/movies?page=1&size=5
        /// Retrieves a paginated list of movies that a specific actor has participated in.
        /// </summary>
        /// <param name="req">The HTTP request containing pagination parameters</param>
        /// <param name="res">The HTTP response to write to</param>
        /// <param name="options">Additional options containing the actor ID</param>
        /// <returns>
        /// 200 OK with JSON response containing:
        /// {
        ///   "data": Movie[],
        ///   "totalCount": number,
        ///   "page": number,
        ///   "size": number
        /// }
        /// 404 Not Found if the actor doesn't exist
        /// 400 Bad Request if the ID or pagination parameters are invalid
        /// </returns>
        public async Task GetActorMovies(HttpListenerRequest req, HttpListenerResponse res, Hashtable options)
        {
            int actorId = int.TryParse((string?)options["id"], out int id) ? id : -1;
            int page = int.TryParse(req.QueryString["page"], out int p) ? p : 1;
            int size = int.TryParse(req.QueryString["size"], out int s) ? s : 5;

            if (actorId == -1)
            {
                var error = new { error = "Invalid actor ID" };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
                return;
            }

            // Assuming there's a method to get movies by actor ID in the service
            var result = await actorService.GetMoviesByActorId(actorId, page, size);

            if (result.IsValid)
            {
                PagedResult<Movie> pagedResult = result.Value!;
                var response = new
                {
                    data = pagedResult.Values,
                    totalCount = pagedResult.TotalCount,
                    page,
                    size
                };
                
                string json = JsonSerializer.Serialize(response, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.OK, json);
            }
            else
            {
                var error = new { error = result.Error!.Message };
                string json = JsonSerializer.Serialize(error, jsonOptions);
                await HttpUtils.RespondJson(req, res, options, (int)HttpStatusCode.BadRequest, json);
            }
        }
    }
}