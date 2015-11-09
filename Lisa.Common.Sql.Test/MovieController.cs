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

        private Database _db = new Database();
    }
}