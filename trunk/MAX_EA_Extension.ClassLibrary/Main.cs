using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using MAX_EA;

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
            Filters.CurrentOutputPath = Path.GetTempPath();
            return "";
        }

        public void EA_Disconnect(EA.Repository Repository)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        // --------------
        // Install MDG
        // Specifically the Publish/MAX item!
        // http://www.sparxsystems.com/enterprise_architect_user_guide/9.3/standard_uml_models/add_import_export_scripts.html
        // --------------
        public string EA_OnInitializeTechnologies(EA.Repository Repository)
        {
            string mdg_xml = "";
            string path = getAppDataFullPath(@"MDG\max-mdg.xml");
            if (!File.Exists(path))
            {
                //MessageBox.Show(string.Format("MDG File not found. Please report this message.\n{0}", path));
            }
            else
            {
                StreamReader reader = new StreamReader(path);
                mdg_xml = reader.ReadToEnd();
            }
            return mdg_xml;
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
                    // 1) MAX "native" Functions, 2) EA Helper Functions
                    string[] ar = { "Import", "Export", "Transform", "Filters", "Validate", "-", "Lock", "Unlock", "Merge Diagrams", "Batch", "-", "Quick Access Tab", "About..." };
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
                switch (ItemName)
                {
                    case "Lock":
                    case "Unlock":
                    case "Import":
                    case "Merge Diagrams":
                        IsEnabled = (Repository.GetTreeSelectedItemType() == EA.ObjectType.otPackage);
                        break;
                    case "Transform":
                    case "Export":
                        IsEnabled = (Repository.GetContextItemType() == EA.ObjectType.otPackage || Repository.GetContextItemType() == EA.ObjectType.otDiagram);
                        break;
                    case "Validate":
                        IsEnabled = (Repository.GetTreeSelectedItemType() == EA.ObjectType.otPackage || Repository.GetTreeSelectedItemType() == EA.ObjectType.otDiagram);
                        break;
                    case "Batch":
                    case "Quick Access Tab":
                    case "Filters":
                    case "About...":
                        IsEnabled = true;
                        break;
                    default:
                        IsEnabled = false;
                        break;
                }
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
                    case "Import":
                        Repository.CreateOutputTab(MAX_TABNAME);
                        Repository.ClearOutput(MAX_TABNAME);
                        if (new Filters().import(Repository, selectedPackage))
                        {
                            // only popup when there were any issues
                            Repository.EnsureOutputVisible(MAX_TABNAME);
                        }
                        break;
                    case "Export":
                        Repository.CreateOutputTab(MAX_TABNAME);
                        Repository.ClearOutput(MAX_TABNAME);
                        if (new Filters().export(Repository))
                        {
                            // only popup when there were any issues
                            Repository.EnsureOutputVisible(MAX_TABNAME);
                        }
                        break;
                    case "Filters":
                        // TODO: create Config Filters Dialogs
                        MessageBox.Show("NYI");
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
                    case "Lock":
                        setLocked(selectedPackage, true);
                        break;
                    case "Unlock":
                        setLocked(selectedPackage, false);
                        break;
                    case "Batch":
                        batch(Repository);
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

        private void setLocked(EA.Package package, bool locked)
        {
            package.Element.Locked = locked;
            foreach (EA.Package subPackage in package.Packages)
            {
                setLocked(subPackage, locked);
            }
            foreach (EA.Element element in package.Elements)
            {
                setLocked(element, locked);
            }
        }

        private void setLocked(EA.Element element, bool locked)
        {
            element.Locked = locked;
            foreach (EA.Element subElement in element.Elements)
            {
                setLocked(subElement, locked);
            }
        }

        private void batch(EA.Repository Repository)
        {
            Dictionary<string, EA.Package> content = new Dictionary<string, EA.Package>();
            string xml = Repository.SQLQuery("SELECT Name, ea_guid FROM t_object WHERE Object_Type = 'Package'"); // order by package_id
            XElement xEADATA = XElement.Parse(xml, LoadOptions.None);
            IEnumerable<XElement> xRows = xEADATA.XPathSelectElements("//Data/Row");
            foreach (XElement xRow in xRows)
            {
                string ea_guid = xRow.Element("ea_guid").Value;
                string name = xRow.Element("Name").Value;
                content[name] = Repository.GetPackageByGuid(ea_guid);
            }
            BatchForm form = new BatchForm();
            form.setContent(content);
            form.ShowDialog();

            if (form.isExportButtonPressed())
            {
                Repository.CreateOutputTab(MAX_TABNAME);
                Repository.ClearOutput(MAX_TABNAME);
                bool issues = false;
                foreach (string name in form.getSelectedItems())
                {
                    EA.Package package = content[name];
                    issues |= new Filters().exportPackage(Repository, package);
                }
                if (issues)
                {
                    // only popup when there were any issues
                    Repository.EnsureOutputVisible(MAX_TABNAME);
                }
            }
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
            // Devel path
            filepath = string.Format(@"C:\Eclipse Workspace\ehrsfm_profile\MAX_EA_Extension.ClassLibrary\{0}", filename);
            if (!File.Exists(filepath))
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
