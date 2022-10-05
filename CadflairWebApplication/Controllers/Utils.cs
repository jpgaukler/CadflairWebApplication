using Autodesk.Forge.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace Forge.Controllers
{
    internal static class Utils
    {

        /// <summary>
        /// Reads appsettings from web.config
        /// </summary>
        internal static string GetAppSetting(string settingKey)
        {
            return Environment.GetEnvironmentVariable(settingKey).Trim();
        }

        /// <summary>
        /// Base64 encode a string
        /// </summary>
        internal static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        internal static List<UploadItemDesc> CreateSingleUploadList(string objectKey, Stream stream)
        {
            var uploadList = new List<UploadItemDesc>
            {
                new UploadItemDesc (objectKey, stream)
            };

            return uploadList;
        }

    }
}
