using System;

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
                _value = Sanitize(value);
            }
        }

        protected abstract string Sanitize(object value);

        private string _value;
    }

    internal class ValueParameterInfo : QueryParameterInfo
    {
        protected override string Sanitize(object value)
        {
            var safe = value.ToString().Replace("'", "''");
            var quoted = string.Format("'{0}'", safe);
            return quoted;
        }
    }

    internal class NameParameterInfo : QueryParameterInfo
    {
        protected override string Sanitize(object value)
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