namespace SimpleMDB;

public class Roles
{
    public static readonly string ADMIN = "Admin";
    public static readonly string USER = "User";

    public static readonly string[] ROLES = { ADMIN, USER };

    public static bool Check(string? role) {return ROLES.Contains(role); }
}