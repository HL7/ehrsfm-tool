using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HL7_FM_EA_Extension
{
    public class Main
    {
        QuickAccessControl view_ctrl;
        ModelBrowserControl mb_ctrl;

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
            EA.Package aPackage = Repository.GetTreeSelectedPackage();
            switch (MenuName)
            {
                case "":
                    return "-&HL7 FM";
                case "-&HL7 FM":
                    string[] ar = { "Import R1.1", "Import R2", "Validate", "Create Diagram", "Quick Access Tab", "FM Browser Tab", "About" };
                    return ar;
            }
            return "";
        }

        //Sets the state of the menu depending if there is an active project or not
        bool IsProjectOpen(EA.Repository Repository)
        {
            try
            {
                EA.Collection c = Repository.Models;
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
                EA.Package SelectedPackage = Repository.GetTreeSelectedPackage();
                switch (ItemName)
                {
                    case "Import R1.1":
                        new R11Importer().import(Repository, SelectedPackage);
                        break;
                    case "Import R2":
                        new R2Importer().import(Repository, SelectedPackage);
                        break;
                    case "Validate":
                        new R2Validator().validate(Repository, SelectedPackage);
                        break;
                    case "Create Diagram":
                        CreateDiagram(Repository);
                        break;
                    case "Quick Access Tab":
                        if (view_ctrl == null || !view_ctrl.Visible)
                        {
                            if (view_ctrl != null)
                            {
                                // if control removed dispose old instance
                                view_ctrl.Dispose();
                            }
                            view_ctrl = (QuickAccessControl)Repository.AddTab("HL7 FM", "HL7_FM_EA_Extension.QuickAccessControl");
                            view_ctrl.SetRepository(Repository);
                            Repository.ActivateTab("EHR-S FM");
                        }
                        break;
                    case "FM Browser Tab":
                        if (mb_ctrl == null || !mb_ctrl.Visible)
                        {
                            if (mb_ctrl != null)
                            {
                                // if control removed dispose old instance
                                mb_ctrl.Dispose();
                            }
                            mb_ctrl = (ModelBrowserControl)Repository.AddTab("FM Browser", "HL7_FM_EA_Extension.ModelBrowserControl");
                            mb_ctrl.SetRepository(Repository);
                            Repository.ActivateTab("FM Browser");
                        }
                        break;
                    case "About":
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

        public bool EA_OnContextItemChanged(EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            if (view_ctrl != null)
            {
                if (ot == EA.ObjectType.otPackage)
                {
                    view_ctrl.SetSelectedPackage(Repository.GetTreeSelectedPackage());
                }
                else
                {
                    view_ctrl.SetSelectedPackage(null);
                }
            }
            if (mb_ctrl != null)
            {
                if (ot == EA.ObjectType.otPackage)
                {
                    mb_ctrl.SetSelectedPackage(Repository.GetTreeSelectedPackage());
                }
                else
                {
                    mb_ctrl.SetSelectedPackage(null);
                }
            }
            return false;
        }

        //This event occurs when a user has double-clicked (or pressed [Enter]) 
        //on the item in context, either in a diagram or in the Project Browser.
        private R2Config config = new R2Config();
        public bool EA_OnContextItemDoubleClicked(EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            if (ot == EA.ObjectType.otElement)
            {
                EA.Element el = Repository.GetElementByGuid(GUID);
                switch (el.Stereotype)
                {
                    case "Function":
                    case "Header":
                        new FunctionForm().Show(el, config);
                        return true;
                    case "Criteria":
                        new CriteriaForm().Show(el, config);
                        return true;
                }
            }
            else if (ot == EA.ObjectType.otPackage)
            {
                EA.Element el = Repository.GetPackageByGuid(GUID).Element;
                switch (el.Stereotype)
                {
                    case "Section":
                        new SectionForm().Show(el, config);
                        return true;
                }
            }
            return false;
        }

        // On double click on output 
        public void EA_OnOutputItemDoubleClicked(EA.Repository Repository, String TabName, String LineText, int ID)
        {
            if (Properties.Resources.OUTPUT_TAB_HL7_FM.Equals(TabName) && ID != -1)
            {
                Repository.ShowInProjectView(Repository.GetElementByID(ID));
            }
        }

// --------------

        private void CreateDiagram(EA.Repository Repository)
        {
            EA.Element selectedElement = (EA.Element)Repository.GetTreeSelectedObject();
            EA.Diagram diagram = CreateDiagram(selectedElement);
            Repository.RefreshModelView(Repository.GetTreeSelectedPackage().PackageID);
            Repository.ShowInProjectView(diagram);
            Repository.OpenDiagram(diagram.DiagramID);
        }

        private EA.Diagram CreateDiagram(EA.Element element)
        {
            EA.Diagram diagram = (EA.Diagram)element.Diagrams.AddNew(element.Name + " Criteria", "Requirements");
            diagram.Update();

            EA.DiagramObject fobj = (EA.DiagramObject)diagram.DiagramObjects.AddNew(string.Format("l={0};r={1};t={2};b={3}", 10, 630, 10, 110), "");
            fobj.ElementID = element.ElementID;
            fobj.Update();

            int left = 10;
            int top = 210;
            const int width = 200;
            const int height = 100;
            const int padding = 10;

            foreach (EA.Element subElement in element.Elements)
            {
                string dimensions = string.Format("l={0};r={1};t={2};b={3};", left, left + width, top, top + height);
                EA.DiagramObject obj = (EA.DiagramObject)diagram.DiagramObjects.AddNew(dimensions, "");
                obj.ElementID = subElement.ElementID;
                obj.Update();

                left += width + padding;
                if (left > 600)
                {
                    left = 10;
                    top += height + padding;
                }
            }
            diagram.IsLocked = true;
            diagram.Update();
            return diagram;
        }
    }
}
