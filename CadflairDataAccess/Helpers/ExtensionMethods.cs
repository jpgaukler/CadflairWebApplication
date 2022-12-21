using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CadflairDataAccess.Helpers
{
    internal static class ExtensionMethods
    {

        /// <summary>
        /// Serialize an object to a json string. Default settings are: <br/><br/>
        /// NullValueHandling = Ignore <br/>
        /// DefaultValueHandling = Ignore
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="settings"></param>
        /// <returns>A json string representing the object</returns>
        internal static string ToJson<T>(this T obj, JsonSerializerSettings settings = null)
        {
            if(settings == null)
            {
                settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.Ignore //ignore empty strings and arrays
                };
            }

            return JsonConvert.SerializeObject(obj, settings); ;
        }
    }

}
