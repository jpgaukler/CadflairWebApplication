using Autodesk.Forge.Core;
using Autodesk.Forge.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace CadflairForgeAccess.Helpers
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
        internal static string ToBase64(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Base64 encode a string
        /// </summary>
        internal static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}
