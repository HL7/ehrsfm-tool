using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using HL7_FM_EA_Extension.R2ModelV2.Base;

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
            EAHelper.repository = Repository;
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
                    return "-&HL7 FM";
                case "-&HL7 FM":
                    string[] ar = { "Import R1.1", "Import R2", "Update Style", "Validate", "-", "Edit Profile", "Compile Profile", "Generate Publication", "Merge Profiles", "-", "Create Diagram", "Quick Access Tab", "FM Browser Tab", "About" };
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
                switch (ItemName)
                {
                    case "Import R1.1":
                    case "Import R2":
                    case "Update Style":
                    case "Validate":
                    case "Edit Profile":
                    case "Compile Profile":
                    case "Merge Profiles":
                        IsEnabled = (Repository.GetTreeSelectedItemType() == EA.ObjectType.otPackage);
                        break;
                    case "Create Diagram":
                        IsEnabled = (Repository.GetTreeSelectedItemType() == EA.ObjectType.otPackage || Repository.GetTreeSelectedItemType() == EA.ObjectType.otElement);
                        break;
                    case "Quick Access Tab":
                    case "FM Browser Tab":
                    case "About":
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
                    case "Edit Profile":
                        new ProfilingForm().Show(Repository, SelectedPackage);
                        break;
                    case "Compile Profile":
                        CompileProfile(Repository, SelectedPackage);
                        break;
                    case "Merge Profiles":
                        new MergeProfilesForm().PopulateAndShow();
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
                EAHelper.LogMessage(e.ToString());
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
        public bool EA_OnContextItemDoubleClicked(EA.Repository repository, string GUID, EA.ObjectType ot)
        {
            if (ot == EA.ObjectType.otElement)
            {
                EA.Element element = repository.GetElementByGuid(GUID);
                R2ModelElement modelElement = R2ModelV2.EA_API.Factory.Create(repository, element);
                if (modelElement != null)
                {
                    switch (modelElement.Stereotype)
                    {
                        case R2Const.ST_HEADER:
                        case R2Const.ST_FUNCTION:
                            new FunctionForm().Show((R2Function)modelElement);
                            return true;
                        case R2Const.ST_CRITERION:
                            new CriterionForm().Show((R2Criterion)modelElement);
                            return true;
                    }
                }
            }
            else if (ot == EA.ObjectType.otPackage)
            {
                EA.Element element = repository.GetPackageByGuid(GUID).Element;
                switch (element.Stereotype)
                {
                    case R2Const.ST_FM_PROFILE:
                    case R2Const.ST_FM_PROFILEDEFINITION:
                        new ProfileMetadataForm().Show(repository, repository.GetPackageByGuid(GUID));
                        return true;
                    case R2Const.ST_SECTION:
                        new SectionForm().Show((R2ModelV2.Base.R2Section)R2ModelV2.EA_API.Factory.Create(repository, element));
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
                EA.Element el = Repository.GetElementByGuid(GUID);
                switch (el.Stereotype)
                {
                    case R2Const.ST_FUNCTION:
                    case R2Const.ST_HEADER:
                    case R2Const.ST_CRITERION:
                    case R2Const.ST_COMPILERINSTRUCTION:
                        R2Config.config.updateStyle(el);
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
                        R2Config.config.updateStyle(el);
                        el.Update();
                        break;
                }
            }
        }

        #region create-diagram
        private void CreateDiagram(EA.Repository Repository)
        {
            EA.Element selectedElement = null;
            switch (Repository.GetContextItemType())
            {
                case EA.ObjectType.otPackage:
                    selectedElement = ((EA.Package)Repository.GetContextObject()).Element;
                    break;
                case EA.ObjectType.otElement:
                    selectedElement = (EA.Element)Repository.GetContextObject();
                    break;
                default:
                    return;
            }
            EA.Diagram diagram = CreateDiagramFromElement(selectedElement);
            Repository.RefreshModelView(Repository.GetTreeSelectedPackage().PackageID);
            Repository.ShowInProjectView(diagram);
            Repository.OpenDiagram(diagram.DiagramID);
        }

        private EA.Diagram CreateDiagramFromElement(EA.Element element)
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
            diagram.Update();
            return diagram;
        }
        #endregion

        #region compile-profile
        /**
         * Compile Profile will take a profile definition and create a profile based on that.
         * 1. Export profile definitions to MAX file (filename is in MAX::ExportFile) of ProfileDefinition Package
         * 2. Find Base Model MAX File (filename is in MAX::ImportFile) of <use> Dependency target from ProfileDefinition Package
         * 3. Compile profile to MAX File (filename is in MAX::ExportFile) of <create> Dependency target from ProfileDefinition Package
         * 4. (optionally) import the compiled profile
         */
        private void CompileProfile(EA.Repository repository, EA.Package package)
        {
            // Only on a "HL7-Profile-Definition" stereotypes package
            if (R2Const.ST_FM_PROFILEDEFINITION.Equals(package.StereotypeEx))
            {
                repository.CreateOutputTab(Properties.Resources.OUTPUT_TAB_HL7_FM);
                repository.ClearOutput(Properties.Resources.OUTPUT_TAB_HL7_FM);
                repository.EnsureOutputVisible(Properties.Resources.OUTPUT_TAB_HL7_FM);

                // !! Use tagged value "MAX:ExportFile" as fileNames!
                // Export it to MAX. 
                EA.TaggedValue tvExportFile = (EA.TaggedValue)package.Element.TaggedValues.GetByName("MAX::ExportFile");
                if (tvExportFile == null)
                {
                    EAHelper.LogMessage(string.Format("MAX::ExportFile tagged value missing in Package \"{0}\"", package.Name));
                    MessageBox.Show("First setup Profile Definition Package.");
                    return;
                }
                string profileDefinitionFileName = tvExportFile.Value;
                EAHelper.LogMessage(string.Format("[INFO] Profile Definition MAX file: {0}", profileDefinitionFileName));
                EAHelper.LogMessage("[BEGIN] Export Profile Definition to MAX file");
                new MAX_EA.MAXExporter3().exportPackage(repository, package, profileDefinitionFileName);
                EAHelper.LogMessage("[END] Export Profile Definition to MAX file");

                // Find associated Base
                EA.Package baseModelPackage = ProfileMetadataForm.getAssociatedBaseModel(repository, package);
                if (baseModelPackage == null)
                {
                    MessageBox.Show("First setup Profile Definition Package.");
                    return;
                }
                EAHelper.LogMessage(string.Format("[INFO] Base Model Package name: {0}", baseModelPackage.Name));
                EA.TaggedValue tvImportFile = (EA.TaggedValue)baseModelPackage.Element.TaggedValues.GetByName("MAX::ImportFile");
                if (tvImportFile == null)
                {
                    MessageBox.Show("First setup Profile Definition Package.");
                    return;
                }
                string baseModelFileName = tvImportFile.Value;
                EAHelper.LogMessage(string.Format("[INFO] Base Model MAX file: {0}", baseModelFileName));

                // Find associated Target Profile Package
                EA.Package compiledProfilePackage = ProfileMetadataForm.getAssociatedOutputProfile(repository, package);
                if (compiledProfilePackage == null)
                {
                    MessageBox.Show("First setup Profile Definition Package.");
                    return;
                }
                EAHelper.LogMessage(string.Format("[INFO] Compiled Profile Package name: {0}", compiledProfilePackage.Name));
                EA.TaggedValue tvExportFile2 = (EA.TaggedValue)compiledProfilePackage.Element.TaggedValues.GetByName("MAX::ExportFile");
                if (tvExportFile2 == null)
                {
                    MessageBox.Show("First setup Profile Definition Package.");
                    return;
                }
                string profileFileName = tvExportFile2.Value;

                // Call R2ProfileCompiler
                EAHelper.LogMessage(string.Format("[INFO] Compiled Profile MAX file: {0}", profileFileName));
                EAHelper.LogMessage("[BEGIN] Compile Profile");
                R2ProfileCompiler compiler = new R2ProfileCompiler();
                compiler._OutputListener = new EAHelper();
                compiler.Compile(baseModelFileName, profileDefinitionFileName, profileFileName);
                EAHelper.LogMessage("[END] Compile Profile");

                // Import compiled profile from MAX file
                //new MAX_EA.MAXImporter3().import(repository, profilePackage, profileFileName);
                MessageBox.Show("Manually import compiled profile now...");
            }
            else
            {
                MessageBox.Show("Please select a Profile Definition Package to Compile.");
            }
        }

        private EA.Package findAssociatedCompiledProfile(EA.Repository Repository, EA.Package package)
        {
            EA.Connector con = package.Connectors.Cast<EA.Connector>().SingleOrDefault(t => R2Const.ST_TARGETPROFILE.Equals(t.Stereotype));
            if (con != null)
            {
                EA.Element packageElement = Repository.GetElementByID(con.SupplierID);
                // Check if target profile is HL7-FM-Profile
                if (R2Const.ST_FM_PROFILE.Equals(packageElement.Stereotype))
                {
                    return Repository.GetPackageByID(packageElement.PackageID).Packages.Cast<EA.Package>().Single(p => p.Element.ElementID == con.SupplierID);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region update-style
        /**
         * Update the visual style (colors) of Section/Header/Function/Criteria elements.
         */
        private void UpdateStyle_recurseEaPackage(EA.Package eaPackage)
        {
            foreach (EA.Package eaSubPackage in eaPackage.Packages)
            {
                UpdateStyle_recurseEaPackage(eaSubPackage);
            }
            foreach (EA.Element eaElement in eaPackage.Elements)
            {
                R2Config.config.updateStyle(eaElement);
                eaElement.Update();
                UpdateStyle_recurseEaElements(eaElement);
            }
        }

        private void UpdateStyle_recurseEaElements(EA.Element eaElement)
        {
            foreach (EA.Element eaChildElement in eaElement.Elements)
            {
                R2Config.config.updateStyle(eaChildElement);
                eaChildElement.Update();
                UpdateStyle_recurseEaElements(eaChildElement);
            }
        }
        #endregion

        #region add-in search methods
        public Boolean FindNonSHALL(EA.Repository Repository, string SearchText, out string XMLResults)
        {
            XElement xResults = new SearchMethods().FindNonSHALL(Repository, SearchText);
            XMLResults = xResults.ToString();
            return true;
        }
        #endregion

        // --------------
        // Install MDG
        // --------------
        public string EA_OnInitializeTechnologies(EA.Repository Repository)
        {
            string mdg_xml = "";
            string path = getAppDataFullPath(@"MDG\HL7_FM_EA_mdg.xml");
            if (!File.Exists(path))
            {
                MessageBox.Show(string.Format("MDG File not found. Please report this message.\n{0}", path));
            }
            else
            {
                StreamReader reader = new StreamReader(path);
                mdg_xml = reader.ReadToEnd();
            }
            return mdg_xml;
        }

        public static string getAppDataFullPath(string filename)
        {
            string filepath;
            // Check if in developer mode
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Devel path
                filepath = string.Format(@"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension\{0}", filename);
            }
            else
            {
                filepath = string.Format(@"{0}\HL7\HL7_FM_EA_Extension\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), filename);
            }
            return filepath;
        }
    }
}
