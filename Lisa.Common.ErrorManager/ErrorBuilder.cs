using System.Collections.Generic;
using System.Resources;
using System.Reflection;
using System.Text;
using System;
using System.Linq;

namespace Lisa.Common.ErrorManager
{
    public class ErrorBuilder
    {
        /// <summary>
        /// Initializes the errorbuilder's resources. 
        /// Additional error resources can be supplied in the parameters.
        /// </summary>
        public static void Initialize(params ResourceManager[] expansionResources)
        {
            Initialize(null, expansionResources);
        }

        /// <summary>
        /// Initializes the errorbuilder's resources. 
        /// Additional error resources can be supplied in the parameters.
        /// <param name="translations">A dictionary of translation files for each supported locale.</param>
        /// </summary>
        public static void Initialize(Dictionary<string, ResourceManager> translations, params ResourceManager[] expansionResources)
        {
            _errorManager = ErrorMessages.ResourceManager;
            _errorMessageTranslations = translations;
            _expansionErrorManagers = expansionResources.ToList() ?? null;
        }

        /// <summary>
        /// Looks up error text using error code, and interpolates supplied variables as needed.
        /// Supported error codes can be found [here](lnk)
        /// </summary>
        /// <param name="code">The error code corresponding to the error resource</param>
        /// <returns>
        /// An error object with formatted message.
        /// If the error code is not found, the message is empty.
        /// </returns>
        public static Error BuildError(int code)
        {
            return BuildError(code, null, null);
        }

        /// <summary>
        /// Looks up error text using error code, and interpolates supplied variables as needed.
        /// Supported error codes can be found [here](lnk)
        /// </summary>
        /// <param name="code">The error code corresponding to the error resource</param>
        /// <param name="obj">Anonymous object of error message variables. Unneeded variables are ignored.</param>
        /// <returns>
        /// An error object with formatted message.
        /// If the error code is not found, the message is empty.
        /// </returns>
        public static Error BuildError(int code, dynamic obj)
        {
            return BuildError(code, obj, null);
        }

        /// <summary>
        /// Looks up error text using error code, and interpolates supplied variables as needed.
        /// Supported error codes can be found [here](lnk)
        /// </summary>
        /// <param name="code">The error code corresponding to the error resource</param>
        /// <param name="locale">Name of localized error resource provided in Initialize()</param>
        /// <returns>
        /// An error object with formatted message.
        /// If the error code is not found, the message is empty.
        /// </returns>
        public static Error BuildError(int code, string locale)
        {
            return BuildError(code, null, locale);
        }

        /// <summary>
        /// Looks up error text using error code, and interpolates supplied variables as needed.
        /// Supported error codes can be found [here](lnk)
        /// </summary>
        /// <param name="code">The error code corresponding to the error resource</param>
        /// <param name="obj">Anonymous object of error message variables. Unneeded variables are ignored.</param>
        /// <param name="locale">Name of localized error resource provided in Initialize()</param>
        /// <returns>
        /// An error object with formatted message.
        /// If the error code is not found, the message is empty.
        /// </returns>
        public static Error BuildError(int code, dynamic obj, string locale)
        {
            Dictionary<string, string> dict = _GetErrorParamDictionary(obj);

            string errorText = _GetErrorText(code, locale);
            errorText = _FormatError(errorText, dict);

            return new Error
            {
                Code = code,
                Message = errorText
            };
        }

        private static string _GetErrorText(int code, string locale)
        {
            string message = null;

            // Try find a translated error message in the specified translation resources
            if (_errorMessageTranslations != null && locale != null && _errorMessageTranslations.ContainsKey(locale))
            {
                ResourceManager translationManager = null;
                if (_errorMessageTranslations.TryGetValue(locale, out translationManager))
                {
                    message = translationManager.GetString("e" + code);
                }
            }

            // If the translation was not found, try finding the error in user-defined error resources
            if (message == null)
            {
                foreach (var manager in _expansionErrorManagers)
                {
                    var s = manager.GetString("e" + code);

                    if (s != null)
                    {
                        message = s;
                    }
                }
            }

            // Error still not found, either get the error message from the default error list or return an empty message.
            if (message == null)
            {
                return _errorManager.GetString("e" + code) ?? string.Empty;
            }

            return message;
        }

        private static Dictionary<string,string> _GetErrorParamDictionary(object obj)
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
                    throw new ArgumentException();
                }

                dict.Add(key, value);
            }

            return dict;
        }

        private static string _FormatError(string errorString, Dictionary<string, string> errorParamDict)
        {
            //int i = 0;
            //StringBuilder newFormatString = new StringBuilder(errorString);
            //Dictionary<string, int> keyToInt = new Dictionary<string, int>();

            //foreach (var value in ValueDict)
            //{
            //    newFormatString = newFormatString.Replace("{" + value.Key + "}", "{" + i.ToString() + "}");
            //    keyToInt.Add(value.Key, i);
            //    i++;
            //}

            //return string.Format(newFormatString.ToString(), ValueDict.OrderBy(x => keyToInt[x.Key]).Select(x => x.Value).ToArray());

            StringBuilder formattedString = new StringBuilder(errorString);

            foreach (var param in errorParamDict)
            {
                formattedString = formattedString.Replace("{" + param.Key + "}", param.Value);
            }

            return formattedString.ToString();
        }

        private static ResourceManager _errorManager;
        private static List<ResourceManager> _expansionErrorManagers;
        private static Dictionary<string, ResourceManager> _errorMessageTranslations;
    }
}
