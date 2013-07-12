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
                    string[] ar = { "Import R1.1", "Import R2", "Validate", "Update Style", "Create Diagram", "Quick Access Tab", "FM Browser Tab", "About" };
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
                    case "Update Style":
                        UpdateStyle_recurseEaPackage(SelectedPackage);
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
                            Repository.ActivateTab("HL7 FM");
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
                    case R2Const.ST_FUNCTION:
                    case R2Const.ST_HEADER:
                        new FunctionForm().Show(el, config);
                        return true;
                    case R2Const.ST_CRITERIA:
                        new CriteriaForm().Show(el, config);
                        return true;
                }
            }
            else if (ot == EA.ObjectType.otPackage)
            {
                EA.Element el = Repository.GetPackageByGuid(GUID).Element;
                switch (el.Stereotype)
                {
                    case R2Const.ST_SECTION:
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

        // On Change of an Item
        public void EA_OnNotifyContextItemModified(EA.Repository Repository, String GUID, EA.ObjectType ot)
        {
            if (ot == EA.ObjectType.otElement)
            {
                R2Config config = new R2Config();
                EA.Element el = Repository.GetElementByGuid(GUID);
                switch (el.Stereotype)
                {
                    case R2Const.ST_FUNCTION:
                    case R2Const.ST_HEADER:
                    case R2Const.ST_CRITERIA:
                        config.updateStyle(el);
                        el.Update();
                        break;
                }
            }
            else if (ot == EA.ObjectType.otPackage)
            {
                EA.Element el = Repository.GetPackageByGuid(GUID).Element;
                switch (el.Stereotype)
                {
                    case R2Const.ST_SECTION:
                        config.updateStyle(el);
                        el.Update();
                        break;
                }
            }
        }

// --------------

        private void CreateDiagram(EA.Repository Repository)
        {
            EA.Element selectedElement = null;
            switch (Repository.GetTreeSelectedItemType())
            {
                case EA.ObjectType.otPackage:
                    selectedElement = Repository.GetTreeSelectedPackage().Element;
                    break;
                case EA.ObjectType.otElement:
                    selectedElement = (EA.Element)Repository.GetTreeSelectedObject();
                    break;
                default:
                    return;
            }
            EA.Diagram diagram = CreateDiagram(selectedElement);
            Repository.RefreshModelView(Repository.GetTreeSelectedPackage().PackageID);
            Repository.ShowInProjectView(diagram);
            Repository.OpenDiagram(diagram.DiagramID);
        }

        private EA.Diagram CreateDiagram(EA.Element element)
        {
            EA.Diagram diagram = (EA.Diagram)element.Diagrams.AddNew(element.Name, "Requirements");
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
            diagram.IsLocked = R2Const.LOCK_ELEMENTS;
            diagram.Update();
            return diagram;
        }

        // --------------

        private void UpdateStyle_recurseEaPackage(EA.Package eaPackage)
        {
            foreach (EA.Package eaSubPackage in eaPackage.Packages)
            {
                UpdateStyle_recurseEaPackage(eaSubPackage);
            }
            foreach (EA.Element eaElement in eaPackage.Elements)
            {
                config.updateStyle(eaElement);
                eaElement.Update();
                UpdateStyle_recurseEaElements(eaElement);
            }
        }

        private void UpdateStyle_recurseEaElements(EA.Element eaElement)
        {
            foreach (EA.Element eaChildElement in eaElement.Elements)
            {
                config.updateStyle(eaChildElement);
                eaChildElement.Update();
                UpdateStyle_recurseEaElements(eaChildElement);
            }
        }

    }
}
