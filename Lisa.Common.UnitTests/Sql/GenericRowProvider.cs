using System.Collections.Generic;
using System.Reflection;

namespace Lisa.Common.Sql
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
                foreach (var property in _data.GetType().GetProperties())
                {
                    yield return new KeyValuePair<string, object>(property.Name, property.GetValue(_data));
                }
            }
        }

        private object _data;
    }
}