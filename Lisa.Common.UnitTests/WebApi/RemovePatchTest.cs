using Lisa.Common.WebApi;
using System.Linq;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class RemovePatchTest
    {
        [Fact]
        public void ItRemovesAStringFromAListByValue()
        {
            var movie = new Movie
            {
                Title = "Galaxy Quest",
                Writers = { "David Howard", "Gene Roddenberry", "Robert Gordon" }
            };

            var patch = new Patch("remove", "writers", "'Gene Roddenberry'");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(0, errors.Count());
            Assert.Equal(2, movie.Writers.Count);
        }

        [Fact]
        public void ItReportsRemovingFromAScalarProperty()
        {
            var movie = new Movie
            {
                Title = "Galaxy Quest"
            };

            var patch = new Patch("remove", "title", "'Galaxy Quest'");
            var errors = Patcher.Apply(new[] { patch }, movie);

            Assert.Equal(1, errors.Count());
            Assert.Equal("Galaxy Quest", movie.Title);
        }
    }
}