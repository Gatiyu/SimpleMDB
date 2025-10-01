// MockUserService.cs
using System;
using System.Collections.Specialized;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SimpleMDB;

public class MockUserService : IUserService
{
    private readonly IUserRepository userRepository;

    public MockUserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
        _ = Create(new User(0, "Admin", "12345123451234512345", "", Roles.ADMIN)); // Default admin user
        // cabezadedospiernas
    }

    public async Task<Result<PagedResult<User>>> ReadAll(int page, int size)
    {
        var pagedResult = await userRepository.ReadAll(page, size);
        return pagedResult == null
            ? new Result<PagedResult<User>>(new Exception("No results found."))
            : new Result<PagedResult<User>>(pagedResult);
    }

    public async Task<Result<User>> Create(User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Role))
        {
            newUser.Role = Roles.USER;
        }
        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty."));
        }
        else if (string.IsNullOrWhiteSpace(newUser.Username) || newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot have more than 16 characters."));
        }
        else if (await userRepository.GetUserbyUsername(newUser.Username) != null)
        {
            return new Result<User>(new Exception("Username is already taken. Choose another username."));
        }

        if (string.IsNullOrWhiteSpace(newUser.Password))
        {
            return new Result<User>(new Exception("Password cannot be empty."));
        }
        else if (newUser.Password.Length < 16)
        {
            return new Result<User>(new Exception("Password cannot have less than 16 characters."));
        }

        if (!Roles.IsValid(newUser.Role))
        {
            return new Result<User>(new Exception("Role is not valid."));
        }

        newUser.Salt = Path.GetRandomFileName();;

        newUser.Password = Encode(newUser.Password + newUser.Salt);
        var createdUser = await userRepository.Create(newUser);
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
        if (string.IsNullOrWhiteSpace(newUser.Role))
        {
            newUser.Role = Roles.USER;
        }
        if (string.IsNullOrWhiteSpace(newUser.Username))
        {
            return new Result<User>(new Exception("Username cannot be empty."));
        }
        else if (string.IsNullOrWhiteSpace(newUser.Username) || newUser.Username.Length > 16)
        {
            return new Result<User>(new Exception("Username cannot have more than 16 characters."));
        }
        else { // Check if username is taken by another user
            var existing = await userRepository.GetUserbyUsername(newUser.Username);
            if (existing != null && existing.Id != id)
            {
                return new Result<User>(new Exception("Username is already taken. Choose another username."));
            }
        }

        if (string.IsNullOrWhiteSpace(newUser.Password))
        {
            return new Result<User>(new Exception("Password cannot be empty."));
        }
        else if (newUser.Password.Length < 16)
        {
            return new Result<User>(new Exception("Password cannot have less than 16 characters."));
        }

        if (!Roles.IsValid(newUser.Role))
        {
            return new Result<User>(new Exception("Role is not valid."));
        }

        newUser.Salt = Path.GetRandomFileName();
        newUser.Password = Encode(newUser.Password + newUser.Salt);

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

    public async Task<Result<string>> GetToken(string username, string password)
    {
        User? user = await userRepository.GetUserbyUsername(username);

        if (user != null && string.Equals(user.Password, Encode(password + user.Salt)))
        {
            return new Result<string>(Encode($"username={user.Username}&role={user.Role}&expires={DateTime.Now.AddMinutes(60)}"));
        }
        else
        {
            return new Result<string> (new Exception("Invalid username or password."));    
        }
    }

    public async Task<Result<NameValueCollection>> ValidateToken(string token)
    {
        if (!string.IsNullOrWhiteSpace(token))
        {
            NameValueCollection? claims = HttpUtility.ParseQueryString(Decode(token));

            //if(claims["expires"]< DateTime.Now) {// send null}
            return new Result<NameValueCollection>(claims);
        }
        else
        {
            var result = new Result<NameValueCollection>(new Exception("Token is null or empty. Invalid token."));
            return await Task.FromResult(result);
        }
    }
    public static string Encode(string plaintext)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintext)); // Tipo de hash raro
    }
    
    public static string Decode(string cyphertext)
    {       
       return Encoding.UTF8.GetString(Convert.FromBase64String(cyphertext)); // Tipo de hash raro
    }
}