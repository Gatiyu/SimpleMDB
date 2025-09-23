namespace SimpleMDB;

public interface UIRespository
{
    public Task<PagedResult<User>> ReadAll(int page, int pageSize);
    public Task<User?> Create(User user);
    public Task<User?> ReadById(int id);
    public Task<User?> Update(int id, User newUser);
    public Task<User?> Delete(int id);

}