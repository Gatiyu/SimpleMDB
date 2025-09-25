namespace SimpleMDB;

public class User(int id = 0, string username = "", string password = "", string salt = "", string role = "")
{
    public int Id { get; set; } = id;
    public string Username { get; set; } = username;
    public string Password { get; set; } = password;
    public string Salt { get; set; } = salt;
    public string Role { get; set; } = role;
    public User? Value { get; internal set; }
}