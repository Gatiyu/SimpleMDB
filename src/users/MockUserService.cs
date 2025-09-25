// MockUserService.cs
using System;
using System.Threading.Tasks;

namespace SimpleMDB;

public class MockUserService : IUserService
{
    private readonly IUserRepository userRepository;

    public MockUserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<Result<PagedResult<User>>> ReadAll(int page, int size)
    {
        var pagedResult = await userRepository.ReadAll(page, size);
        return pagedResult == null
            ? new Result<PagedResult<User>>(new Exception("No results found."))
            : new Result<PagedResult<User>>(pagedResult);
    }

    public async Task<Result<User>> Create(User user)
    {
        var createdUser = await userRepository.Create(user);
        return createdUser == null
            ? new Result<User>(new Exception("User could not be created."))
            : new Result<User>(createdUser);
    }

    public async Task<Result<User>> Read(int id)
    {
        var user = await userRepository.Read(id);
        return user == null
            ? new Result<User>(new Exception("User could not be read."))
            : new Result<User>(user);
    }

    public async Task<Result<User>> Update(int id, User newUser)
    {
        var user = await userRepository.Update(id, newUser);
        return user == null
            ? new Result<User>(new Exception("User could not be updated."))
            : new Result<User>(user);
    }

    public async Task<Result<User>> Delete(int id)
    {
        var user = await userRepository.Delete(id);
        return user == null
            ? new Result<User>(new Exception("User could not be deleted."))
            : new Result<User>(user);
    }
}