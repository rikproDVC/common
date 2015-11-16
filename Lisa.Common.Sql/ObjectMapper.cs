using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
    public class ObjectMapper
    {
        public object Single(IDataProvider data)
        {
            var tree = new TreeBuilder();
            if (data.Next())
            {
                tree.Add(data.Fields);
            }

            return tree.Root.Children.FirstOrDefault()?.CreateObject();
        }

        public IEnumerable<object> Many(IDataProvider data)
        {
            var tree = new TreeBuilder();
            while (data.Next())
            {
                tree.Add(data.Fields);
            }

            // Collect the results instead of yielding them, because that makes the life time
            // of the data provider predictable. Specifically, if we just yield the results, they
            // will be retrieved after the connection to the database has already closed.
            var results = new List<object>();
            foreach (var node in tree.Root.Children)
            {
                results.Add(node.CreateObject());
            }
            return results;
        }
    }
}