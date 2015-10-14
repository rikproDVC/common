using Microsoft.AspNet.Mvc;

namespace Lisa.Common.WebApi.Test
{
    [Route("movies")]
    public class MoviesController
    {
        public MoviesController(Database db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var articles = _db.FetchMovies();
            return new ObjectResult(articles);
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var article = _db.FetchMovie(id);
            return new ObjectResult(article);
        }

        [HttpPatch("{id}")]
        public IActionResult Patch(string id, [FromBody] Patch[] patches)
        {
            var article = _db.FetchMovie(id);
            Patcher.Apply(patches, article);

            return new ObjectResult(article);
        }

        private readonly Database _db;
    }
}