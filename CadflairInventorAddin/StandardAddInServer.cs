using CadflairInventorAddin.Commands;
using CadflairInventorAddin.Commands.Upload;
using CadflairInventorAddin.Properties;
using CadflairInventorAddin.Helpers;
using Inventor;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using CadflairInventorAddin.Api;
using CadflairInventorLibrary.Helpers;
using System.Reflection;

namespace CadflairInventorAddin
{
    /// <summary>
    /// This is the primary AddIn Server class that implements the ApplicationAddInServer interface
    /// that all Inventor AddIns are required to implement. The communication between Inventor and
    /// the AddIn is via the methods on this interface.
    /// </summary>
    [GuidAttribute("DCC57B37-B24B-4A47-BB10-68BF826C9488")]
    public class StandardAddInServer : Inventor.ApplicationAddInServer
    {
        public StandardAddInServer()
        {
        }

        #region ApplicationAddInServer Members

        private UserInterfaceManager _userInterfaceManager;
        private string _outputLogPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "output.log");

        //private ButtonDefinition _addDimensionAttributesButton;
        //private ButtonDefinition _refreshDimensionsButton;


        /// <summary>
        /// This method is called by Inventor when the AddIn is loaded.
        /// </summary>
        public void Activate(Inventor.ApplicationAddInSite addInSiteObject, bool firstTime)
        {
            // This method is called by Inventor when it loads the addin.
            // The AddInSiteObject provides access to the Inventor Application object.
            // The FirstTime flag indicates if the addin is loaded for the first time.

            // Initialize AddIn members.
            Globals.InventorApplication = addInSiteObject.Application;

            //retrieve the GUID for this class
            GuidAttribute addInCLSID = (GuidAttribute)GuidAttribute.GetCustomAttribute(typeof(StandardAddInServer), typeof(GuidAttribute));
            Globals.AddInCLSIDString = "{" + addInCLSID.Value + "}";

            // add trace listener for logging errors
            System.IO.File.Delete(_outputLogPath);
            Trace.Listeners.Add(new TextWriterTraceListener(_outputLogPath));
            Trace.AutoFlush = true;

            // add user interface manager
            _userInterfaceManager = Globals.InventorApplication.UserInterfaceManager;
            _userInterfaceManager.UserInterfaceEvents.OnResetRibbonInterface += UserInterfaceEvents_OnResetRibbonInterface;

            // setup button definitions
            ControlDefinitions controlDefs = Globals.InventorApplication.CommandManager.ControlDefinitions;

            UploadToCadflair.UploadToCadflairButton = controlDefs.AddButtonDefinition(DisplayName: "Upload\nProduct",
                                                                                      InternalName: "Cadflair Upload Command",
                                                                                      Classification: CommandTypesEnum.kNonShapeEditCmdType,
                                                                                      ClientId: Globals.AddInCLSIDString,
                                                                                      DescriptionText: "Command to upload a model to the Cadflair platform.",
                                                                                      ToolTipText: "Upload the active model to Cadflair.",
                                                                                      StandardIcon: Resources.UploadSmall.ToIPictureDisp(),
                                                                                      LargeIcon: Resources.UploadLarge.ToIPictureDisp());

            ExportSvf.ExportSvfButton = controlDefs.AddButtonDefinition(DisplayName: "Export Svf",
                                                                        InternalName: "Cadflair Export Svf Command",
                                                                        Classification: CommandTypesEnum.kNonShapeEditCmdType,
                                                                        ClientId: Globals.AddInCLSIDString,
                                                                        DescriptionText: "Command to export a model to svf format for use with the online Forge Viewer.",
                                                                        ToolTipText: "Save the active document as Svf format.",
                                                                        StandardIcon: Resources.ExportSvfSmall.ToIPictureDisp(),
                                                                        LargeIcon: Resources.ExportSvfLarge.ToIPictureDisp());


            Authentication.SignInButton = controlDefs.AddButtonDefinition(DisplayName: "Sign In", 
                                                                          InternalName: "Cadflair SignIn Command", 
                                                                          Classification: CommandTypesEnum.kNonShapeEditCmdType, 
                                                                          ClientId: Globals.AddInCLSIDString, 
                                                                          DescriptionText: "Command to sign into the Cadflair platform.", 
                                                                          ToolTipText: "Sign in to Cadflair.",
                                                                          StandardIcon: Resources.SignInSmall.ToIPictureDisp(),
                                                                          LargeIcon: Resources.SignInLarge.ToIPictureDisp());

            Authentication.SignOutButton = controlDefs.AddButtonDefinition(DisplayName: "Sign Out", 
                                                                           InternalName: "Cadflair SignOut Command", 
                                                                           Classification:CommandTypesEnum.kNonShapeEditCmdType, 
                                                                           ClientId: Globals.AddInCLSIDString, 
                                                                           DescriptionText: "Sign out of Cadflair.", 
                                                                           ToolTipText: "Sign out of Cadflair.",
                                                                           StandardIcon: Resources.SignOutSmall.ToIPictureDisp(),
                                                                           LargeIcon: Resources.SignOutLarge.ToIPictureDisp());

            //_addDimensionAttributesButton = controlDefs.AddButtonDefinition("Add Automation\nAttributes", "Add Automation Attributes Command", CommandTypesEnum.kShapeEditCmdType, Globals.AddInCLSIDString, "Add AttributeSets to automate drawing elements.", "Save drawing data to AttributeSets for drawing automation.", PictureDispConverter.ToIPictureDisp(Resources.LockSmall), PictureDispConverter.ToIPictureDisp(Resources.LockLarge));
            //_refreshDimensionsButton = controlDefs.AddButtonDefinition("Refresh\nLinear Dimensions", "Refresh Linear Dimensions Command", CommandTypesEnum.kShapeEditCmdType, Globals.AddInCLSIDString, "Repositions linear dimesions based on their attributes.", "Repositions all inear dimesions that have 'TextPosition' attributes assigned.", PictureDispConverter.ToIPictureDisp(Resources.TopAttributeSmall));

            // add button handlers
            UploadToCadflair.UploadToCadflairButton.OnExecute += UploadToCadflair.UploadToCadflairButton_OnExecute;
            ExportSvf.ExportSvfButton.OnExecute += ExportSvf.ExportSvfButton_OnExecute;
            Authentication.SignInButton.OnExecute += Authentication.SignInButton_OnExecute;
            Authentication.SignOutButton.OnExecute += Authentication.SignOutButton_OnExecute;
            //_addDimensionAttributesButton.OnExecute += DrawingAttributesCommand.AddDimensionAttributesButton_OnExecute;
            //_refreshDimensionsButton.OnExecute += DrawingAttributesCommand.RefreshDimensionsButton_OnExecute;

            // set up azure b2c authentication provider
            Authentication.InitializeAzureB2C();

            if (firstTime)
            {
                AddToUserInterface();
            }
        }


        private void UserInterfaceEvents_OnResetRibbonInterface(NameValueMap Context)
        {
            AddToUserInterface();
        }

        private void AddToUserInterface()
        {
            //setup tabs
            RibbonTab assemblyTab = _userInterfaceManager.Ribbons["Assembly"].RibbonTabs.Add("Cadflair", "Cadflair Assembly Tab", Globals.AddInCLSIDString);
            RibbonTab partTab = _userInterfaceManager.Ribbons["Part"].RibbonTabs.Add("Cadflair", "Cadflair Part Tab", Globals.AddInCLSIDString);
            RibbonTab drawingTab = _userInterfaceManager.Ribbons["Drawing"].RibbonTabs.Add("Cadflair", "Cadflair Drawing Tab", Globals.AddInCLSIDString);

            //setup panels
            RibbonPanel assemblyPanel = assemblyTab.RibbonPanels.Add("Cadflair", "Cadflair Assembly Panel", Globals.AddInCLSIDString);
            RibbonPanel partPanel = partTab.RibbonPanels.Add("Cadflair", "Cadflair Part Panel", Globals.AddInCLSIDString);
            RibbonPanel drawingPanel = drawingTab.RibbonPanels.Add("Cadflair", "Cadflair Drawing Panel", Globals.AddInCLSIDString);

            //add components assembly ribbon 
            assemblyPanel.CommandControls.AddButton(UploadToCadflair.UploadToCadflairButton, true);
            assemblyPanel.CommandControls.AddButton(ExportSvf.ExportSvfButton, true);
            assemblyPanel.CommandControls.AddButton(Authentication.SignInButton, false);
            assemblyPanel.CommandControls.AddButton(Authentication.SignOutButton, false);

            //add components part ribbon 
            partPanel.CommandControls.AddButton(UploadToCadflair.UploadToCadflairButton, true);
            partPanel.CommandControls.AddButton(ExportSvf.ExportSvfButton, true);
            partPanel.CommandControls.AddButton(Authentication.SignInButton, false);
            partPanel.CommandControls.AddButton(Authentication.SignOutButton, false);

            //add components drawing ribbon 
            //drawingPanel.CommandControls.AddButton(_addDimensionAttributesButton, true);
            //drawingPanel.CommandControls.AddButton(_refreshDimensionsButton, true);

        }

        /// <summary>
        /// This method is called by Inventor when the AddIn is unloaded.
        /// The AddIn will be unloaded either manually by the user or
        /// when the Inventor session is terminated
        /// </summary>
        public void Deactivate()
        {

            // clear trace listener for logging errors
            Trace.Listeners.Clear();
            Trace.AutoFlush = false;

            // disconnect events
            _userInterfaceManager.UserInterfaceEvents.OnResetRibbonInterface -= UserInterfaceEvents_OnResetRibbonInterface;
            UploadToCadflair.UploadToCadflairButton.OnExecute -= UploadToCadflair.UploadToCadflairButton_OnExecute;
            ExportSvf.ExportSvfButton.OnExecute -= ExportSvf.ExportSvfButton_OnExecute;
            Authentication.SignInButton.OnExecute -= Authentication.SignInButton_OnExecute;
            Authentication.SignOutButton.OnExecute -= Authentication.SignOutButton_OnExecute;
            //_addDimensionAttributesButton.OnExecute -= DrawingAttributesCommand.AddDimensionAttributesButton_OnExecute;
            //_refreshDimensionsButton.OnExecute -= DrawingAttributesCommand.RefreshDimensionsButton_OnExecute;

            // Release objects.
            Globals.InventorApplication = null;
            _userInterfaceManager = null;

            //buttons 
            UploadToCadflair.UploadToCadflairButton = null;
            Authentication.SignInButton = null;
            Authentication.SignOutButton = null;
            //_addDimensionAttributesButton = null;
            //_refreshDimensionsButton = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        /// <summary>
        /// This property is provided to allow the AddIn to expose an API 
        /// of its own to other programs. Typically, this  would be done by
        /// implementing the AddIn's API interface in a class and returning 
        /// that class object through this property.
        /// </summary>
        public object Automation
        {
            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

    }
}
