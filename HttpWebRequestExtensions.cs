using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace WebLib
{
    public static class HttpWebRequestExtensions
    {
        private static Dictionary<string, PropertyInfo> _optionProperties = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);

        static HttpWebRequestExtensions()
        {
            Type type = typeof(HttpWebRequest);

            foreach (PropertyInfo property in type.GetProperties())
                _optionProperties[property.Name] = property;
        }

        public static void SetOption(this HttpWebRequest request, string name, object value)
        {
            string optionName = name.Replace("-", "");
            if (_optionProperties.ContainsKey(optionName))
            {
                PropertyInfo property = _optionProperties[optionName];
                property.SetValue(request, value);
            }
            else
            {
                request.Headers[name] = value.ToString();
            }
        }
    }
}
