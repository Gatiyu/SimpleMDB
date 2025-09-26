// MockActorService.cs
using System;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace SimpleMDB;

public class MockActorService : IActorService
{
    private readonly IActorRepository actorRepository;

    public MockActorService(IActorRepository actorRepository)
    {
        this.actorRepository = actorRepository;
    }

    public async Task<Result<PagedResult<Actor>>> ReadAll(int page, int size)
    {
        var pagedResult = await actorRepository.ReadAll(page, size);
        return pagedResult == null
            ? new Result<PagedResult<Actor>>(new Exception("No results found."))
            : new Result<PagedResult<Actor>>(pagedResult);
    }

    public async Task<Result<Actor>> Create(Actor newActor)
    {
        if (string.IsNullOrWhiteSpace(newActor.FirstName))
        {
            return new Result<Actor>(new Exception("First name cannot be empty."));
        }
        else if (newActor.FirstName.Length > 16)
        {
            return new Result<Actor>(new Exception("First cannot have more than 16 characters."));
        }
        else if (string.IsNullOrWhiteSpace(newActor.LastName))
        {
            return new Result<Actor>(new Exception("Last name cannot be empty."));
        }
        else if(newActor.LastName.Length > 16)
        {
            return new Result<Actor>(new Exception("Last name cannot have more than 16 characters."));
        }

        var createdActor = await actorRepository.Create(newActor);
        return createdActor == null
            ? new Result<Actor>(new Exception("Actor could not be created."))
            : new Result<Actor>(createdActor);
    }

    public async Task<Result<Actor>> Read(int id)
    {
        var actor = await actorRepository.Read(id);
        return actor == null
            ? new Result<Actor>(new Exception("Actor could not be read."))
            : new Result<Actor>(actor);
    }

    public async Task<Result<Actor>> Update(int id, Actor newActor)
    {
        var actor = await actorRepository.Update(id, newActor);
        return actor == null
            ? new Result<Actor>(new Exception("Actor could not be updated."))
            : new Result<Actor>(actor);
    }

    public async Task<Result<Actor>> Delete(int id)
    {
        var actor = await actorRepository.Delete(id);
        return actor == null
            ? new Result<Actor>(new Exception("Actor could not be deleted."))
            : new Result<Actor>(actor);
    }
}