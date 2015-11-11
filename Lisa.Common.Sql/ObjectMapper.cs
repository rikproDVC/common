using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
    public class ObjectMapper
    {
        public ExpandoObject Single(IDataProvider data)
        {
            var tree = new MappingTree();
            if (data.Next())
            {
                tree.Add(data.Fields);
                return tree.Root.Children.First().CreateObject();
            }

            return null;
        }

        public IEnumerable<ExpandoObject> Many(IDataProvider data)
        {
            var tree = new MappingTree();
            while (data.Next())
            {
                tree.Add(data.Fields);
            }

            // Collect the results instead of yielding them, because that makes the life time
            // of the data provider predictable. Specifically, if we just yield the results, they
            // will be retrieved after the connection to the database has already closed.
            var results = new List<ExpandoObject>();
            foreach (var node in tree.Root.Children)
            {
                results.Add(node.CreateObject());
            }
            return results;
        }
    }
}