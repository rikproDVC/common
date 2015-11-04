using System.Linq;
using Xunit;

namespace Lisa.Common.Sql
{
    public class ObjectMapperTest
    {
        [Fact]
        public void ItCreatesAnObjectOutOfARow()
        {
            var movie = new
            {
                Title = "Galaxy Quest",
                Year = 1999
            };

            var row = new GenericRowProvider(movie);
            dynamic result = ObjectMapper.Single(row);

            Assert.Equal("Galaxy Quest", result.Title);
            Assert.Equal(1999, result.Year);
        }

        [Fact]
        public void ItCanMapMultipleRows()
        {
            var movies = new[]
            {
                new { Title = "Galaxy Quest" },
                new { Title = "The Shawshank Redemption" },
                new { Title = "Chocolat" }
            };

            var table = new GenericDataProvider(movies);
            var result = ObjectMapper.Many(table);

            Assert.Equal(3, result.Count());
            Assert.Equal("Title", result.ElementAt(0).First().Key);
            Assert.Equal("Galaxy Quest", result.ElementAt(0).First().Value);
            Assert.Equal("Title", result.ElementAt(1).First().Key);
            Assert.Equal("The Shawshank Redemption", result.ElementAt(1).First().Value);
            Assert.Equal("Title", result.ElementAt(2).First().Key);
            Assert.Equal("Chocolat", result.ElementAt(2).First().Value);
        }
    }
}