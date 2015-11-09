using Microsoft.AspNet.Mvc;

namespace Lisa.Common.Sql.Test
{
    [Route("/")]
    public class MovieController
    {
        public ActionResult Get()
        {
            var movies = _db.FetchMovies();
            return new HttpOkObjectResult(movies);
        }

        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            _db.FetchMovies();
            var movie = _db.FetchMovie(id);
            return new HttpOkObjectResult(movie);
        }

        private Database _db = new Database();
    }
}