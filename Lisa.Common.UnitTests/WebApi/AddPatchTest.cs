using Lisa.Common.WebApi;
using System.Linq;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class AddPatchTest
    {
        [Fact]
        public void ItAddsAValueToAListOfStrings()
        {
            var movie = new Movie
            {
                Title = "Chocolat",
                Writers = { "Joanne Harris" }
            };

            var patch = new Patch("add", "writers", "'Robert Nelson Jacobs'");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(0, errors.Count());
            Assert.Equal(2, movie.Writers.Count);
            Assert.Equal("Robert Nelson Jacobs", movie.Writers[1]);
        }

        [Fact]
        public void ItReportsAddingAnArrayToAListOfStrings()
        {
            var movie = new Movie
            {
                Title = "Chocolat",
                Writers = { "Joanne Harris" }
            };

            var patch = new Patch("add", "writers", "[ 'Robert Nelson Jacobs' ]");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(1, errors.Count());
            Assert.Equal(1, movie.Writers.Count);
        }

        [Fact]
        public void ItReportsAddingToAScalarProperty()
        {
            var movie = new Movie
            {
                Title = "Chocolat"
            };

            var patch = new Patch("add", "title", "'Robert Nelson Jacobs'");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(1, errors.Count());
            Assert.Equal("Chocolat", movie.Title);
        }
    }
}