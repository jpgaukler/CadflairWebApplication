using Newtonsoft.Json;
using System.Diagnostics;

namespace CadflairInventorPlugin.Helpers
{
    internal static class Globals
    {
        /// <summary>
        /// Global Inventor Application object.
        /// </summary>
        internal static Inventor.InventorServer InventorApplication;

        internal static void ReportProgress(string message)
        {
            dynamic progress = new
            {
                message,
            };

            string progressJson = JsonConvert.SerializeObject(progress);
            Trace.TraceInformation("!ACESAPI:acesHttpOperation({0},\"\",\"\",{1},null)", "onProgress", progressJson);
        }
    }
}
