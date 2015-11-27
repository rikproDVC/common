using System;
using System.Reflection;

namespace Lisa.Common.Sql
{
    public static class QueryBuilder
    {
        public static string Build(string query, object parameters = null)
        {
            if (parameters != null)
            {
                foreach (var property in parameters.GetType().GetProperties())
                {
                    var name = string.Format("'@{0}'", property.Name);
                    var value = string.Format("'{0}'", property.GetValue(parameters).ToString().Replace("'", "''"));
                    query = query.Replace(name, value);

                    name = string.Format("@{0}", property.Name);
                    query = query.Replace(name, value);

                    name = string.Format("${0}", property.Name);

                    if (query.Contains(name))
                    {
                        string rawValue = property.GetValue(parameters).ToString();
                        if (rawValue.Contains("[") || rawValue.Contains("]"))
                        {
                            string error = string.Format("Argument '{0}' contains illegal characters.", property.Name);
                            throw new ArgumentException(error);
                        }

                        value = string.Format("[{0}]", rawValue);
                        query = query.Replace(name, value);
                    }
                }
            }

            return query;
        }
    }
}