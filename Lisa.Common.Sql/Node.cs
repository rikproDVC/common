using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Lisa.Common.Sql
{
    internal class Node
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
            foreach (var child in Children)
            {
                if (identity.Equals(child.Identity))
                {
                    return child;
                }
            }

            return null;
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

        public virtual bool IsEmpty { get; }
        protected virtual void Map(IDictionary<string, object> obj)
        {
        }
    }

    internal class ScalarNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            obj.Add(Name, Value);
        }

        public override bool IsEmpty
        {
            get
            {
                return Value == null;
            }
        }
    }

    internal class SubObjectNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            if (!IsEmpty)
            {
                var value = CreateObject();
                obj.Add(Name, value);
            }
        }

        public override bool IsEmpty
        {
            get
            {
                foreach (var child in Children)
                {
                    if (!child.IsEmpty)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }

    internal class ListNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            var list = new List<ExpandoObject>();
            obj.Add(Name, list);

            foreach (var child in Children)
            {
                var listItem = child.CreateObject();
                list.Add(listItem);
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