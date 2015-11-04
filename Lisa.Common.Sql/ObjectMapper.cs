using System.Collections.Generic;
using System.Dynamic;

namespace Lisa.Common.Sql
{
    public static class ObjectMapper
    {
        public static ExpandoObject Single(IRowProvider row)
        {
            var result = (ICollection<KeyValuePair<string, object>>) new ExpandoObject();

            foreach (var field in row.Fields)
            {
                result.Add(field);
            }

            return (ExpandoObject) result;
        }

        public static IEnumerable<ExpandoObject> Many(IDataProvider table)
        {
            foreach (var row in table.Rows)
            {
                yield return Single(row);
            }
        }
    }
}