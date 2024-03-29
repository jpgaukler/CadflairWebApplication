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

            DrawingAttributesCommand.AddDimensionAttributesButton = controlDefs.AddButtonDefinition(DisplayName: "Add Automation\nAttributes", 
                                                                                                   InternalName: "Add Automation Attributes Command", 
                                                                                                   Classification:CommandTypesEnum.kShapeEditCmdType, 
                                                                                                   ClientId: Globals.AddInCLSIDString, 
                                                                                                   DescriptionText: "Add AttributeSets to automate drawing elements.", 
                                                                                                   ToolTipText: "Save drawing data to AttributeSets for drawing automation.",
                                                                                                   StandardIcon: Resources.LockSmall.ToIPictureDisp(),
                                                                                                   LargeIcon: Resources.LockLarge.ToIPictureDisp());

            DrawingAttributesCommand.RefreshDimensionsButton = controlDefs.AddButtonDefinition(DisplayName: "Refresh\nLinear Dimensions", 
                                                                                               InternalName: "Refresh Linear Dimensions Command", 
                                                                                               Classification:CommandTypesEnum.kShapeEditCmdType, 
                                                                                               ClientId: Globals.AddInCLSIDString, 
                                                                                               DescriptionText: "Repositions linear dimesions based on their attributes.", 
                                                                                               ToolTipText: "Repositions all inear dimesions that have 'TextPosition' attributes assigned.",
                                                                                               StandardIcon: Resources.LockSmall.ToIPictureDisp(),
                                                                                               LargeIcon: Resources.LockLarge.ToIPictureDisp());

            iPartExport.ExportiPartsStpsButton= controlDefs.AddButtonDefinition(DisplayName: "iPart - Export Stps", 
                                                                               InternalName: "iPart - Export Stps", 
                                                                               Classification:CommandTypesEnum.kShapeEditCmdType, 
                                                                               ClientId: Globals.AddInCLSIDString, 
                                                                               DescriptionText: "Export stp file for all iPart configurations.", 
                                                                               ToolTipText: "Export stp file for all iPart configurations.",
                                                                               StandardIcon: Resources.ExportSvfSmall.ToIPictureDisp(),
                                                                               LargeIcon: Resources.ExportSvfLarge.ToIPictureDisp());

            iPartExport.ExportiPartsPdfsButton= controlDefs.AddButtonDefinition(DisplayName: "iPart - Export Pdfs", 
                                                                               InternalName: "iPart - Export Pdfs", 
                                                                               Classification:CommandTypesEnum.kShapeEditCmdType, 
                                                                               ClientId: Globals.AddInCLSIDString, 
                                                                               DescriptionText: "Export drawing pdf for all iPart configurations.", 
                                                                               ToolTipText: "Export drawing pdf for all iPart configurations.",
                                                                               StandardIcon: Resources.ExportSvfSmall.ToIPictureDisp(),
                                                                               LargeIcon: Resources.ExportSvfLarge.ToIPictureDisp());

            ExportThumbnail.ExportThumbnailButton = controlDefs.AddButtonDefinition(DisplayName: "Export Thumbnail", 
                                                                               InternalName: "Export Thumbnail", 
                                                                               Classification:CommandTypesEnum.kNonShapeEditCmdType, 
                                                                               ClientId: Globals.AddInCLSIDString, 
                                                                               DescriptionText: "Export a thumbnail image for the active document.", 
                                                                               ToolTipText: "Export a thumbnail image for the active document.", 
                                                                               StandardIcon: Resources.ExportSvfSmall.ToIPictureDisp(),
                                                                               LargeIcon: Resources.ExportSvfLarge.ToIPictureDisp());

            // add button handlers
            UploadToCadflair.UploadToCadflairButton.OnExecute += UploadToCadflair.UploadToCadflairButton_OnExecute;
            ExportSvf.ExportSvfButton.OnExecute += ExportSvf.ExportSvfButton_OnExecute;
            Authentication.SignInButton.OnExecute += Authentication.SignInButton_OnExecute;
            Authentication.SignOutButton.OnExecute += Authentication.SignOutButton_OnExecute;
            DrawingAttributesCommand.AddDimensionAttributesButton.OnExecute += DrawingAttributesCommand.AddDimensionAttributesButton_OnExecute;
            DrawingAttributesCommand.RefreshDimensionsButton.OnExecute += DrawingAttributesCommand.RefreshDimensionsButton_OnExecute;
            iPartExport.ExportiPartsStpsButton.OnExecute += iPartExport.ExportiPartStpsButton_OnExecute;
            iPartExport.ExportiPartsPdfsButton.OnExecute += iPartExport.ExportiPartPdfsButton_OnExecute;
            ExportThumbnail.ExportThumbnailButton.OnExecute += ExportThumbnail.ExportThumbnailButton_OnExecute;

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
            //assemblyPanel.CommandControls.AddButton(ExportSvf.ExportSvfButton, true);
            assemblyPanel.CommandControls.AddButton(Authentication.SignInButton, false);
            assemblyPanel.CommandControls.AddButton(Authentication.SignOutButton, false);
            assemblyPanel.CommandControls.AddButton(ExportThumbnail.ExportThumbnailButton, false);

            //add components part ribbon 
            partPanel.CommandControls.AddButton(UploadToCadflair.UploadToCadflairButton, true);
            //partPanel.CommandControls.AddButton(ExportSvf.ExportSvfButton, true);
            partPanel.CommandControls.AddButton(Authentication.SignInButton, false);
            partPanel.CommandControls.AddButton(Authentication.SignOutButton, false);
            partPanel.CommandControls.AddButton(iPartExport.ExportiPartsStpsButton, false);
            partPanel.CommandControls.AddButton(ExportThumbnail.ExportThumbnailButton, false);

            //add components drawing ribbon 
            drawingPanel.CommandControls.AddButton(DrawingAttributesCommand.AddDimensionAttributesButton, true);
            drawingPanel.CommandControls.AddButton(DrawingAttributesCommand.RefreshDimensionsButton, true);
            drawingPanel.CommandControls.AddButton(iPartExport.ExportiPartsPdfsButton, true);

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
            DrawingAttributesCommand.AddDimensionAttributesButton.OnExecute -= DrawingAttributesCommand.AddDimensionAttributesButton_OnExecute;
            DrawingAttributesCommand.RefreshDimensionsButton.OnExecute -= DrawingAttributesCommand.RefreshDimensionsButton_OnExecute;
            iPartExport.ExportiPartsStpsButton.OnExecute -= iPartExport.ExportiPartStpsButton_OnExecute;
            iPartExport.ExportiPartsPdfsButton.OnExecute -= iPartExport.ExportiPartPdfsButton_OnExecute;
            ExportThumbnail.ExportThumbnailButton.OnExecute -= ExportThumbnail.ExportThumbnailButton_OnExecute;

            // Release objects.
            Globals.InventorApplication = null;
            _userInterfaceManager = null;

            //buttons 
            UploadToCadflair.UploadToCadflairButton = null;
            Authentication.SignInButton = null;
            Authentication.SignOutButton = null;
            DrawingAttributesCommand.AddDimensionAttributesButton = null;
            DrawingAttributesCommand.RefreshDimensionsButton = null;
            iPartExport.ExportiPartsStpsButton = null;
            iPartExport.ExportiPartsPdfsButton = null;
            ExportThumbnail.ExportThumbnailButton = null;

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
