using System.IO;
using System.Reflection;

namespace CadflairInventorAddin.Helpers
{
    internal static class Globals
    {
        /// <summary>
        /// Global Inventor Application object.
        /// </summary>
        public static Inventor.Application InventorApplication { get; set; }

        public static string AddInCLSIDString { get; set; }
        public static string OutputLogPath { get => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output.log"); }
    }
}
