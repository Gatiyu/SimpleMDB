namespace SimpleMDB;

public interface IActorService
{
    public Task<Result<PagedResult<Actor>>> ReadAll(int page, int size);
    public Task<Result<Actor>> Create(Actor actor);
    public Task<Result<Actor>> Read(int id);
    public Task<Result<Actor>> Update(int id, Actor newActor);
    public Task<Result<Actor>> Delete(int id);
}