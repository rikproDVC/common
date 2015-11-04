using System.Collections.Generic;

namespace Lisa.Common.Sql
{
    public interface IDataProvider
    {
        IEnumerable<IRowProvider> Rows { get; }
    }
}