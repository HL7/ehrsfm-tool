using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MAX_EA_Extension
{
    public class Main
    {
        QuickAccessControl view_ctrl;

        //Called Before EA starts to check Add-In Exists
        public String EA_Connect(EA.Repository Repository)
        {
            //No special processing required.
            /* return "MDG" to receive extra MDG Events
             */
            return "";
        }

        public void EA_Disconnect(EA.Repository Repository)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        //Called when user Click Add-Ins Menu item from within EA.
        //Populates the Menu with our desired selections.
        public object EA_GetMenuItems(EA.Repository Repository, string Location, string MenuName)
        {
            switch (MenuName)
            {
                case "":
                    return "-&MAX";
                case "-&MAX":
                    string[] ar = { "Import/Update", "Export", "-", "Transform", "Validate", "Merge Diagrams", "-", "About..." };
                    return ar;
            }
            return "";
        }

        //Sets the state of the menu depending if there is an active project or not
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models; // triggers an Exception if no project is open
                return true;
            }
            catch
            {
                return false;
            }
        }

        //Called once Menu has been opened to see what menu items are active.
        public void EA_GetMenuState(EA.Repository Repository, string Location, string MenuName, string ItemName, ref bool IsEnabled, ref bool IsChecked)
        {
            if (IsProjectOpen(Repository))
            {
                IsEnabled = true;
            }
            else
            {
                if (ItemName == "About...")
                {
                    IsEnabled = true;
                }
                // If no open project, disable all other menu options
                else
                {
                    IsEnabled = false;
                }
            }
        }

        //Called when user makes a selection in the menu.
        //This is your main exit point to the rest of your Add-in
        public void EA_MenuClick(EA.Repository Repository, string Location, string MenuName, string ItemName)
        {
            try
            {
                EA.Package selectedPackage = Repository.GetTreeSelectedPackage();
                switch (ItemName)
                {
                    case "Import/Update":
                        Repository.CreateOutputTab(MAX_TABNAME);
                        Repository.ClearOutput(MAX_TABNAME);
                        if (new MAX_EA.MAXImporter3().import(Repository, selectedPackage))
                        {
                            // only popup when there were any issues
                            Repository.EnsureOutputVisible(MAX_TABNAME);
                        }
                        break;
                    case "Export":
                        Repository.CreateOutputTab(MAX_TABNAME);
                        Repository.ClearOutput(MAX_TABNAME);
                        if (new MAX_EA.MAXExporter3().export(Repository))
                        {
                            // only popup when there were any issues
                            Repository.EnsureOutputVisible(MAX_TABNAME);
                        }
                        break;
                    case "Transform":
                        Repository.CreateOutputTab(MAX_TABNAME);
                        Repository.ClearOutput(MAX_TABNAME);
                        if (new TransformParamsForm().Show(Repository))
                        {
                            // only popup when there were any issues
                            Repository.EnsureOutputVisible(MAX_TABNAME);
                        }
                        break;
                    case "Validate":
                        Repository.CreateOutputTab(MAX_TABNAME);
                        Repository.ClearOutput(MAX_TABNAME);
                        if (new ValidateParamsForm().Show(Repository))
                        {
                            // only popup when there were any issues
                            Repository.EnsureOutputVisible(MAX_TABNAME);
                        }
                        break;
                    case "Merge Diagrams":
                        mergeDiagrams(Repository, selectedPackage);
                        break;
                    case "Quick Access Tab":
                        if (view_ctrl == null || !view_ctrl.Visible)
                        {
                            if (view_ctrl != null)
                            {
                                // if control removed dispose old instance
                                view_ctrl.Dispose();
                            }
                            view_ctrl = (QuickAccessControl)Repository.AddTab(MAX_TABNAME, "MAX_EA_Extension.QuickAccessControl");
                            view_ctrl.SetRepository(Repository);
                            Repository.ActivateTab(MAX_TABNAME);
                        }
                        break;
                    case "About...":
                        AboutBox about = new AboutBox();
                        about.ShowDialog();
                        break;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void mergeDiagrams(EA.Repository repository, EA.Package package)
        {
            Dictionary<int, EA.DiagramObject> dobjects = new Dictionary<int, EA.DiagramObject>();
            Dictionary<int, EA.DiagramLink> dlinks = new Dictionary<int, EA.DiagramLink>();
            foreach (EA.Diagram diagram in package.Diagrams)
            {
                foreach (EA.DiagramObject dobject in diagram.DiagramObjects)
                {
                    dobjects[dobject.ElementID] = dobject;
                }
                foreach (EA.DiagramLink dlink in diagram.DiagramLinks)
                {
                    dlinks[dlink.ConnectorID] = dlink;
                }
            }

            EA.Diagram mdiagram = (EA.Diagram)package.Diagrams.AddNew(string.Format("{0} (Merged)", package.Name), "Custom");
            mdiagram.Update();

            // copy objects
            int left = 10;
            int top = 10;
            const int width = 100;
            const int height = 80;
            const int padding = 10;
            foreach (EA.DiagramObject dobject in dobjects.Values)
            {
                EA.Element element = repository.GetElementByID(dobject.ElementID);
                if ("Boundary".Equals(element.Type))
                {
                    continue;
                }

                string dimensions = string.Format("l={0};r={1};t={2};b={3};", left, left + width, top, top + height);
                left += padding + width;
                EA.DiagramObject mobject = (EA.DiagramObject)mdiagram.DiagramObjects.AddNew(dimensions, "");
                mobject.ElementID = element.ElementID;
                mobject.Update();

                if (left == 1000)
                {
                    left = 10;
                    top += height;
                }
            }
            mdiagram.Update();

            // copy links (o.a. NoteLinks)
            foreach (EA.DiagramLink dlink in dlinks.Values)
            {
                EA.DiagramLink mlink = (EA.DiagramLink)mdiagram.DiagramLinks.AddNew("", "");
                mlink.ConnectorID = dlink.ConnectorID;
                mlink.Update();
            }

            package.Update();
            repository.OpenDiagram(mdiagram.DiagramID);
        }

        public bool EA_OnContextItemChanged(EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            if (ot == EA.ObjectType.otPackage)
            {
                if (view_ctrl != null)
                {
                    view_ctrl.SetSelectedPackage(Repository.GetTreeSelectedPackage());
                }
            }
            else
            {
                if (view_ctrl != null)
                {
                    view_ctrl.SetSelectedPackage(null);
                }
            }
            return false;
        }

        public static string getAppDataFullPath(string filename)
        {
            string filepath;
            // Check if in developer mode
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Devel path
                filepath = string.Format(@"C:\Eclipse Workspace\ehrsfm_profile\MAX_EA_Extension.ClassLibrary\{0}", filename);
            }
            else
            {
                filepath = string.Format(@"{0}\HL7\MAX_EA_Extension\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), filename);
            }
            return filepath;
        }

        public const string MAX_TABNAME = "MAX";
        public static void LogMessage(EA.Repository repository, string message, int id = 0)
        {
            string timestamp = DateTime.Now.ToString();
            repository.WriteOutput(MAX_TABNAME, string.Format("@{0} {1}", timestamp, message), id);
        }
    }
}
