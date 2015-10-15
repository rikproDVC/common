using System.Collections.Generic;

namespace Lisa.Common.WebApi.Test
{
    public class Movie
    {
        public Movie()
        {
            Writers = new List<string>();
        }

        public string Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public IList<string> Writers { get; set; }
    }
}