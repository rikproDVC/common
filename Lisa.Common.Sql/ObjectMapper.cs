using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
    public class ObjectMapper
    {
        public ExpandoObject Single(IRowProvider row)
        {
            var tree = new MappingTree();
            tree.Add(row);

            return tree.Root.Children.First().CreateObject();
        }

        public IEnumerable<ExpandoObject> Many(IDataProvider table)
        {
            var tree = new MappingTree();
            foreach (var row in table.Rows)
            {
                tree.Add(row);
            }

            foreach (var node in tree.Root.Children)
            {
                yield return node.CreateObject();
            }
        }
    }
}