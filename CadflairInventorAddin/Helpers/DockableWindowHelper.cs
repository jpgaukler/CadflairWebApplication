using Inventor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace CadflairInventorAddin.Helpers
{
    /// <summary>
    /// This class creates a dockable window and embeds a WPF window inside of it. 
    /// </summary>
    internal class DockableWindowHelper
    {
        private readonly Window _wpfWindow;
        private readonly DockableWindow _dockableWindow;
        private readonly UserInterfaceManager _userInterfaceManager;

        public bool IsOpen { get; set; }

        public DockableWindowHelper(Window wpfWindow, string dockableWindowInternalName, string dockableWindowTitle)
        {
            _userInterfaceManager = Globals.InventorApplication.UserInterfaceManager;

            // capture reference to window
            _wpfWindow = wpfWindow;
            _wpfWindow.WindowStyle = System.Windows.WindowStyle.None;
            _wpfWindow.ResizeMode = System.Windows.ResizeMode.NoResize;

            // create the dockable window
            try
            {
                _dockableWindow = _userInterfaceManager.DockableWindows[dockableWindowInternalName];
                _dockableWindow.Title = dockableWindowTitle;
            }
            catch
            {
                _dockableWindow = _userInterfaceManager.DockableWindows.Add(Globals.AddInCLSIDString, dockableWindowInternalName, dockableWindowTitle);
            }

            _dockableWindow.SetMinimumSize(400, 550);
            _dockableWindow.ShowVisibilityCheckBox = false;
            _dockableWindow.ShowTitleBar = true;

            // attache wpf Window to DockableWindow
            WindowInteropHelper helper = new WindowInteropHelper(wpfWindow);
            helper.EnsureHandle();
            _dockableWindow.AddChild(helper.Handle);

            // Set key hook.
            HwndSource.FromHwnd(helper.Handle).AddHook(WndProc);

            // add event handlers for the dockable window
            _userInterfaceManager.DockableWindows.Events.OnHide += DockableWindow_OnHide;
            _userInterfaceManager.DockableWindows.Events.OnHelp += DockableWindow_OnHelp;
        }

        public void Show()
        {
            _wpfWindow.Show();
            _dockableWindow.Visible = true;
            IsOpen = true;
        }

        private void DockableWindow_OnHide(DockableWindow dockableWindow, EventTimingEnum beforeOrAfter, NameValueMap context, out HandlingCodeEnum handlingCode)
        {
            handlingCode = HandlingCodeEnum.kEventNotHandled;

            if (dockableWindow.InternalName != _dockableWindow.InternalName) return;
            if (beforeOrAfter == EventTimingEnum.kAfter) return;

            handlingCode = HandlingCodeEnum.kEventHandled;

            _dockableWindow.Clear();
            _userInterfaceManager.DockableWindows.Events.OnHide -= DockableWindow_OnHide;
            _userInterfaceManager.DockableWindows.Events.OnHelp -= DockableWindow_OnHelp;
            IsOpen = false;

            _wpfWindow.Close();
        }

        private void DockableWindow_OnHelp(DockableWindow DockableWindow, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            HandlingCode = HandlingCodeEnum.kEventNotHandled;

            if (DockableWindow.InternalName != _dockableWindow.InternalName) return;

            // ADD A LINK TO CADFLAIR HELP PAGE HERE
            Process.Start("http://www.cadflair.com/");

            HandlingCode = HandlingCodeEnum.kEventHandled;
        }




        private const UInt32 DLGC_WANTARROWS = 0x0001;
        private const UInt32 DLGC_WANTTAB = 0x0002;
        private const UInt32 DLGC_WANTALLKEYS = 0x0004;
        private const UInt32 DLGC_HASSETSEL = 0x0008;
        private const UInt32 DLGC_WANTCHARS = 0x0080;
        private const UInt32 WM_GETDLGCODE = 0x0087;

        /// <summary>
        /// This is a helper for adding a wpf window to a dockable control. This was pulled from an Autodesk forum.<br></br>
        /// See https://forums.autodesk.com/t5/inventor-forum/dockable-window-with-wpf-controls-don-t-receive-keyboard-input/td-p/9115997 for more info.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        public static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETDLGCODE)
            {
                handled = true;
                return new IntPtr(DLGC_WANTCHARS | DLGC_WANTARROWS | DLGC_HASSETSEL | DLGC_WANTTAB | DLGC_WANTALLKEYS);
            }
            return IntPtr.Zero;
        }
    }
}
