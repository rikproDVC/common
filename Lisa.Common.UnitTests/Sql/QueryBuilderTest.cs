using Lisa.Common.Sql;
using System;
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

        [Fact]
        public void ItSanitizesQuotesWithinValueParameters()
        {
            string query = "SELECT * FROM Planets WHERE Name='@Name'";
            object parameters = new
            {
                Name = "Q'onos"
            };

            string result = QueryBuilder.Build(query, parameters);
            Assert.Equal("SELECT * FROM Planets WHERE Name='Q''onos'", result);
        }

        [Fact]
        public void ItReplacesNameParameters()
        {
            string query = "SELECT * FROM Planets WHERE $Column='Vulcan'";
            object parameters = new
            {
                Column = "Inhabitant"
            };

            string result = QueryBuilder.Build(query, parameters);
            Assert.Equal("SELECT * FROM Planets WHERE [Inhabitant]='Vulcan'", result);
        }

        [Fact]
        public void ItRejectsNameParametersWithSquareBrackets()
        {
            string query = "SELECT * FROM Planets WHERE $Column='Vulcan'";
            object parameters = new
            {
                Column = "[Inhabitant]"
            };

            Assert.Throws<ArgumentException>(() => QueryBuilder.Build(query, parameters));
        }

        [Fact]
        public void ItRejectsQueryIfParameterIsMissing()
        {
            string query = "SELECT * FROM Planets WHERE MoonCount=@MoonCount";
            object parameters = new
            {
                Name = "Vulcan"
            };

            Assert.Throws<ArgumentException>(() => QueryBuilder.Build(query, parameters));
        }
    }
}