using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Lisa.Common.Sql.Test
{
    public sealed class Database : IDisposable
    {
        public IEnumerable<object> FetchMovies()
        {
            var query = @"select Movies.Id AS [@], Title, Year, Directors.Id AS #Directors_@ID, Directors.FirstName AS #Directors_FirstName, Directors.LastName AS #Directors_LastName
                from movies
                left join directors on movies.id = directors.movie
                order by directors.firstname asc";
            return _gateway.SelectMany(query);
        }

        public object FetchMovie(object id)
        {
            var query = "select * from movies where id=@Id";
            var parameters = new { Id = id };
            return _gateway.SelectSingle(query, parameters);
        }

        public object CreateMovie(Movie movie)
        {
            var query = "insert into movies(title, year) values(@Title, @Year)";
            return _gateway.Insert(query, movie);
        }

        public void Dispose()
        {
            _gateway?.Dispose();
        }

        private Gateway _gateway = new Gateway(@"Data Source=(localdb)\v11.0;Initial Catalog=common;Integrated Security=True");
    }
}