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
            var query = "select * from movies";
            return _gateway.SelectMany(query);
        }

        public object FetchMovie(int id)
        {
            var query = "select * from movies where id=@Id";
            var parameters = new { Id = id };
            return _gateway.SelectSingle(query, parameters);
        }

        public void Dispose()
        {
            _gateway?.Dispose();
        }

        private Gateway _gateway = new Gateway(@"Data Source=(localdb)\v11.0;Initial Catalog=common;Integrated Security=True");
    }
}