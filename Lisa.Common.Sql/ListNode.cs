using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
    internal class ListNode : Node
    {
        protected override void Map(IDictionary<string, object> obj)
        {
            var list = new List<object>();
            obj.Add(Name, list);

            foreach (var child in Children)
            {
                if (!child.IsEmpty)
                {
                    var listItem = child.CreateObject();
                    list.Add(listItem);
                }
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return Children.All(child => child.IsEmpty);
            }
        }
    }
}