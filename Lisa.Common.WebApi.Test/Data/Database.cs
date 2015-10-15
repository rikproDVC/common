using System.Collections.Generic;
using System.Linq;

namespace Lisa.Common.WebApi.Test
{
    public class Database
    {
        public Database()
        {
            _movies = new List<Movie>
            {
                new Movie
                {
                    Id = "shawshank-redemption",
                    Title = "The Shawshank Redemption",
                    Year = 1994,
                    Writers = { "Stephen King", "Frank Darabont" }
                },

                new Movie
                {
                    Id = "chocolat",
                    Title = "Chocolat",
                    Year = 2000,
                    Writers = { "Joanne Harris" }
                },

                new Movie
                {
                    Id = "galaxy-quest",
                    Title = "Galaxy Quest",
                    Year = 1999,
                    Writers = { "David Howard", "Robert Gordon" }
                }
            };
        }

        public IEnumerable<Movie> FetchMovies()
        {
            return _movies;
        }

        public Movie FetchMovie(string id)
        {
            return _movies.Where(movie => movie.Id == id).SingleOrDefault();
        }

        private IList<Movie> _movies;
    }
}