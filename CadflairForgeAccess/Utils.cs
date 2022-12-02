using Autodesk.Forge.Core;
using Autodesk.Forge.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace CadflairForgeAccess
{
    internal static class Utils
    {

        /// <summary>
        /// Reads appsettings from web.config
        /// </summary>
        internal static string GetAppSetting(string settingKey)
        {
            string? appSettingValue = Environment.GetEnvironmentVariable(settingKey)?.Trim();
            return appSettingValue ?? string.Empty;
        }

        /// <summary>
        /// Base64 encode a string
        /// </summary>
        internal static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Create a list of UploadItemDesc for a single file. This can be used with the Objects.API.uploadResources() method for uploading files directly to Amazon S3.
        /// </summary>
        /// <param name="objectKey"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
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
