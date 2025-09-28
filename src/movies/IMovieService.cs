namespace SimpleMDB;

public interface IMovieService
{
    public Task<Result<PagedResult<Movie>>> ReadAll(int page, int size);
    public Task<Result<Movie>> Create(Movie movie);
    public Task<Result<Movie>> Read(int id);
    public Task<Result<Movie>> Update(int id, Movie newMovie);
    public Task<Result<Movie>> Delete(int id);
}