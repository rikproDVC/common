using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
    internal class ArrayNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            var array = new List<object>();
            obj.Add(Name, array);

            foreach (var child in Children)
            {
                array.Add(child.Value);
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return Children.Count == 0;
            }
        }
    }
}