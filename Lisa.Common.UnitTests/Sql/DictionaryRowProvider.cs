using Lisa.Common.Sql;
using System.Collections.Generic;
using System.Reflection;

namespace Lisa.Common.UnitTests
{
    internal class DictionaryRowProvider : IRowProvider
    {
        public DictionaryRowProvider(IDictionary<string, object> data)
        {
            _data = data;
        }

        public IEnumerable<KeyValuePair<string, object>> Fields
        {
            get
            {
                return _data;
            }
        }

        private IDictionary<string, object> _data;
    }
}