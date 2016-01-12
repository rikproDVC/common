using System.Collections.Generic;
using System.Resources;
using System.Reflection;
using System.Text;
using System;
using System.Linq;

namespace Lisa.Common.Errors
{
    public class ErrorBuilder
    {
        /// <summary>
        /// Initializes the errorbuilder's resources. 
        /// Additional error resources can be supplied in the parameters.
        /// </summary>
        public static void Initialize(params ResourceManager[] expansionResources)
        {
            _errorManager = ErrorMessages.ResourceManager;
            _expansionErrorManagers = expansionResources.ToList() ?? null;
        }

        /// <summary>
        /// Looks up error text using error code, and interpolates supplied variables as needed.
        /// Supported error codes can be found in the documentation.
        /// </summary>
        /// <param name="code">The error code corresponding to the error resource.</param>
        /// <param name="obj">Anonymous object of error message variables. Unneeded variables are ignored.</param>
        /// <returns>
        /// An error object with formatted message.
        /// If the error code is not found, the message is empty.
        /// </returns>
        public static Error BuildError(int code, dynamic obj = null)
        {
            Dictionary<string, string> dict = GetErrorParamDictionary(obj);

            string errorText = GetErrorText(code);
            errorText = FormatError(errorText, dict);

            return new Error
            {
                Code = code,
                Message = errorText,
                Values = obj
            };
        }

        private static string GetErrorText(int code)
        {
            string message = null;
            
            // Try and find the error in custom error resources
            foreach (var manager in _expansionErrorManagers)
            {
                var s = manager.GetString("e" + code);

                if (s != null)
                {
                    message = s;
                }
            }

            // Error not found in custom error resources, either get the error message from the default error list or return an empty message.
            if (message == null)
            {
                return _errorManager.GetString("e" + code) ?? string.Empty;
            }

            return message;
        }

        private static Dictionary<string,string> GetErrorParamDictionary(object obj)
        {
            var dict = new Dictionary<string, string>();

            if (obj == null)
            {
                return dict;
            }

            foreach(var property in obj.GetType().GetProperties())
            {
                var key = property.Name;
                string value = null;
                
                if (property.GetValue(obj) as IEnumerable<string> != null)
                {
                    value = string.Join(", ", property.GetValue(obj) as IEnumerable<string>);
                }
                else if (property.GetValue(obj) as int? != null)
                {
                    value = (property.GetValue(obj) as int?).ToString();
                }
                else
                {
                    value = property.GetValue(obj) as string;
                }

                if (value == null)
                {
                    throw new ArgumentException("Value object properties may only be of types string, int or IEnumerable.");
                }

                dict.Add(key, value);
            }

            return dict;
        }

        private static string FormatError(string errorString, Dictionary<string, string> errorParamDict)
        {
            StringBuilder formattedString = new StringBuilder(errorString);

            foreach (var param in errorParamDict)
            {
                formattedString = formattedString.Replace("{" + param.Key + "}", param.Value);
            }

            return formattedString.ToString();
        }

        private static ResourceManager _errorManager;
        private static List<ResourceManager> _expansionErrorManagers;
    }
}
