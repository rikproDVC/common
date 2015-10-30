using Lisa.Common.WebApi;
using System.Linq;
using Xunit;

namespace Lisa.Common.UnitTests
{
    public class ReplacePatchTest
    {
        [Fact]
        public void ItReplacesAPropertyOfTypeString()
        {
            var movie = new Movie
            {
                Title = "Chocolate"
            };

            var patch = new Patch("replace", "title", "'Chocolat'");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(0, errors.Count());
            Assert.Equal("Chocolat", movie.Title);
        }

        [Fact]
        public void ItReplacesAPropertyOfTypeInt()
        {
            var movie = new Movie
            {
                Year = 1999
            };

            var patch = new Patch("replace", "year", "2000");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(0, errors.Count());
            Assert.Equal(2000, movie.Year);
        }

        [Fact]
        public void ItNullsANullableType()
        {
            var movie = new Movie
            {
                Title = "Chocolat"
            };

            var patch = new Patch("replace", "title", null);
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(0, errors.Count());
            Assert.Equal(null, movie.Title);
        }

        [Fact]
        public void ItReportsNullingANonNullableType()
        {
            var movie = new Movie
            {
                Year = 2000
            };

            var patch = new Patch("replace", "year", null);
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(1, errors.Count());
            Assert.Equal(2000, movie.Year);
        }

        [Fact]
        public void ItReportsAssigningAnArrayToASimpleProperty()
        {
            var movie = new Movie
            {
                Year = 1999
            };

            var patch = new Patch("replace", "year", "[ 1999, 2000 ]");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(1, errors.Count());
            Assert.Equal(1999, movie.Year);
        }

        [Fact]
        public void ItReportsAssigningAnObjectToASimpleProperty()
        {
            var movie = new Movie
            {
                Title = "Chocolat"
            };

            var patch = new Patch("replace", "title", "{ title: 'Chocolat' }");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(1, errors.Count());
            Assert.Equal("Chocolat", movie.Title);
        }
    }
}