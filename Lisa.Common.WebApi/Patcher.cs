using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Lisa.Common.WebApi
{
    public static class Patcher
    {
        public static void Apply(IEnumerable<Patch> patches, object obj)
        {
            foreach (var patch in patches)
            {
                switch (patch.Action.ToLower())
                {
                    case "replace":
                        ApplyReplace(patch, obj);
                        break;
                }
            }
        }

        private static void ApplyReplace(Patch patch, object obj)
        {
            var property = GetProperty(obj, patch.Field);
            SetProperty(obj, property, patch.Value);
        }

        private static PropertyInfo GetProperty(object obj, string field)
        {
            var type = obj.GetType();
            return type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }

        private static void SetProperty(object obj, PropertyInfo property, JToken value)
        {
            property.SetValue(obj, value?.ToObject(property.PropertyType));
        }
    }
}