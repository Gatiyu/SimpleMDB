namespace SimpleMDB;

public class Actor
{
    public int Id { get; set; }
       public string FirstName { get; set; } = "";
       public string LastName { get; set; } = "";
       public string Bio { get; set; } = "";
    public float rating { get; set; }

    public Actor(int id = 0, string firstName = "", string lastName = "", string bio = "", float ranking = 0)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Bio = bio;
        rating = ranking;
    }

    public override string ToString()
    {
          return $"Actor(Id={Id}, FirstName={FirstName}, LastName={LastName}, Bio={Bio}, Rating={rating})";
    }
}