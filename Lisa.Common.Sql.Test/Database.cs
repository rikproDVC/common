using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Lisa.Common.Sql.Test
{
    public class Database
    {
        public IEnumerable<object> FetchMovies()
        {
            var query = @"select Movies.Id AS [@], Title, Year, Directors.Id AS #Directors_@ID, Directors.FirstName AS #Directors_FirstName, Directors.LastName AS #Directors_LastName
                from movies
                left join directors on movies.id = directors.movie
                order by directors.firstname asc";

            using (var connection = new SqlConnection(@"Data Source=(localdb)\v11.0;Initial Catalog=common;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                var reader = command.ExecuteReader();
                var dataProvider = new SqlDataProvider(reader);
                var mapper = new ObjectMapper();
                return mapper.Many(dataProvider);
            }
        }
    }
}