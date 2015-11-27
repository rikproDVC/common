using Lisa.Common.Sql;
using System;
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

            var row = new GenericDataProvider(movie);
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
            var movie = (IDictionary<string, object>) result.ElementAt(0);
            Assert.Equal("Title", movie.First().Key);
            Assert.Equal("Galaxy Quest", movie.First().Value);
            movie = (IDictionary<string, object>) result.ElementAt(1);
            Assert.Equal("Title", movie.First().Key);
            Assert.Equal("The Shawshank Redemption", movie.First().Value);
            movie = (IDictionary<string, object>) result.ElementAt(2);
            Assert.Equal("Title", movie.First().Key);
            Assert.Equal("Chocolat", movie.First().Value);
        }

        [Fact]
        public void ItCanMapASubObject()
        {
            var movie = new
            {
                Title = "Chocolat",
                Release_Year = 2000,
            };

            var row = new GenericDataProvider(movie);
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

            var row = new GenericDataProvider(movie);
            dynamic result = new ObjectMapper().Single(row);

            Assert.Equal(1999, result.Release.Year);
            Assert.Equal("USA", result.Release.Country);
        }

        [Fact]
        public void ItCanMapMultipleSubObjects()
        {
            var movie = new
            {
                Title = "Galaxy Quest",
                Release_Year = 1999,
                Release_Country = "USA",
                Rating_Body = "MPAA",
                Rating_Rank = "PG"
            };

            var row = new GenericDataProvider(movie);
            dynamic result = new ObjectMapper().Single(row);

            Assert.Equal(1999, result.Release.Year);
            Assert.Equal("USA", result.Release.Country);
            Assert.Equal("MPAA", result.Rating.Body);
            Assert.Equal("PG", result.Rating.Rank);
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

            var row = new GenericDataProvider(movie);
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

            var row = new GenericDataProvider(movie);
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
                    { "@Id", 1 },
                    { "Title", "Chocolat" },
                    { "#Writers_Name", "Joanne Harris" }
                },
                new Dictionary<string, object>
                {
                    { "@Id", 1 },
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

        [Fact]
        public void ItCanMapAnArray()
        {
            var movies = new[]
            {
                new Dictionary<string, object>
                {
                    { "@Id", 1 },
                    { "Title", "The Shawshank Redemption" },
                    { "#Writers", "Frank Darabont" }
                },
                new Dictionary<string, object>
                {
                    { "@Id", 1 },
                    { "Title", "The Shawshank Redemption" },
                    { "#Writers", "Stephen King" }
                }
            };

            var table = new GenericDataProvider(movies);
            var result = new ObjectMapper().Many(table);

            Assert.Equal(1, result.Count());
            dynamic movie = result.ElementAt(0);
            Assert.Equal(2, movie.Writers.Count);
            Assert.Equal("Frank Darabont", movie.Writers[0]);
            Assert.Equal("Stephen King", movie.Writers[1]);
        }

        [Fact]
        public void ItCanMapAnArrayInASubObject()
        {
            var movies = new[]
            {
                new Dictionary<string, object>
                {
                    { "@ID", 1 },
                    { "Title", "The Shawshank Redemption" },
                    { "Crew_@ID", 1 },
                    { "Crew_Director", "Frank Darabont" },
                    { "Crew_#Writers", "Frank Darabont" },
                },
                new Dictionary<string, object>
                {
                    { "@ID", 1 },
                    { "Title", "The Shawshank Redemption" },
                    { "Crew_@ID", 1 },
                    { "Crew_Director", "Frank Darabont" },
                    { "Crew_#Writers", "Stephen King" },
                }
            };

            var table = new GenericDataProvider(movies);
            var result = new ObjectMapper().Many(table);

            Assert.Equal(1, result.Count());
            dynamic movie = result.ElementAt(0);
            Assert.Equal("Frank Darabont", movie.Crew.Director);
            Assert.Equal(2, movie.Crew.Writers.Count);
            Assert.Equal("Frank Darabont", movie.Crew.Writers[0]);
            Assert.Equal("Stephen King", movie.Crew.Writers[1]);
        }

        [Fact]
        public void ItCanMapAListInASubObject()
        {
            var movies = new[]
            {
                new Dictionary<string, object>
                {
                    { "@ID", 1 },
                    { "Title", "The Shawshank Redemption" },
                    { "Crew_@ID", 1 },
                    { "Crew_Director", "Frank Darabont" },
                    { "Crew_#Writers_FirstName", "Frank" },
                    { "Crew_#Writers_LastName", "Darabont" }
                },
                new Dictionary<string, object>
                {
                    { "@ID", 1 },
                    { "Title", "The Shawshank Redemption" },
                    { "Crew_@ID", 1 },
                    { "Crew_Director", "Frank Darabont" },
                    { "Crew_#Writers_FirstName", "Stephen" },
                    { "Crew_#Writers_LastName", "King" }
                }
            };

            var table = new GenericDataProvider(movies);
            var result = new ObjectMapper().Many(table);

            Assert.Equal(1, result.Count());
            dynamic movie = result.ElementAt(0);
            Assert.Equal("Frank Darabont", movie.Crew.Director);
            Assert.Equal(2, movie.Crew.Writers.Count);
            Assert.Equal("Frank", movie.Crew.Writers[0].FirstName);
            Assert.Equal("Darabont", movie.Crew.Writers[0].LastName);
            Assert.Equal("Stephen", movie.Crew.Writers[1].FirstName);
            Assert.Equal("King", movie.Crew.Writers[1].LastName);
        }

        [Fact]
        public void ItCanMapAListInAList()
        {
            var directors = new[]
            {
                new Dictionary<string, object>
                {
                    { "@ID", 1 },
                    { "Name", "Frank Darabont" },
                    { "#Movies_@ID", "1" },
                    { "#Movies_Title", "The Shawshank Redemption" },
                    { "#Movies_#Writers_FirstName", "Frank" },
                    { "#Movies_#Writers_LastName", "Darabont" },
                },
                new Dictionary<string, object>
                {
                    { "@ID", 1 },
                    { "Name", "Frank Darabont" },
                    { "#Movies_@ID", "1" },
                    { "#Movies_Title", "The Shawshank Redemption" },
                    { "#Movies_#Writers_FirstName", "Stephen" },
                    { "#Movies_#Writers_LastName", "King" },
                },
                new Dictionary<string, object>
                {
                    { "@ID", 2 },
                    { "Name", "Lasse Hallström" },
                    { "#Movies_@ID", "1" },
                    { "#Movies_Title", "Chocolat" },
                    { "#Movies_#Writers_FirstName", "Robert" },
                    { "#Movies_#Writers_LastName", "Jacobs" },
                },
                new Dictionary<string, object>
                {
                    { "@ID", 2 },
                    { "Name", "Lasse Hallström" },
                    { "#Movies_@ID", "1" },
                    { "#Movies_Title", "Chocolat" },
                    { "#Movies_#Writers_FirstName", "Joanne" },
                    { "#Movies_#Writers_LastName", "Harris" },
                },
                new Dictionary<string, object>
                {
                    { "@ID", 2 },
                    { "Name", "Lasse Hallström" },
                    { "#Movies_@ID", "2" },
                    { "#Movies_Title", "An Unfinished Life" },
                    { "#Movies_#Writers_FirstName", "Mark" },
                    { "#Movies_#Writers_LastName", "Spragg" },
                },
                new Dictionary<string, object>
                {
                    { "@ID", 2 },
                    { "Name", "Lasse Hallström" },
                    { "#Movies_@ID", "2" },
                    { "#Movies_Title", "An Unfinished Life" },
                    { "#Movies_#Writers_FirstName", "Virginia" },
                    { "#Movies_#Writers_LastName", "Spragg" },
                },
            };

            var table = new GenericDataProvider(directors);
            var result = new ObjectMapper().Many(table);

            Assert.Equal(2, result.Count());
            dynamic director = result.ElementAt(0);
            Assert.Equal("Frank Darabont", director.Name);
            Assert.Equal(1, director.Movies.Count);
            Assert.Equal("The Shawshank Redemption", director.Movies[0].Title);
            Assert.Equal(2, director.Movies[0].Writers.Count);
            Assert.Equal("Stephen", director.Movies[0].Writers[1].FirstName);
        }

        [Fact]
        public void ItDoesNotMapEmptySubObjects()
        {
            object nil = null;
            var movie = new
            {
                Title = "Galaxy Quest",
                Release_Year = nil,
                Release_Country = nil
            };

            var row = new GenericDataProvider(movie);
            IDictionary<string, object> result = (IDictionary<string, object>) new ObjectMapper().Single(row);

            Assert.False(result.ContainsKey("Release"));
        }

        [Fact]
        public void ItMapsEmptyLists()
        {
            var movie = new Dictionary<string, object>
            {
                { "Title", "Galaxy Quest" },
                { "#Writers_FirstName", null },
                { "#Writers_LastName", null }
            };

            var row = new GenericDataProvider(movie);
            dynamic result = new ObjectMapper().Single(row);

            Assert.NotNull(result.Writers);
            Assert.Equal(0, result.Writers.Count);
        }

        [Fact]
        public void ItDoesNotMapSubObjectsWithOnlyEmptyLists()
        {
            var movie = new Dictionary<string, object>
            {
                { "Title", "Chocolat" },
                { "Crew_Count", null },
                { "Crew_#Writers_FirstName", null },
                { "Crew_#Writers_LastName", null }
            };

            var row = new GenericDataProvider(movie);
            IDictionary<string, object> result = (IDictionary<string, object>) new ObjectMapper().Single(row);

            Assert.False(result.ContainsKey("Crew"));
        }

        [Fact]
        public void ItMapsEmptyListsInSubObjectsWithOtherProperties()
        {
            var movie = new Dictionary<string, object>
            {
                { "Title", "Chocolat" },
                { "Crew_Count", 0 },
                { "Crew_#Writers_FirstName", null },
                { "Crew_#Writers_LastName", null }
            };

            var row = new GenericDataProvider(movie);
            IDictionary<string, object> result = (IDictionary<string, object>) new ObjectMapper().Single(row);

            Assert.True(result.ContainsKey("Crew"));
            Assert.Equal(0, ((dynamic) result["Crew"]).Count);
            Assert.NotNull(((dynamic) result["Crew"]).Writers);
            Assert.Equal(0, ((dynamic) result["Crew"]).Writers.Count);
        }

        [Fact]
        public void ItTreatsDbNullAsAnEmptyValue()
        {
            var movies = new[]
            {
                new Dictionary<string, object>
                {
                    { "@ID", 1 },
                    { "Title", "Chocolat" },
                    { "#Directors_@ID", DBNull.Value },
                    { "#Directors_FirstName", DBNull.Value },
                    { "#Directors_LastName", DBNull.Value }
                },
                new Dictionary<string, object>
                {
                    { "@ID", 2 },
                    { "Title", "The Shawshank Redemption" },
                    { "#Directors_@ID", 12 },
                    { "#Directors_FirstName", "Frank" },
                    { "#Directors_LastName", "Darabont" }
                }
            };

            var table = new GenericDataProvider(movies);
            dynamic result = new ObjectMapper().Many(table);

            var movie = result[0];
            Assert.Equal(0, movie.Directors.Count);
        }
    }
}