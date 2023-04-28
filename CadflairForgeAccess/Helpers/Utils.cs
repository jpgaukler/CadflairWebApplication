namespace CadflairForgeAccess.Helpers
{
    internal static class Utils
    {

        /// <summary>
        /// Base64 encode a string
        /// </summary>
        internal static string ToBase64String(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Base64 encode a string
        /// </summary>
        internal static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}
