using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Lisa.Common.Sql.Test
{
    public class Database
    {
        public IEnumerable<object> FetchMovies()
        {
            var query = "select * from movies";

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