using Lisa.Common.Sql;
using Xunit;

namespace Lisa.Common.UnitTests.Sql
{
    public class QueryBuilderTest
    {
        [Fact]
        public void ItReturnsQueriesWithoutParametersUnchanged()
        {
            string query = "SELECT * FROM Planets";

            string result = QueryBuilder.Build(query);
            Assert.Equal("SELECT * FROM Planets", result);
        }

        [Fact]
        public void ItReplacesValueParameters()
        {
            string query = "SELECT * FROM Planets WHERE MoonCount=@MoonCount AND Star=@Star";
            object parameters = new
            {
                MoonCount = 2,
                Star = "Sol"
            };

            string result = QueryBuilder.Build(query, parameters);
            Assert.Equal("SELECT * FROM Planets WHERE MoonCount='2' AND Star='Sol'", result);
        }

        [Fact]
        public void ItDoesNotAddUnnecessaryQuotes()
        {
            string query = "SELECT * FROM Planets WHERE MoonCount='@MoonCount' AND Star='@Star'";
            object parameters = new
            {
                MoonCount = 2,
                Star = "Sol"
            };

            string result = QueryBuilder.Build(query, parameters);
            Assert.Equal("SELECT * FROM Planets WHERE MoonCount='2' AND Star='Sol'", result);
        }
    }
}