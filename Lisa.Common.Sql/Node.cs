using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
    internal abstract class Node
    {
        public Node()
        {
            Children = new List<Node>();
        }

        public object Identity { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public ICollection<Node> Children { get; set; }

        public Node Find(object identity)
        {
            return Children
                .Where(child => identity.Equals(child.Identity))
                .FirstOrDefault();
        }

        public ExpandoObject CreateObject()
        {
            IDictionary<string, object> obj = new ExpandoObject();
            foreach (var child in Children)
            {
                child.Map(obj);
            }

            return (ExpandoObject) obj;
        }

        public abstract bool IsEmpty { get; }
        protected abstract void Map(IDictionary<string, object> obj);
    }
}