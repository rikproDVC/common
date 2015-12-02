using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Lisa.Common.Sql
{
    public static class QueryBuilder
    {
        public static string Build(string query, object values = null)
        {
            var parameters = ExtractParameters(query, values);
            return ReplaceParameters(query, parameters, values);
        }

        private static IEnumerable<QueryParameterInfo> ExtractParameters(string query, object values)
        {
            var valueParameters = ExtractParameters<ValueParameterInfo>(@"(?<quote>')@(?<name>\w+)(?<-quote>')|@(?<name>\w+)", query, values);
            var nameParameters = ExtractParameters<NameParameterInfo>(@"(?<bracket>\[)\$(?<name>\w+)(?<-bracket>\])|\$(?<name>\w+)", query, values);
            return valueParameters.Union(nameParameters);
        }

        private static IEnumerable<QueryParameterInfo> ExtractParameters<T>(string pattern, string query, object values) where T : QueryParameterInfo, new()
        {
            var matches = Regex.Matches(query, pattern);
            foreach (Match match in matches)
            {
                var info = new T()
                {
                    Name = match.Groups["name"].Value,
                    Start = match.Groups[0].Index,
                    End = match.Groups[0].Index + match.Groups[0].Length,
                };

                info.Value = GetParameterValue(info.Name, values);

                yield return info;
            }
        }

        private static object GetParameterValue(string name, object values)
        {
            var property = values.GetType().GetProperty(name);
            if (property == null)
            {
                var message = string.Format("No value specified for parameter '{0}'.", name);
                throw new ArgumentException(message);
            }

            return property.GetValue(values);
        }

        private static string ReplaceParameters(string query, IEnumerable<QueryParameterInfo> parameters, object values)
        {
            var result = new StringBuilder();
            int index = 0;
            string copy;

            foreach (var parameter in parameters)
            {
                copy = query.Substring(index, parameter.Start - index);
                result.Append(copy);
                result.Append(parameter.Value);

                index = parameter.End;
            }

            copy = query.Substring(index);
            result.Append(copy);

            return result.ToString();
        }
    }
}