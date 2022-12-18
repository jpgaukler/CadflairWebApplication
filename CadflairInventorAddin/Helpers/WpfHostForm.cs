using Inventor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CadflairInventorAddin.Helpers
{
    /// <summary>
    /// This is a WinForm that creates an Inventor Dockable window and embeds itself inside of it. The form displays an ElementHost which embeds a WPF user control inside of it.
    /// </summary>
    public partial class WpfHostForm : Form
    {
        private readonly DockableWindow _dockableWindow;
        private readonly UserInterfaceManager _userInterfaceManager;
        public bool IsOpen { get; set; }

        public WpfHostForm(UIElement wpfControl, string dockableWindowInternalName, string dockableWindowTitle)
        {
            InitializeComponent();

            // embed the wpf control in the form
            elementHost1.Child = wpfControl;

            // create the dockable window
            _userInterfaceManager = Globals.InventorApplication.UserInterfaceManager;

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
            _dockableWindow.AddChild(Handle);

            // add event handlers for the dockable window
            _userInterfaceManager.DockableWindows.Events.OnHide += DockableWindow_OnHide;
            _userInterfaceManager.DockableWindows.Events.OnHelp += DockableWindow_OnHelp;
        }

        private void WpfHostForm_Shown(object sender, EventArgs e)
        {
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

            Close();
        }

        private void DockableWindow_OnHelp(DockableWindow DockableWindow, NameValueMap Context, out HandlingCodeEnum HandlingCode)
        {
            HandlingCode = HandlingCodeEnum.kEventNotHandled;

            if (DockableWindow.InternalName != _dockableWindow.InternalName) return;

            // ADD A LINK TO CADFLAIR HELP PAGE HERE
            Process.Start("http://www.cadflair.com/");

            HandlingCode = HandlingCodeEnum.kEventHandled;
        }

    }
}
