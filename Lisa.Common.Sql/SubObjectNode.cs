using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
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
                return Children.All(child => child.IsEmpty);
            }
        }
    }
}