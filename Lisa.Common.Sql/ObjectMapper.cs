using System.Collections.Generic;
using System.Dynamic;

namespace Lisa.Common.Sql
{
    public static class ObjectMapper
    {
        public static ExpandoObject Single(IRowProvider row)
        {
            var result = (IDictionary<string, object>) new ExpandoObject();

            foreach (var field in row.Fields)
            {
                if (field.Key.Contains("_"))
                {
                    var parts = field.Key.Split('_');

                    if (!result.ContainsKey(parts[0]))
                    {
                        var subObject = Single(new SubObjectRowProvider(parts[0], row));

                        if (parts[0].StartsWith("#"))
                        {
                            var list = new List<ExpandoObject>();
                            list.Add(subObject);
                            result.Add(new KeyValuePair<string, object>(parts[0].Substring(1), list));
                        }
                        else
                        {
                            result.Add(new KeyValuePair<string, object>(parts[0], subObject));
                        }
                    }
                }
                else
                {
                    result.Add(field);
                }
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