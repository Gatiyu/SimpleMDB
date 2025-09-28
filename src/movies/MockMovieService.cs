// MockMovieService.cs
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace SimpleMDB;

public class MockMovieService : IMovieService
{
    private readonly IMovieRepository movieRepository;

    public MockMovieService(IMovieRepository movieRepository)
    {
        this.movieRepository = movieRepository;
    }

    public async Task<Result<PagedResult<Movie>>> ReadAll(int page, int size)
    {
        var pagedResult = await movieRepository.ReadAll(page, size);
        return pagedResult == null
            ? new Result<PagedResult<Movie>>(new Exception("No results found."))
            : new Result<PagedResult<Movie>>(pagedResult);
    }

    public async Task<Result<Movie>> Create(Movie newMovie)
    {
        if (string.IsNullOrWhiteSpace(newMovie.Title))
        {
            return new Result<Movie>(new Exception("Title name cannot be empty."));
        }
        else if (newMovie.Title.Length > 256)
        {
            return new Result<Movie>(new Exception("Title cannot have more than 256 characters."));
        }
        else if(newMovie.Year > DateTime.Now.Year)
        {
            return new Result<Movie>(new Exception("Year cannot be in the future."));
        }

        var createdMovie = await movieRepository.Create(newMovie);
        return createdMovie == null
            ? new Result<Movie>(new Exception("Movie could not be created."))
            : new Result<Movie>(createdMovie);
    }

    public async Task<Result<Movie>> Read(int id)
    {
        var movie = await movieRepository.Read(id);
        return movie == null
            ? new Result<Movie>(new Exception("Movie could not be read."))
            : new Result<Movie>(movie);
    }

    public async Task<Result<Movie>> Update(int id, Movie newMovie)
    {
          if (string.IsNullOrWhiteSpace(newMovie.Title))
        {
            return new Result<Movie>(new Exception("Title name cannot be empty."));
        }
        else if (newMovie.Title.Length > 256)
        {
            return new Result<Movie>(new Exception("Title cannot have more than 256 characters."));
        }
        else if(newMovie.Year > DateTime.Now.Year)
        {
            return new Result<Movie>(new Exception("Year cannot be in the future."));
        }
        
        var movie = await movieRepository.Update(id, newMovie);
        return movie == null
            ? new Result<Movie>(new Exception("Movie could not be updated."))
            : new Result<Movie>(movie);
    }

    public async Task<Result<Movie>> Delete(int id)
    {
        var movie = await movieRepository.Delete(id);
        return movie == null
            ? new Result<Movie>(new Exception("Movie could not be deleted."))
            : new Result<Movie>(movie);
    }
}