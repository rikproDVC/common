using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Lisa.Common.Sql
{
    internal abstract class QueryParameterInfo
    {
        public string Name { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = ConvertToString(value);
            }
        }

        protected abstract string ConvertToString(object value);

        private string _value;
    }

    internal class ValueParameterInfo : QueryParameterInfo
    {
        protected override string ConvertToString(object value)
        {
            var list = value as IEnumerable<object>;
            if (list != null)
            {
                var values = list.Select(item => ConvertToString(item));
                return string.Join(", ", values);
            }
            else
            {
                var safe = value.ToString().Replace("'", "''");
                var quoted = string.Format("'{0}'", safe);
                return quoted;
            }
        }
    }

    internal class NameParameterInfo : QueryParameterInfo
    {
        protected override string ConvertToString(object value)
        {
            var stringValue = value.ToString();
            if (stringValue.Contains("[") || stringValue.Contains("]"))
            {
                var message = string.Format("Argument '{0}' contains illegal characters.", Name);
                throw new ArgumentException(message);
            }

            var bracketed = string.Format("[{0}]", stringValue);
            return bracketed;
        }
    }
}