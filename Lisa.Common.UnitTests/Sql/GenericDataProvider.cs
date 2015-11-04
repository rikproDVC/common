using System.Collections.Generic;

namespace Lisa.Common.Sql
{
    internal class GenericDataProvider : IDataProvider
    {
        public GenericDataProvider(params object[] rows)
        {
            _rows = rows;
        }

        public IEnumerable<IRowProvider> Rows
        {
            get
            {
                foreach (var row in _rows)
                {
                    yield return new GenericRowProvider(row);
                }
            }
        }

        private object[] _rows;
    }
}
