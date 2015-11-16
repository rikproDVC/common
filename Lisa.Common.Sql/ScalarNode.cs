using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Lisa.Common.Sql
{
    internal class ScalarNode : Node
    {
        public override bool IsEmpty
        {
            get
            {
                return Value == null;
            }
        }

        public override object CreateObject()
        {
            return Value;
        }

        protected override void Map(IDictionary<string, object> obj)
        {
            obj.Add(Name, Value);
        }
    }
}