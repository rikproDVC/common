using Lisa.Common.Sql;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Lisa.Common.UnitTests
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
            dynamic result = new ObjectMapper().Single(row);

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
            var result = new ObjectMapper().Many(table);

            Assert.Equal(3, result.Count());
            Assert.Equal("Title", result.ElementAt(0).First().Key);
            Assert.Equal("Galaxy Quest", result.ElementAt(0).First().Value);
            Assert.Equal("Title", result.ElementAt(1).First().Key);
            Assert.Equal("The Shawshank Redemption", result.ElementAt(1).First().Value);
            Assert.Equal("Title", result.ElementAt(2).First().Key);
            Assert.Equal("Chocolat", result.ElementAt(2).First().Value);
        }

        [Fact]
        public void ItCanMapASubObject()
        {
            var movie = new
            {
                Title = "Chocolat",
                Release_Year = 2000,
            };

            var row = new GenericRowProvider(movie);
            dynamic result = new ObjectMapper().Single(row);

            Assert.Equal(2000, result.Release.Year);
        }

        [Fact]
        public void ItCanMapASubObjectWithMultipleProperties()
        {
            var movie = new
            {
                Title = "Galaxy Quest",
                Release_Year = 1999,
                Release_Country = "USA"
            };

            var row = new GenericRowProvider(movie);
            dynamic result = new ObjectMapper().Single(row);

            Assert.Equal(1999, result.Release.Year);
            Assert.Equal("USA", result.Release.Country);
        }

        [Fact]
        public void ItCanMapSubObjectsWithinSubObjects()
        {
            var movie = new
            {
                Title = "Galaxy Quest",
                Release_Country = "USA",
                Release_Time_Year = 1999,
                Release_Time_Day = "Thursday"
            };

            var row = new GenericRowProvider(movie);
            dynamic result = new ObjectMapper().Single(row);

            Assert.Equal("USA", result.Release.Country);
            Assert.Equal(1999, result.Release.Time.Year);
            Assert.Equal("Thursday", result.Release.Time.Day);
        }

        [Fact]
        public void ItCanMapAListItem()
        {
            var movie = new Dictionary<string, object>
            {
                { "Title", "Chocolat" },
                { "#Writers_Name", "Joanne Harris" }
            };

            var row = new DictionaryRowProvider(movie);
            dynamic result = new ObjectMapper().Single(row);

            Assert.Equal(1, result.Writers.Count);
            Assert.Equal("Joanne Harris", result.Writers[0].Name);
        }

        [Fact]
        public void ItCanMapAListWithMultipleItems()
        {
            var movies = new[]
            {
                new Dictionary<string, object>
                {
                    { "Title", "Chocolat" },
                    { "#Writers_Name", "Joanne Harris" }
                },
                new Dictionary<string, object>
                {
                    { "Title", "Chocolat" },
                    { "#Writers_Name", "Robert Nelson Jacobs" }
                }
            };

            var table = new GenericDataProvider(movies);
            var result = new ObjectMapper().Many(table);

            Assert.Equal(1, result.Count());
            dynamic movie = result.ElementAt(0);
            Assert.Equal(2, movie.Writers.Count);
            Assert.Equal("Joanne Harris", movie.Writers[0].Name);
            Assert.Equal("Robert Nelson Jacobs", movie.Writers[1].Name);
        }
    }
}