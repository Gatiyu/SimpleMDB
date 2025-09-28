namespace SimpleMDB;

public interface IActorMovieService
{
    public Task<Result<PagedResult<Movie>>> ReadAllMoviesByActor(int actorId, int page, int size);
    public Task<Result<PagedResult<Actor>>> ReadAllActorsByMovie(int movieId, int page, int size);
    public Task<Result<List<Movie>>> ReadAllMovies();
    public Task<Result<List<Actor>>> ReadAllActors();
    public Task<Result<ActorMovie>> Create(int actorId, int movieId, string roleName);
    public Task<Result<ActorMovie>> Delete(int id);

}