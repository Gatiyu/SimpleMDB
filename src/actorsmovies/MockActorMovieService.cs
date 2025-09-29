namespace SimpleMDB;

public class MockActorMovieService : IActorMovieService
{
    private IActorMovieRepository actorMovieRepository;

    public MockActorMovieService(IActorMovieRepository actorMovieRepository)
    {
        this.actorMovieRepository = actorMovieRepository;
    }
    public async Task<Result<PagedResult<(ActorMovie, Movie)>>> ReadAllMoviesByActor(int actorId, int page, int size)
    {
        var pagedResult = await actorMovieRepository.ReadAllMoviesByActor(actorId, page, size);

        var result = (pagedResult == null) ?
        new Result<PagedResult<(ActorMovie, Movie)>>(new Exception("No movies by actor results found.")) :
        new Result<PagedResult<(ActorMovie, Movie)>>(pagedResult);

        return await Task.FromResult(result);
    }
    public async Task<Result<PagedResult<(ActorMovie, Actor)>>> ReadAllActorsByMovie(int movieId, int page, int size)
    {
        var pagedResult = await actorMovieRepository.ReadAllActorsByMovie(movieId, page, size);

        var result = (pagedResult == null) ?
        new Result<PagedResult<(ActorMovie, Actor)>>(new Exception("No actors by movies results found.")) :
        new Result<PagedResult<(ActorMovie, Actor)>>(pagedResult);

        return await Task.FromResult(result);
    }
    public async Task<Result<List<Actor>>> ReadAllActors()
    {
        var pagedResult = await actorMovieRepository.ReadAllActors();

        var result = (pagedResult == null) ?
        new Result<List<Actor>>(new Exception("No actors results found.")) :
        new Result<List<Actor>>(pagedResult);

        return await Task.FromResult(result);
    }

    public async Task<Result<List<Movie>>> ReadAllMovies()
    {
        var pagedResult = await actorMovieRepository.ReadAllMovies();

        var result = (pagedResult == null) ?
        new Result<List<Movie>>(new Exception("No movies results found.")) :
        new Result<List<Movie>>(pagedResult);

        return await Task.FromResult(result);
    }
    public async Task<Result<ActorMovie>> Create(int actorId, int movieId, string roleName)
    {
        ActorMovie? actorMovie = await actorMovieRepository.Create(actorId, movieId, roleName);

        var result = (actorMovie == null) ?
        new Result<ActorMovie>(new Exception("ActorMovie could not be created.")) :
        new Result<ActorMovie>(actorMovie!);

        return await Task.FromResult(result);
    }
    public async Task<Result<ActorMovie>> Delete(int id)
    {
        ActorMovie? actorMovie = await actorMovieRepository.Delete(id);

        var result = (actorMovie == null) ?
        new Result<ActorMovie>(new Exception("ActorMovie could not be deleted.")) :
        new Result<ActorMovie>(actorMovie!);

        return await Task.FromResult(result);
    }

}