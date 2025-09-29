namespace SimpleMDB;

public class MockActorMovieRepository : IActorMovieRepository
{
    private IActorRepository actorRepository;
    private IMovieRepository movieRepository;
    private List<ActorMovie> actorMovies;
    private int idCounter;

    public MockActorMovieRepository(IActorRepository actorRepository, IMovieRepository movieRepository)
    {
        this.actorRepository = actorRepository;
        this.movieRepository = movieRepository;
        actorMovies = [];
        idCounter = 0;
        Random r = new Random();

        for (int aid = 0; aid < 100; aid++)
        {
            int count = r.Next(100);
            for (int j = 0; j < count; j++)
            {
                int mid = r.Next(100);
                actorMovies.Add(new ActorMovie(idCounter++, aid, mid, "Popo"));
            } 
        }
    }
        
    public async Task<PagedResult<(ActorMovie, Movie)>> ReadAllMoviesByActor(int actorId, int page, int pageSize)
    {
        List<ActorMovie> ams = actorMovies.FindAll((am) => am.ActorId == actorId);
        List<(ActorMovie, Movie)> movies = [];

        foreach (var am in ams)
        {
            var movie = (await movieRepository.Read(am.MovieId))!;
            movies.Add((am, movie));
        }

        int totalCount = movies.Count;
        int start = Math.Clamp((page - 1) * pageSize, 0, totalCount);
        int length = Math.Clamp(pageSize, 0, totalCount - start);
        List<(ActorMovie, Movie)> values = movies.Slice(start, length);
        var PagedResult = new PagedResult<(ActorMovie, Movie)>(values, totalCount);

        return await Task.FromResult(PagedResult);
    }

    public async Task<PagedResult<(ActorMovie, Actor)>> ReadAllActorsByMovie(int movieId, int page, int pageSize)
    {
        List<ActorMovie> ams = actorMovies.FindAll((am) => am.MovieId == movieId);
        List<(ActorMovie, Actor)> actors = [];

        foreach (var am in ams)
        {
            var actor = (await actorRepository.Read(am.ActorId))!;
            actors.Add((am, actor));
        }

        int totalCount = actors.Count;
        int start = Math.Clamp((page - 1) * pageSize, 0, totalCount);
        int length = Math.Clamp(pageSize, 0, totalCount - start);
        List<(ActorMovie, Actor)> values = actors.Slice(start, length);
        var PagedResult = new PagedResult<(ActorMovie, Actor)>(values, totalCount);
        
        return await Task.FromResult(PagedResult);
    }
    public async Task<List<Actor>> ReadAllActors()
    {
        var pagedResult = await actorRepository.ReadAll(1, int.MaxValue);
        return await Task.FromResult(pagedResult.Values);        
    }
    public async Task<List<Movie>> ReadAllMovies()
    {
        var pagedResult = await movieRepository.ReadAll(1, int.MaxValue);
        return await Task.FromResult(pagedResult.Values);
    }
    public async Task<ActorMovie?> Create(int actorId, int movieId, string roleName)
    {
        var actorMovie = new ActorMovie(idCounter++, actorId, movieId, roleName);
        actorMovies.Add(actorMovie);

        return await Task.FromResult(actorMovie);
    }
    public async Task<ActorMovie?> Delete(int id)
    {
        ActorMovie? actorMovie = actorMovies.Find((am) => am.Id == id);

        if (actorMovie != null)
        {
            actorMovies.Remove(actorMovie);
        }
        return await Task.FromResult(actorMovie!);
    }
}