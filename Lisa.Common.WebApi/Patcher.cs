﻿using Newtonsoft.Json.Linq;
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

        private static void ApplyPatches(IEnumerable<Patch> patches, object obj)
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