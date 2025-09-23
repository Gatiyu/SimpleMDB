namespace SimpleMDB;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string Role { get; set; }

    public User(int id = 0, string username = "", string password = "", string salt = "", string role = "")
    {
        Id = id;
        Username = username;
        Password = password;
        Salt = salt;
        Role = role;
    }
}