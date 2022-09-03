using CadflairInventorAddin.Properties;
using Inventor;
using System;
using System.Runtime.InteropServices;

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
        private ButtonDefinition _addDimensionAttributesButton;
        private ButtonDefinition _refreshDimensionsButton;


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

            //add user interface manager
            _userInterfaceManager = Globals.InventorApplication.UserInterfaceManager;
            _userInterfaceManager.UserInterfaceEvents.OnResetRibbonInterface += UserInterfaceEvents_OnResetRibbonInterface;

            //setup button definitions
            ControlDefinitions controlDefs = Globals.InventorApplication.CommandManager.ControlDefinitions;
            _addDimensionAttributesButton = controlDefs.AddButtonDefinition("Add Automation\nAttributes", "Add Automation Attributes Command", CommandTypesEnum.kShapeEditCmdType, Globals.AddInCLSIDString, "Add AttributeSets to automate drawing elements.", "Save drawing data to AttributeSets for drawing automation.", PictureDispConverter.ToIPictureDisp(Resources.LockSmall), PictureDispConverter.ToIPictureDisp(Resources.LockLarge));
            _refreshDimensionsButton = controlDefs.AddButtonDefinition("Refresh\nLinear Dimensions", "Refresh Linear Dimensions Command", CommandTypesEnum.kShapeEditCmdType, Globals.AddInCLSIDString, "Repositions linear dimesions based on their attributes.", "Repositions all inear dimesions that have 'TextPosition' attributes assigned.", PictureDispConverter.ToIPictureDisp(Resources.TopAttributeSmall));

            //add button handlers
            _addDimensionAttributesButton.OnExecute += AttributesCommand.AddDimensionAttributesButton_OnExecute;
            _refreshDimensionsButton.OnExecute += AttributesCommand.RefreshDimensionsButton_OnExecute;


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
            //setup ribbons and panels
            Ribbons ribbons = _userInterfaceManager.Ribbons;
            Ribbon drawingRibbon = ribbons["Drawing"];
            RibbonTab drawingTab = drawingRibbon.RibbonTabs.Add("Cadflair", "Cadflair Drawing Tab", Globals.AddInCLSIDString);
            RibbonPanel linearDimsPanel = drawingTab.RibbonPanels.Add("Dimensions", "Cadflair Drawing Panel", Globals.AddInCLSIDString);
        

            //add components to user interface
            linearDimsPanel.CommandControls.AddButton(_addDimensionAttributesButton, true);
            linearDimsPanel.CommandControls.AddButton(_refreshDimensionsButton, true);

        }

        public void Deactivate()
        {
            // This method is called by Inventor when the AddIn is unloaded.
            // The AddIn will be unloaded either manually by the user or
            // when the Inventor session is terminated

            // disconnected events
            _userInterfaceManager.UserInterfaceEvents.OnResetRibbonInterface -= UserInterfaceEvents_OnResetRibbonInterface;
            _addDimensionAttributesButton.OnExecute -= AttributesCommand.AddDimensionAttributesButton_OnExecute;
            _refreshDimensionsButton.OnExecute -= AttributesCommand.RefreshDimensionsButton_OnExecute;

            // Release objects.
            Globals.InventorApplication = null;
            _userInterfaceManager = null;


            //buttons 
            _addDimensionAttributesButton = null;
            _refreshDimensionsButton = null;


            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void ExecuteCommand(int commandID)
        {
            // Note:this method is now obsolete, you should use the 
            // ControlDefinition functionality for implementing commands.
        }

        public object Automation
        {
            // This property is provided to allow the AddIn to expose an API 
            // of its own to other programs. Typically, this  would be done by
            // implementing the AddIn's API interface in a class and returning 
            // that class object through this property.

            get
            {
                // TODO: Add ApplicationAddInServer.Automation getter implementation
                return null;
            }
        }

        #endregion

    }
}
