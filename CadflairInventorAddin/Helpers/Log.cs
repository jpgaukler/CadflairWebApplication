using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadflairInventorAddin.Helpers
{
    internal class Log
    {
        public static void Error(Exception ex, string methodName, object arg1 = null, object arg2 = null, object arg3 = null, object arg4 = null, object arg5 = null, object arg6 = null, object arg7 = null, object arg8 = null)
        {
            string args = string.Empty;

            if (arg1 != null) args = $"{arg1}";
            if (arg2 != null) args += $", {arg2}"; 
            if (arg3 != null) args += $", {arg3}"; 
            if (arg4 != null) args += $", {arg4}"; 
            if (arg5 != null) args += $", {arg5}"; 
            if (arg6 != null) args += $", {arg6}"; 
            if (arg7 != null) args += $", {arg7}"; 
            if (arg8 != null) args += $", {arg8}"; 

            Trace.TraceError($"{DateTimeOffset.Now} - {methodName}({args}) failed:");
            Trace.WriteLine(ex.ToString());
            Trace.WriteLine("");
        }

        public static void Info(string message)
        {
            Trace.TraceInformation($"{DateTimeOffset.Now} - {message}");
            Trace.WriteLine("");
        }


    }
}
