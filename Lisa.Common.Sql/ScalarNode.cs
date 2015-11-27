using System;
using System.Collections.Generic;

namespace Lisa.Common.Sql
{
    internal class ScalarNode : Node
    {
        public override bool IsEmpty
        {
            get
            {
                return Value == null || Value == DBNull.Value;
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