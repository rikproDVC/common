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
                Arrays = new List<KeyValuePair<string, object>>();
            }

            public object Identity { get; set; }
            public IList<KeyValuePair<string, object>> Scalars { get; set; }
            public IList<SubObjectInfo> SubObjects { get; set; }
            public IList<SubObjectInfo> Lists { get; set; }
            public IList<KeyValuePair<string, object>> Arrays { get; set; }
        }

        public ExpandoObject Single(IRowProvider row)
        {
            return MapObject(row.Fields);
        }

        public IEnumerable<ExpandoObject> Many(IDataProvider table)
        {
            // NOTE: what happens if you walk over two IDataProviders with the same ObjectMapper?
            _objects.Clear();

            foreach (var row in table.Rows)
            {
                MapObject(row.Fields);
            }

            return _objects.Values;
        }

        private ExpandoObject MapObject(IEnumerable<KeyValuePair<string, object>> fields)
        {
            ExpandoObject obj;

            var rowInfo = GetRowInfo(fields);
            if (!_objects.ContainsKey(rowInfo.Identity))
            {
                obj = new ExpandoObject();
                _objects.Add(rowInfo.Identity, obj);
                MapScalars(obj, rowInfo);
            }
            else
            {
                obj = _objects[rowInfo.Identity];
            }

            MapSubObjects(obj, rowInfo);
            MapLists(obj, rowInfo);
            MapArrays(obj, rowInfo);

            return obj;
        }

        private void MapObject(ExpandoObject obj, IEnumerable<KeyValuePair<string, object>> fields)
        {
            // This function is only used to add items to lists and arrays inside sub objects (and 
            // sub objects inside sub objects etc.), so the scalars have already been mapped.

            var rowInfo = GetRowInfo(fields);
            MapSubObjects(obj, rowInfo);
            MapLists(obj, rowInfo);
            MapArrays(obj, rowInfo);
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
                if (obj.ContainsKey(subObjectInfo.Name))
                {
                    var subObject = obj[subObjectInfo.Name];
                    new ObjectMapper().MapObject((ExpandoObject) subObject, subObjectInfo.Fields);
                }
                else
                {
                    var subObject = new ObjectMapper().MapObject(subObjectInfo.Fields);
                    obj.Add(subObjectInfo.Name, subObject);
                }
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
                var listItem = new ObjectMapper().MapObject(listInfo.Fields);
                list.Add(listItem);
            }
        }

        private void MapArrays(IDictionary<string, object> obj, RowInfo rowInfo)
        {
            foreach (var field in rowInfo.Arrays)
            {
                if (!obj.ContainsKey(field.Key))
                {
                    obj.Add(field.Key, new List<object>());
                }

                var array = (IList<object>) obj[field.Key];
                array.Add(field.Value);
            }
        }

        private RowInfo GetRowInfo(IEnumerable<KeyValuePair<string, object>> fields)
        {
            var info = new RowInfo();
            var subObjects = new Dictionary<string, SubObjectInfo>();
            var lists = new Dictionary<string, SubObjectInfo>();

            // If a row doesn't have an explicit identity set, it gets assigned a unique identity.
            info.Identity = new object();

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
                else if (IsArrayField(field))
                {
                    var arrayName = field.Key.Substring(1);
                    info.Arrays.Add(new KeyValuePair<string, object>(arrayName, field.Value));
                }
                else
                {
                    info.Scalars.Add(field);
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
            return field.Key.Contains("_") && !field.Key.StartsWith("#");
        }

        private bool IsListField(KeyValuePair<string, object> field)
        {
            return field.Key.StartsWith("#") && field.Key.Contains("_");
        }

        private bool IsArrayField(KeyValuePair<string, object> field)
        {
            return field.Key.StartsWith("#") && !field.Key.Contains("_");
        }

        private IDictionary<object, ExpandoObject> _objects = new Dictionary<object, ExpandoObject>();
    }
}