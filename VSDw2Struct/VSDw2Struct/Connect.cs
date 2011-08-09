using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;
using EnvDTE80;
using Extensibility;

namespace Dw2Struct
{
    public class Connect : IDTExtensibility2, IDTCommandTarget
    {
        private DTE2 application;
        private AddIn addInInstance;
        private PBHelper pb;

        public Connect()
        {
        }

        public void Exec(string CmdName, vsCommandExecOption ExecuteOption, ref object VariantIn, ref object VariantOut, ref bool Handled)
        {
            // if the menu item "Generate Structure" is called
            if(CmdName == "Dw2Struct.Connect.Dw2Struct")
            {
                if(application.SelectedItems.Count == 0) return;

                if (pb != null)
                {
                    pb.GenerateStructure(application.SelectedItems.Item(1));
                }
            }
            // if the menu item "Generate NonVisualObject" is called
            else if (CmdName == "Dw2Struct.Connect.Dw2Nv")
            {
                if (application.SelectedItems.Count == 0) return;

                if (pb != null)
                {
                    pb.GenerateNonVisualObject(application.SelectedItems.Item(1));
                }
            }
                
        }

        public void QueryStatus(string CmdName, vsCommandStatusTextWanted NeededText, ref vsCommandStatus StatusOption, ref object CommandText)
        {
            if (NeededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (CmdName == "Dw2Struct.Connect.Dw2Struct" || CmdName == "Dw2Struct.Connect.Dw2Nv")
                {
                    if (application.SelectedItems.Count == 0)
                    {
                        StatusOption = vsCommandStatus.vsCommandStatusInvisible;
                        return;
                    }

                    // it's only for datawindows
                    if (pb.IsDatawindow(application.SelectedItems.Item(1)))
                    {
                        StatusOption = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled;
                        return;
                    }
                    else
                    {
                        StatusOption = vsCommandStatus.vsCommandStatusInvisible;
                        return;
                    }
                }

            }
        }

        public void OnConnection(object Application, ext_ConnectMode ConnectMode, object AddInInst, ref Array custom)
        {
            pb = new PBHelper();
            application = (DTE2)Application;
            addInInstance = (AddIn)AddInInst;
            if (ConnectMode == ext_ConnectMode.ext_cm_UISetup)
            {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)application.Commands;
                
                // get item bar
                Microsoft.VisualStudio.CommandBars.CommandBar itemCommandBar = ((Microsoft.VisualStudio.CommandBars.CommandBars)application.CommandBars)["Item"];

                try
                {
                    // add me commands
                    Command command = commands.AddNamedCommand2(addInInstance, "Dw2Struct", "Generate Structure", "Generates a structure of the columns in the datawindow", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                    Command command2 = commands.AddNamedCommand2(addInInstance, "Dw2Nv", "Generate NonVisualObject", "Generates an NonVisualObject of the columns in the datawindow", true, 59, ref contextGUIDS, (int)vsCommandStatus.vsCommandStatusSupported + (int)vsCommandStatus.vsCommandStatusEnabled, (int)vsCommandStyle.vsCommandStylePictAndText, vsCommandControlType.vsCommandControlTypeButton);
                                        
                    if ((command != null))
                    {
                        command.AddControl(itemCommandBar, itemCommandBar.Controls.Count);                        
                    }

                    if ((command2 != null))
                    {
                        command2.AddControl(itemCommandBar, itemCommandBar.Controls.Count);
                    }
                }
                catch (System.ArgumentException)
                {
                }
            }
        }

        #region some implementations it didn't need

        public void OnAddInsUpdate(ref Array custom)
        {

        }

        public void OnBeginShutdown(ref Array custom)
        {

        }

        public void OnDisconnection(ext_DisconnectMode RemoveMode, ref Array custom)
        {
            
        }

        public void OnStartupComplete(ref Array custom)
        {

        }

        #endregion some implementations it didn't need
    }
}
