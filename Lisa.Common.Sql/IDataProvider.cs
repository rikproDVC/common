using System.Collections.Generic;

namespace Lisa.Common.Sql
{
    public interface IDataProvider
    {
        IEnumerable<KeyValuePair<string, object>> Fields { get; }
        bool Next();
    }
}