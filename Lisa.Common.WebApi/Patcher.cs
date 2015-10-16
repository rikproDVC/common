using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Lisa.Common.WebApi
{
    public static class Patcher
    {
        public static IEnumerable<string> Apply(IEnumerable<Patch> patches, object obj)
        {
            var errors = ValidatePatches(patches, obj);
            if (errors.Count == 0)
            {
                ApplyPatches(patches, obj);
            }

            return errors;
        }

        private static IList<string> ValidatePatches(IEnumerable<Patch> patches, object obj)
        {
            var errors = new List<string>();
            int index = 0;

            foreach (var patch in patches)
            {
                ValidateAction(patch.Action, index, errors);
                ValidateField(patch.Field, obj, index, errors);
                if (errors.Count == 0)
                {
                    ValidateValue(patch, obj, index, errors);
                }

                index++;
            }

            return errors;
        }

        private static void ValidateAction(string action, int index, IList<string> errors)
        {
            switch (action.ToLower())
            {
                case "replace":
                case "add":
                case "remove":
                    break;

                default:
                    var error = string.Format("Cannot apply patch #{0}, because '{1}' is not a valid action.", index, action);
                    errors.Add(error);
                    break;
            }
        }

        private static void ValidateField(string field, object obj, int index, IList<string> errors)
        {
            if (GetProperty(obj, field) == null)
            {
                var error = string.Format("Cannot apply patch #{0}, because the field '{1}' doesn't exist.", index, field);
                errors.Add(error);
            }
        }

        private static void ValidateValue(Patch patch, object obj, int index, IList<string> errors)
        {
            switch (patch.Action.ToLower())
            {
                case "replace":
                    ValidatePropertyType(patch, obj, index, errors);
                    break;

                case "add":
                case "remove":
                    ValidateListType(patch, obj, index, errors);
                    break;
            }
        }

        private static void ValidatePropertyType(Patch patch, object obj, int index, IList<string> errors)
        {
            var property = GetProperty(obj, patch.Field);

            try
            {
                patch.Value?.ToObject(property.PropertyType);
            }
            catch
            {
                var error = string.Format("Cannot apply patch #{0}. Cannot replace the value of field '{1}', because the value cannot be converted to a {2}.", index, patch.Field, property.PropertyType);
                errors.Add(error);
            }
        }

        private static void ValidateListType(Patch patch, object obj, int index, IList<string> errors)
        {
            var property = GetProperty(obj, patch.Field);
            if (!property.PropertyType.IsConstructedGenericType ||
                !property.PropertyType.Implements(typeof(IList<>)))
            {
                var error = string.Format("Cannot apply patch #{0}. Cannot add to field '{1}', because it's not a list.", index, patch.Field);
                errors.Add(error);
            }
            else
            {
                ValidateElementType(patch, obj, index, errors);
            }
        }

        private static void ValidateElementType(Patch patch, object obj, int index, IList<string> errors)
        {
            var list = GetPropertyAsList(obj, patch.Field);
            var elementType = GetElementType(list);

            try
            {
                patch.Value?.ToObject(elementType);
            }
            catch
            {
                var error = string.Format("Cannot apply patch #{0}. Cannot add to field '{1}', because the given value cannot be converted to a {2}.", index, patch.Field, elementType);
                errors.Add(error);
            }
        }

        private static void ApplyPatches(IEnumerable<Patch> patches, object obj)
        {
            foreach (var patch in patches)
            {
                switch (patch.Action.ToLower())
                {
                    case "replace":
                        ApplyReplace(patch, obj);
                        break;

                    case "add":
                        ApplyAdd(patch, obj);
                        break;

                    case "remove":
                        ApplyRemove(patch, obj);
                        break;
                }
            }
        }

        private static void ApplyReplace(Patch patch, object obj)
        {
            var property = GetProperty(obj, patch.Field);
            SetProperty(obj, property, patch.Value);
        }

        private static void ApplyAdd(Patch patch, object obj)
        {
            IList property = GetPropertyAsList(obj, patch.Field);
            Type elementType = GetElementType(property);
            var value = patch.Value.ToObject(elementType);

            property.Add(value);
        }

        private static void ApplyRemove(Patch patch, object obj)
        {
            IList property = GetPropertyAsList(obj, patch.Field);
            Type elementType = GetElementType(property);
            var value = patch.Value.ToObject(elementType);

            property.Remove(value);
        }

        private static PropertyInfo GetProperty(object obj, string field)
        {
            var type = obj.GetType();
            return type.GetProperty(field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        }

        private static IList GetPropertyAsList(object obj, string field)
        {
            PropertyInfo property = GetProperty(obj, field);
            return (IList) property.GetValue(obj);
        }

        private static Type GetElementType(IList list)
        {
            return list.GetType().GetGenericArguments()[0];
        }

        private static bool Implements(this Type child, Type ancestor)
        {
            if (child.GetGenericTypeDefinition() == ancestor)
            {
                return true;
            }

            foreach (var @interface in child.GetInterfaces())
            {
                if (@interface.GetGenericTypeDefinition() == ancestor)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetProperty(object obj, PropertyInfo property, JToken value)
        {
            property.SetValue(obj, value?.ToObject(property.PropertyType));
        }
    }
}