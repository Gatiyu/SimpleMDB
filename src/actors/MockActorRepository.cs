using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleMDB;

public class MockActorRepository : IActorRepository
{
    private List<Actor> actors;
    private int idCount;

    public MockActorRepository()
    {
        actors = [];
        idCount = 0;

        var firstNames = new[]
        {
            "James", "Emma", "Liam", "Olivia", "Noah", "Ava", "William", "Sophia", "Michael", "Isabella",
            "Alexander", "Mia", "Daniel", "Charlotte", "Henry", "Amelia", "Joseph", "Harper", "David", "Evelyn",
            "Thomas", "Emily", "Charles", "Elizabeth", "John", "Sofia", "Benjamin", "Abigail", "Lucas", "Ella",
            "Mason", "Avery", "Logan", "Grace", "Jacob", "Lily", "Jackson", "Victoria", "Ethan", "Chloe",
            "Samuel", "Madison", "Sebastian", "Aria", "Matthew", "Scarlett", "Aiden", "Zoe", "Luke", "Penelope",
            "Gabriel", "Layla", "Anthony", "Riley", "Isaac", "Ellie", "Owen", "Hannah", "Nathan", "Julia",
            "Caleb", "Addison", "Andrew", "Aubrey", "Jack", "Natalie", "Joshua", "Camila", "Christopher", "Leah",
            "Nicholas", "Lillian", "Ryan", "Lucy", "Jayden", "Maya", "Dylan", "Sadie", "Connor", "Mila",
            "Evan", "Nora", "Isaiah", "Violet", "Aaron", "Savannah", "Eli", "Brooklyn", "Hunter", "Claire",
            "Julian", "Skylar", "Levi", "Alexa", "Grayson", "Audrey", "Elijah", "Caroline", "Carter", "Hazel"
        };

        var lastNames = new[]
        {
            "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
            "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
            "Lee", "Perez", "Thompson", "White", "Harris", "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson",
            "Walker", "Young", "Allen", "King", "Wright", "Scott", "Torres", "Nguyen", "Hill", "Flores",
            "Green", "Adams", "Nelson", "Baker", "Hall", "Rivera", "Campbell", "Mitchell", "Carter", "Roberts",
            "Gomez", "Phillips", "Evans", "Turner", "Diaz", "Parker", "Cruz", "Edwards", "Collins", "Reyes",
            "Stewart", "Morris", "Morales", "Murphy", "Cook", "Rogers", "Gutierrez", "Ortiz", "Morgan", "Cooper",
            "Peterson", "Bailey", "Reed", "Kelly", "Howard", "Ramos", "Kim", "Cox", "Ward", "Richardson",
            "Watson", "Brooks", "Chavez", "Wood", "James", "Bennett", "Gray", "Mendoza", "Ruiz", "Hughes",
            "Price", "Alvarez", "Castillo", "Sanders", "Patel", "Myers", "Long", "Ross", "Foster", "Jimenez"
        };

        var highlights = new[]
        {
            "A versatile actor known for compelling performances in drama and action films.",
            "An award-winning performer with a knack for bringing complex characters to life.",
            "A rising star celebrated for their natural charisma and emotional depth.",
            "A seasoned actor with decades of experience in theater and cinema.",
            "A talented performer who excels in both comedic and dramatic roles.",
            "A dynamic actor known for their intense portrayals and captivating screen presence.",
            "A beloved figure in independent films, praised for their authenticity.",
            "A multi-talented star who shines in both blockbuster hits and indie projects.",
            "An actor with a unique ability to connect with audiences through heartfelt performances.",
            "A veteran of the stage and screen, known for their powerful delivery."
        };

        var random = new Random();

        for (int i = 0; i < firstNames.Length; i++)
        {
            float rating = (float)Math.Round(random.NextDouble() * 10, 1); // Ratings between 6.0 and 10.0
            string career = highlights[i % highlights.Length];
            string bio = $"{firstNames[i]} {lastNames[i]} is an actor known for their workk in {career}. " +
                         "Over their career, they have garnered critical acclaim and a dedicated fanbase.";

            actors.Add(new Actor(idCount++, firstNames[i],lastNames[i], bio, rating));
        }
    }
    

    public async Task<PagedResult<Actor>> ReadAll(int page, int size)
    {
        int totalCount = actors.Count;
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start);

        List<Actor> values = actors.GetRange(start, length);

        var pagedResult = new PagedResult<Actor>(values, totalCount);

        return await Task.FromResult(pagedResult);
    }

    public async Task<Actor?> Create(Actor Actor)
    {
        Actor.Id = idCount++;
        actors.Add(Actor);
        Console.WriteLine(Actor);

        return await Task.FromResult(Actor);
    }

    public async Task<Actor?> Read(int id)
    {
        Actor? Actor = actors.FirstOrDefault((u) => u.Id == id);
        return await Task.FromResult(Actor);
    }

    public async Task<Actor?> Update(int id, Actor newActor)
    {
        Actor? actor = actors.FirstOrDefault((u) => u.Id == id);

        if (actor != null)
        {
            actor.FirstName = newActor.FirstName;
            actor.LastName = newActor.LastName;
            actor.Bio = newActor.Bio;
            actor.rating = newActor.rating;
        }
        return await Task.FromResult(actor);
    }

    public async Task<Actor?> Delete(int id)
    {
        Actor? Actor = actors.FirstOrDefault((u) => u.Id == id);

        if (Actor != null)
        {
            actors.Remove(Actor);
        }
        return await Task.FromResult(Actor);
    }
}

