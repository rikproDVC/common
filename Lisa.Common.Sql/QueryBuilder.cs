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
                    var value = string.Format("'{0}'", property.GetValue(parameters));
                    query = query.Replace(name, value);

                    name = string.Format("@{0}", property.Name);
                    value = string.Format("'{0}'", property.GetValue(parameters));
                    query = query.Replace(name, value);
                }
            }

            return query;
        }
    }
}