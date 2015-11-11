using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Lisa.Common.Sql
{
    public class SqlDataProvider : IDataProvider
    {
        public SqlDataProvider(SqlDataReader reader)
        {
            _reader = reader;
        }

        public IEnumerable<KeyValuePair<string, object>> Fields
        {
            get
            {
                for (int i = 0; i < _reader.FieldCount; i++)
                {
                    string name = _reader.GetName(i);
                    object value = _reader[i];
                    yield return new KeyValuePair<string, object>(name, value);
                }
            }
        }

        public bool Next()
        {
            return _reader.Read();
        }

        private SqlDataReader _reader;
    }
}