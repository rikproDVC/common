using Lisa.Common.Sql;
using System.Collections.Generic;
using System.Reflection;

namespace Lisa.Common.UnitTests
{
    internal class GenericRowProvider : IRowProvider
    {
        public GenericRowProvider(object data)
        {
            _data = data;
        }

        public IEnumerable<KeyValuePair<string, object>> Fields
        {
            get
            {
                if (_data is IDictionary<string, object>)
                {
                    foreach (var field in (ICollection<KeyValuePair<string, object>>) _data)
                    {
                        yield return field;
                    }
                }
                else
                {
                    foreach (var property in _data.GetType().GetProperties())
                    {
                        yield return new KeyValuePair<string, object>(property.Name, property.GetValue(_data));
                    }
                }
            }
        }

        private object _data;
    }
}