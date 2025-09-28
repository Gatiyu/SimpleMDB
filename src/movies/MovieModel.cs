namespace SimpleMDB;

public class Movie
{
    public int Id { get; set; }
       public string Title { get; set; }
       public int Year { get; set; }
       public string Description { get; set; }
    public float rating { get; set; }

    public Movie(int id = 0, string title = "", int year = 2025, string description = "", float ranking = 0)
    {
        Id = id;
        Title = title;
        Year = year;
        Description = description;
        rating = ranking;
    }

    public override string ToString()
    {
          return $"Movie(Id={Id}, Title={Title}, Year={Year}, Description={Description}, Rating={rating})";
    }
}