using System.Reflection.Metadata;
using Microsoft.VisualBasic;
using SimpleMDB;

namespace SimpleMDB;

public class MockUserRepository : UIRespository
{
    private List<User> users;
    private int idCount;

    public MockUserRepository()
    {
        users = [];
        idCount = 0;
    }
    public async Task<PagedResult<User>> ReadAll(int page, int pageSize)
    {
        int totalCount = users.Count;
        int start = Math.Clamp((page - 1) * pageSize, 0, totalCount);
        int length = Math.Clamp(pageSize, 0, totalCount - start);
        List<User> values = users.GetRange(start, length);
        var pagedResult = new PagedResult<User>(values, totalCount);
        return await Task.FromResult(pagedResult);
    }
    public async Task<User?> Create(User user)
    {
        user.Id = idCount++;
        users.Add(user);
        return await Task.FromResult(user);
    }
    public async Task<User?> ReadById(int id)
    {
        User? user = users.FirstOrDefault((u) => u.Id == id);
        return await Task.FromResult(user);
    }
    public async Task<User?> Update(int id, User newUser)
    {
        User? user = users.FirstOrDefault((u) => u.Id == id);

        if (user != null)
        {
            user.Username = newUser.Username;
            user.Password = newUser.Password;
            user.Salt = newUser.Salt;
            user.Role = newUser.Role;
        }
        return await Task.FromResult(user);
    }
    public async Task<User?> Delete(int id)
    {
        User? user = users.FirstOrDefault((u) => u.Id == id);

        if (user != null)
        {
            users.Remove(user);
        }
        return await Task.FromResult(user);
    }

}