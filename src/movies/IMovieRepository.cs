namespace SimpleMDB;

public interface IMovieRepository
{
    public Task<PagedResult<Movie>> ReadAll(int page, int size);
    public Task<Movie?> Create(Movie Movie);
    public Task<Movie?> Read(int id);
    public Task<Movie?> Update(int id, Movie newMovie);
    public Task<Movie?> Delete(int id);
}