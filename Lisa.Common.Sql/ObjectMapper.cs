using System.Collections.Generic;
using System.Dynamic;

namespace Lisa.Common.Sql
{
    public class ObjectMapper
    {
        internal class SubObjectInfo
        {
            public SubObjectInfo()
            {
                Fields = new List<KeyValuePair<string, object>>();
            }

            public string Name { get; set; }
            public IList<KeyValuePair<string, object>> Fields { get; set; }
        }

        internal class RowInfo
        {
            public RowInfo()
            {
                Scalars = new List<KeyValuePair<string, object>>();
                SubObjects = new List<SubObjectInfo>();
                Lists = new List<SubObjectInfo>();
            }

            public object Identity { get; set; }
            public IList<KeyValuePair<string, object>> Scalars { get; set; }
            public IList<SubObjectInfo> SubObjects { get; set; }
            public IList<SubObjectInfo> Lists { get; set; }
        }

        public ExpandoObject Single(IRowProvider row)
        {
            return MapObject(row.Fields);
        }

        public IEnumerable<ExpandoObject> Many(IDataProvider table)
        {
            foreach (var row in table.Rows)
            {
                yield return MapObject(row.Fields);
            }


            // NOTE: Is this called when enumerator never reaches end of collection?
            // Also, what happens if you walk over two IDataProviders with the same ObjectMapper?
            _objects.Clear();
        }

        private ExpandoObject MapObject(IEnumerable<KeyValuePair<string, object>> fields)
        {
            ExpandoObject obj;

            var rowInfo = GetRowInfo(fields);
            if (!_objects.ContainsKey(rowInfo.Identity))
            {
                obj = new ExpandoObject();
                MapScalars(obj, rowInfo);
                MapSubObjects(obj, rowInfo);
            }
            else
            {
                obj = _objects[rowInfo.Identity];
            }

            MapLists(obj, rowInfo);

            return obj;
        }

        private void MapScalars(IDictionary<string, object> obj, RowInfo rowInfo)
        {
            foreach (var field in rowInfo.Scalars)
            {
                obj.Add(field);
            }
        }

        private void MapSubObjects(IDictionary<string, object> obj, RowInfo rowInfo)
        {
            foreach (var subObjectInfo in rowInfo.SubObjects)
            {
                var subObject = MapObject(subObjectInfo.Fields);
                obj.Add(subObjectInfo.Name, subObject);
            }
        }

        private void MapLists(IDictionary<string, object> obj, RowInfo rowInfo)
        {
            foreach (var listInfo in rowInfo.Lists)
            {
                if (!obj.ContainsKey(listInfo.Name))
                {
                    obj.Add(listInfo.Name, new List<ExpandoObject>());
                }

                var list = (IList<ExpandoObject>) obj[listInfo.Name];
                var listItem = MapObject(listInfo.Fields);
                obj.Add(listInfo.Name, listItem);
            }
        }

        private RowInfo GetRowInfo(IEnumerable<KeyValuePair<string, object>> fields)
        {
            var info = new RowInfo();
            var subObjects = new Dictionary<string, SubObjectInfo>();
            var lists = new Dictionary<string, SubObjectInfo>();

            foreach (var field in fields)
            {
                if (IsIdentity(field))
                {
                    info.Identity = field.Value;
                }
                else if (IsSubObjectField(field))
                {
                    var subObjectName = field.Key.Substring(0, field.Key.IndexOf("_"));
                    var subFieldName = field.Key.Substring(field.Key.IndexOf("_") + 1);
                    var subField = new KeyValuePair<string, object>(subFieldName, field.Value);

                    if (!subObjects.ContainsKey(subObjectName))
                    {
                        var subObjectInfo = new SubObjectInfo();
                        subObjectInfo.Name = subObjectName;
                        subObjectInfo.Fields.Add(subField);
                        subObjects.Add(subObjectName, subObjectInfo);

                        info.SubObjects.Add(subObjectInfo);
                    }
                    else
                    {
                        var subObjectInfo = subObjects[subObjectName];
                        subObjectInfo.Fields.Add(subField);
                    }
                }
                else if (IsListField(field))
                {
                    var listName = field.Key.Substring(1, field.Key.IndexOf("_") - 1);
                    var subFieldName = field.Key.Substring(field.Key.IndexOf("_") + 1);
                    var subField = new KeyValuePair<string, object>(subFieldName, field.Value);

                    if (!lists.ContainsKey(listName))
                    {
                        var listInfo = new SubObjectInfo();
                        listInfo.Name = listName;
                        listInfo.Fields.Add(subField);
                        lists.Add(listName, listInfo);

                        info.Lists.Add(listInfo);
                    }
                    else
                    {
                        var listInfo = lists[listName];
                        listInfo.Fields.Add(subField);
                    }
                }
            }

            return info;
        }

        private bool IsIdentity(KeyValuePair<string, object> field)
        {
            return field.Key.StartsWith("@");
        }

        private bool IsSubObjectField(KeyValuePair<string, object> field)
        {
            return field.Key.Contains("_") && field.Key.StartsWith("#");
        }

        private bool IsListField(KeyValuePair<string, object> field)
        {
            return field.Key.StartsWith("#") && field.Key.Contains("_");
        }

        private IDictionary<object, ExpandoObject> _objects = new Dictionary<object, ExpandoObject>();


        private static IEnumerable<KeyValuePair<string, object>> GetSimpleFields(IRowProvider row)
        {
            foreach (var field in row.Fields)
            {
                if (!field.Key.StartsWith("#") && !field.Key.Contains("_"))
                {
                    yield return field;
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> GetSubObjectFields(IRowProvider row)
        {
            foreach (var field in row.Fields)
            {
                if (field.Key.Contains("_") && !field.Key.StartsWith("#"))
                {
                    yield return field;
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, object>> GetListFields(IRowProvider row)
        {
            foreach (var field in row.Fields)
            {
                if (field.Key.StartsWith("#") && field.Key.Contains("_"))
                {
                    yield return field;
                }
            }
        }
    }
}