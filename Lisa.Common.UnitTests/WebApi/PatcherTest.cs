using Lisa.Common.WebApi;
using System.Linq;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class PatcherTest
    {
        [Fact]
        public void ItReportsAnInvalidAction()
        {
            var movie = new Movie
            {
                Title = "Galaxy Quest",
                Year = 1999
            };

            var patch = new Patch("surrender", "year", "1999");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(1, errors.Count());
        }

        [Fact]
        public void ItDoesNotPatchWhenOnePatchFails()
        {
            var movie = new Movie
            {
                Title = "Galaxy Quest",
                Year = 1999
            };

            var patches = new[]
            {
                new Patch("replace", "title", "'Galaxy Trek'"),
                new Patch("replace", "year", "'the future'")
            };

            var errors = Patcher.Apply(patches, movie);

            Assert.Equal(1, errors.Count());
            Assert.Equal("Galaxy Quest", movie.Title);
            Assert.Equal(1999, movie.Year);
        }
    }
}