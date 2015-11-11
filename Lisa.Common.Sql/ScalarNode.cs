using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
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
}