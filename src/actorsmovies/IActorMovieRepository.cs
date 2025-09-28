namespace SimpleMDB;

public interface IActorMovieRepository
{
    public Task<PagedResult<Movie>> ReadAllMoviesByActor(int actorId, int page, int size);
    public Task<PagedResult<Actor>> ReadAllActorsByMovie(int movieId, int page, int size);
    public Task<List<Movie>> ReadAllMovies();
    public Task<List<Actor>> ReadAllActors();
    public Task<ActorMovie?> Create(int actorId, int movieId, string roleName);
    public Task<ActorMovie?> Delete(int id);

}