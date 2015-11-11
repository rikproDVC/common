using Lisa.Common.Sql;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lisa.Common.UnitTests
{
    internal class GenericDataProvider : IDataProvider
    {
        public GenericDataProvider(params object[] rows)
        {
            _rows = rows;
        }

        public GenericDataProvider(object row)
        {
            _rows = new[] { row };
        }

        public bool Next()
        {
            currentRow++;
            return currentRow < _rows.Length;
        }

        public IEnumerable<KeyValuePair<string, object>> Fields
        {
            get
            {
                if (currentRow < 0 || currentRow >= _rows.Length)
                {
                    throw new InvalidOperationException();
                }

                var row = _rows[currentRow];
                if (row is IDictionary<string, object>)
                {
                    foreach (var field in (ICollection<KeyValuePair<string, object>>) row)
                    {
                        yield return field;
                    }
                }
                else
                {
                    foreach (var property in row.GetType().GetProperties())
                    {
                        yield return new KeyValuePair<string, object>(property.Name, property.GetValue(row));
                    }
                }
            }
        }

        private object[] _rows;
        private int currentRow = -1;
    }
}
