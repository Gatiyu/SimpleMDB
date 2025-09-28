using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleMDB;

public class MockMovieRepository : IMovieRepository
{
    private List<Movie> movies;
    private int idCount;

    public MockMovieRepository()
    {
        movies = [];
        idCount = 0;

        var titles = new[]
        {
            "The Galactic Odyssey", "Midnight Serenade", "Echoes of Yesterday", "The Crimson Cipher", "Silent Shadows",
            "Whispers in the Wind", "A Journey Beyond", "The Last Stand", "Chronicles of the Abyss", "Starlight Reverie",
            "Forgotten Realms", "The Quantum Paradox", "Celestial Dawn", "The Alchemist's Secret", "Sands of Time",
            "The Emerald Enigma", "Rise of the Phoenix", "The Dragon's Legacy", "The Shadow's Grasp", "The Final Frontier",
            "The Lost City", "The Golden Compass", "The Silver Lining", "The Bronze Heart", "The Iron Will",
            "The Steel Resolve", "The Diamond Age", "The Crystal Shard", "The Sapphire Star", "The Ruby Throne",
            "The Jade Empire", "The Obsidian Mirror", "The Onyx Blade", "The Pearl of Wisdom", "The Coral Key",
            "The Amber Spyglass", "The Ivory Tower", "The Ebony Gate", "The Scarlet Letter", "The Crimson Peak",
            "The Azure Dream", "The Cerulean Sea", "The Indigo Prophecy", "The Violet Hour", "The Magenta Maze",
            "The Golden Fleece", "The Silver Chalice", "The Bronze Serpent", "The Iron Fist", "The Steel Cage",
            "The Diamond Eye", "The Crystal Skull", "The Sapphire Moon", "The Ruby Heart", "The Jade Garden",
            "The Obsidian Fortress", "The Onyx Crown", "The Pearl River", "The Coral Castle", "The Amber Road",
            "The Ivory Coast", "The Ebony Forest", "The Scarlet Tide", "The Crimson River", "The Azure Sky",
            "The Cerulean Blade", "The Indigo Mountain", "The Violet Storm", "The Magenta Field", "The Golden Age",
            "The Silver River", "The Bronze Shield", "The Iron Mountain", "The Steel Empire", "The Diamond City",
            "The Crystal Cave", "The Sapphire Sea", "The Ruby Desert", "The Jade Mountain", "The Obsidian City",
            "The Onyx River", "The Pearl Harbor", "The Coral Reef", "The Amber Forest", "The Ivory Palace",
            "The Ebony Ship", "The Scarlet Dawn", "The Crimson Night", "The Azure Blade", "The Cerulean River",
            "The Indigo Sea", "The Violet Mountain", "The Magenta Sky", "The Golden Empire", "The Silver Fortress"
        };

        var years = new[]
        {
        1994, 1972, 2008, 1974, 1957, 1993, 2003, 1994, 1966, 1999,
        1994, 2010, 2001, 1980, 2002, 1999, 1990, 1975, 1995, 1954,
        2002, 1991, 1946, 1997, 1995, 1994, 1998, 2001, 1998, 2014,
        2019, 1999, 1942, 1994, 2006, 2006, 2014, 2000, 2000, 1979,
        1940, 1988, 2006, 1988, 1957, 2012, 2008, 1980, 2018, 2003,
        1986, 1968, 1957, 2018, 2012, 2019, 2019, 1999, 1995, 2010,
        2017, 2009, 2017, 2013, 2015, 1997, 1998, 2010, 2017, 2016,
        2019, 2013, 2019, 2004, 2002, 2011, 2006, 2011, 1997, 1954,
        2000, 2005, 2010, 2015, 2020, 1990, 1985, 1970, 1965, 2007,
        2012, 2018, 1996, 2004, 2009, 2011, 2013, 2014, 2016, 2021
        };

        var descriptions = new[]
        {
        "A thrilling space opera about a journey to the edge of the universe.",
        "A romantic drama set in the heart of Paris.",
        "A historical epic about love and loss during a great war.",
        "A mystery that unravels a secret society's ancient code.",
        "A noir film about a detective haunted by his past.",
        "A fantasy adventure about a young hero's quest.",
        "A sci-fi film that explores the boundaries of human consciousness.",
        "An action-packed story of survival against all odds.",
        "A horror story that delves into the depths of the unknown.",
        "A whimsical tale of dreams and reality.",
        "A journey through a land of myth and magic.",
        "A mind-bending thriller about time travel.",
        "A story of hope and new beginnings in a post-apocalyptic world.",
        "A quest for the legendary philosopher's stone.",
        "A sweeping epic that spans generations.",
        "A puzzle-box mystery with a shocking twist.",
        "A story of rebirth and redemption.",
        "A fantasy epic about a dynasty of dragon riders.",
        "A dark fantasy about the struggle against an ancient evil.",
        "A sci-fi adventure to the outer reaches of space.",
        "An adventure to find a legendary city of gold.",
        "A fantasy story about a young girl and her magical device.",
        "A drama about finding hope in the darkest of times.",
        "A war film about courage and sacrifice.",
        "A story of determination and perseverance.",
        "A tale of a hero's unyielding spirit.",
        "A sci-fi epic set in a technologically advanced future.",
        "A fantasy adventure about a powerful magical artifact.",
        "A space opera about a legendary star.",
        "A story of power and betrayal in a royal court.",
        "A historical epic set in ancient China.",
        "A supernatural thriller about a cursed object.",
        "A fantasy story about a legendary weapon.",
        "A quest for a mythical pearl of great power.",
        "A mystery set in a tropical paradise.",
        "A fantasy adventure about a magical instrument.",
        "A story of isolation and madness.",
        "A horror story about a gateway to another dimension.",
        "A classic tale of sin and redemption.",
        "A gothic romance with a dark secret.",
        "A surreal journey through a dreamscape.",
        "A story of adventure on the high seas.",
        "A supernatural mystery about a series of strange events.",
        "A drama about the final moments of a great artist.",
        "A psychedelic journey through a bizarre landscape.",
        "A story of a mythical quest for a legendary artifact.",
        "A tale of a holy relic and the knights who protect it.",
        "A story of a cursed object and its terrible power.",
        "A martial arts film about a legendary fighting style.",
        "A story of a brutal and unforgiving competition.",
        "A thriller about a heist to steal a priceless gem.",
        "A horror story about a mysterious and powerful artifact.",
        "A space adventure to a distant and mysterious moon.",
        "A story of a queen's love for her people.",
        "A fantasy tale of a beautiful and magical place.",
        "A story of a dark and powerful fortress.",
        "A tale of a king's rise to power.",
        "A story of a great and prosperous city.",
        "A story of a beautiful and dangerous place.",
        "A tale of a long and perilous journey.",
        "A story of a beautiful and exotic land.",
        "A story of a dark and mysterious place.",
        "A story of a great and terrible battle.",
        "A story of a beautiful and tragic love.",
        "A story of a great and powerful weapon.",
        "A story of a beautiful and dangerous journey.",
        "A story of a mysterious and magical place.",
        "A story of a great and powerful storm.",
        "A story of a beautiful and strange world.",
        "A story of a great and powerful civilization.",
        "A story of a beautiful and dangerous river.",
        "A story of a great and powerful fortress.",
        "A story of a great and powerful empire.",
        "A story of a beautiful and prosperous city.",
        "A story of a mysterious and magical cave.",
        "A story of a beautiful and dangerous sea.",
        "A story of a desolate and dangerous desert.",
        "A story of a beautiful and magical mountain.",
        "A story of a dark and dangerous city.",
        "A story of a dark and dangerous river.",
        "A story of a safe and prosperous harbor.",
        "A story of a beautiful and dangerous reef.",
        "A story of a dark and mysterious forest.",
        "A story of a beautiful and grand palace.",
        "A story of a dark and mysterious ship.",
        "A story of a beautiful and hopeful morning.",
        "A story of a dark and dangerous night.",
        "A story of a powerful and legendary weapon.",
        "A story of a beautiful and dangerous river.",
        "A story of a mysterious and magical sea.",
        "A story of a beautiful and dangerous mountain.",
        "A story of a beautiful and strange sky.",
        "A story of a great and powerful empire.",
        "A story of a beautiful and strong fortress."
        };
    
        var random = new Random();
        int seedCount = Math.Min(titles.Length, Math.Min(years.Length, descriptions.Length));

        for (int i = 0; i < seedCount; i++)
        {
            float rating = (float)Math.Round(random.NextDouble() * 10, 1);
            movies.Add(new Movie(idCount++, titles[i], years[i], descriptions[i], rating));
        }
    }
    

    public async Task<PagedResult<Movie>> ReadAll(int page, int size)
    {
        int totalCount = movies.Count;
        int start = Math.Clamp((page - 1) * size, 0, totalCount);
        int length = Math.Clamp(size, 0, totalCount - start);

        List<Movie> values = movies.GetRange(start, length);

        var pagedResult = new PagedResult<Movie>(values, totalCount);

        return await Task.FromResult(pagedResult);
    }

    public async Task<Movie?> Create(Movie Movie)
    {
        Movie.Id = idCount++;
        movies.Add(Movie);
        Console.WriteLine(Movie);

        return await Task.FromResult(Movie);
    }

    public async Task<Movie?> Read(int id)
    {
        Movie? Movie = movies.FirstOrDefault((u) => u.Id == id);
        return await Task.FromResult(Movie);
    }

    public async Task<Movie?> Update(int id, Movie newMovie)
    {
        Movie? movie = movies.FirstOrDefault((u) => u.Id == id);

        if (movie != null)
        {
            movie.Title = newMovie.Title;
            movie.Year = newMovie.Year;
            movie.Description = newMovie.Description;
            movie.rating = newMovie.rating;
        }
        return await Task.FromResult(movie);
    }

    public async Task<Movie?> Delete(int id)
    {
        Movie? Movie = movies.FirstOrDefault((u) => u.Id == id);

        if (Movie != null)
        {
            movies.Remove(Movie);
        }
        return await Task.FromResult(Movie);
    }
}
