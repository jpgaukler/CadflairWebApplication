using System;

namespace CadflairInventorLibrary.Helpers
{
    public class InventorMainFrame : System.Windows.Forms.IWin32Window
    {
        public IntPtr Handle { get; set; }

        public InventorMainFrame(long hWnd)
        {
            Handle = (IntPtr)hWnd;
        }
    }
}
