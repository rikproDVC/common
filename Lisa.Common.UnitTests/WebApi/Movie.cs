using System.Collections.Generic;

namespace Lisa.Common.UnitTests
{
    internal class Movie
    {
        public Movie()
        {
            Writers = new List<string>();
        }

        public string Title { get; set; }
        public int Year { get; set; }
        public IList<string> Writers { get; set; }
    }
}
