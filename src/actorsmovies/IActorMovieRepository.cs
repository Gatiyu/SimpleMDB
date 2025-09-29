namespace SimpleMDB;

public interface IActorMovieRepository
{
    public Task<PagedResult<(ActorMovie, Movie)>> ReadAllMoviesByActor(int actorId, int page, int size);
    public Task<PagedResult<(ActorMovie, Actor)>> ReadAllActorsByMovie(int movieId, int page, int size);
    public Task<List<Movie>> ReadAllMovies();
    public Task<List<Actor>> ReadAllActors();
    public Task<ActorMovie?> Create(int actorId, int movieId, string roleName);
    public Task<ActorMovie?> Delete(int id);

}