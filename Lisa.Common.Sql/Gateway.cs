using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace Lisa.Common.Sql
{
    public sealed class Gateway : IDisposable
    {
        public Gateway(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
        }

        public object SelectSingle(string query, object parameters = null)
        {
            return SelectMany(query, parameters).FirstOrDefault();
        }

        public IEnumerable<object> SelectMany(string query, object parameters = null)
        {
            _connection.Open();

            try
            {
                var command = _connection.CreateCommand();
                command.CommandText = query;
                command.CommandType = CommandType.Text;

                if (parameters != null)
                {
                    foreach (var property in parameters.GetType().GetProperties())
                    {
                        var parameter = new SqlParameter(property.Name, property.GetValue(parameters));
                        command.Parameters.Add(parameter);
                    }
                }

                var reader = command.ExecuteReader();
                var dataProvider = new SqlDataProvider(reader);
                var mapper = new ObjectMapper();
                return mapper.Many(dataProvider);

            }
            finally
            {
                _connection.Close();
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        private SqlConnection _connection;
    }
}