using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows.Forms;

namespace HL7_FM_EA_Extension
{
    class R2Importer
    {
        private EA.Repository Repository;
        private R2Config config = new R2Config();
        private MAX_EA.ProgressWindow progress = new MAX_EA.ProgressWindow();

        private Dictionary<string, EA.Element> elements = new Dictionary<string, EA.Element>();
        private Dictionary<string, Link> compositions = new Dictionary<string, Link>();
        private Dictionary<string, Link> consequenceLinks = new Dictionary<string, Link>();
        private Dictionary<string, Link> seeAlsoLinks = new Dictionary<string, Link>();

        public void import(EA.Repository Repository, EA.Package rootPackage)
        {
            string xmlFileName = showFileDialog("Select EHR-S FM R2 XML File", "xml files (*.xml)|*.xml", @"D:\VisualStudio Projects\HL7\EHRSFM_EA_AddIn\EHRSFM\EHRS_FM_R2_N2_C3_FunctionList_2013MAY.xml", true);
            if (!string.IsNullOrEmpty(xmlFileName))
            {
                import(Repository, rootPackage, xmlFileName);
            }
        }

        public void import(EA.Repository Repository, EA.Package rootPackage, string xmlFileName)
        {
            this.Repository = Repository;
            Repository.EnableUIUpdates = false;
            Repository.BatchAppend = true;

            Repository.CreateOutputTab(Properties.Resources.OUTPUT_TAB_HL7_FM);
            Repository.ClearOutput(Properties.Resources.OUTPUT_TAB_HL7_FM);
            Repository.EnsureOutputVisible(Properties.Resources.OUTPUT_TAB_HL7_FM);

            // Just make sure the lists,dictionaries are empty
            elements.Clear();
            compositions.Clear();
            consequenceLinks.Clear();

            // Read the XML
            XElement xModel = XElement.Load(xmlFileName);

            // setup progress window
            progress.Show();
            int functionCount = int.Parse(xModel.XPathEvaluate("count(//Function)").ToString());
            progress.setup(functionCount);

            // Create the main package
            string fmName = string.Format("{0} (import v{1})", getXElementValue(xModel, "Alias"), Properties.Resources.VERSION_DATE);
            EA.Package fmPackage = (EA.Package)rootPackage.Packages.AddNew(fmName, "Package");
            fmPackage.Version = getXElementValue(xModel, "Version") + " " + getXElementValue(xModel, "Date");
            fmPackage.Notes = xModel.XPathSelectElement("Overview").CreateNavigator().InnerXml.Replace("<br />", "\r\n");
            fmPackage.IsNamespace = true;
            fmPackage.Update();
            fmPackage.Element.Stereotype = R2Const.ST_FM;
            fmPackage.Element.Author = getXElementValue(xModel, "Author");
            fmPackage.Element.Phase = getXElementValue(xModel, "Type");
            fmPackage.Element.Update();
            fmPackage.Element.Locked = R2Const.LOCK_ELEMENTS;

            // Start with the chapters (section!) and iterate functions/headers and attach criteria
            foreach (XElement xChapter in xModel.Elements("Chapter"))
            {
                string name = getXElementValue(xChapter, "Name");
                string ID = getXElementValue(xChapter, "ID");
                string alias = getXElementValue(xChapter, "Alias");
                string overview = getXElementValue(xChapter, "Overview");
                string example = getXElementValue(xChapter, "Example");
                string actors = getXElementValue(xChapter, "Actors");
                string notes = string.Format("$OV${0}$EX${1}$AC${2}", overview, example, actors);

                EA.Package sectionPackage = (EA.Package)fmPackage.Packages.AddNew(name, "Package");
                sectionPackage.Notes = notes;
                sectionPackage.IsNamespace = true;
                sectionPackage.TreePos = int.Parse(ID);
                sectionPackage.Update();
                sectionPackage.Element.Alias = alias;
                sectionPackage.Element.Stereotype = R2Const.ST_SECTION;
                config.updateStyle(sectionPackage.Element);
                sectionPackage.Element.Update();
                sectionPackage.Element.Locked = R2Const.LOCK_ELEMENTS;

                // Create TaggedValues for extra notes
                addTaggedValue(sectionPackage.Element, "ID", ID);

                int TPos = 0;
                foreach (XElement xFunction in xChapter.Elements("Function"))
                {
                    EA.Element functionElement;
                    string parentID = getXElementValue(xFunction, "ParentID");
                    if (elements.ContainsKey(parentID))
                    {
                        EA.Element parentElement = elements[parentID];
                        functionElement = (EA.Element)parentElement.Elements.AddNew("", "Feature");
                    }
                    else // If the parent function doesnot exist; add to the sectionPackage; chapters are not in the functions dictionary
                    {
                        functionElement = (EA.Element)sectionPackage.Elements.AddNew("", "Feature");
                    }
                    string functionID = getXElementValue(xFunction, "ID");
                    addComposition(functionID, parentID);
                    importFunction(functionElement, xFunction);
                    functionElement.TreePos = TPos++; // Keep order from import file
                    functionElement.Update();
                    functionElement.Locked = R2Const.LOCK_ELEMENTS;
                }
            }

            // Add ConsequenceLinks
            foreach (Link clink in consequenceLinks.Values)
            {
                if (elements.ContainsKey(clink.targetID))
                {
                    EA.Element sourceElement = elements[clink.sourceID];
                    EA.Element targetElement = elements[clink.targetID];
                    EA.Connector connector = (EA.Connector)sourceElement.Connectors.AddNew("", "Dependency");
                    connector.Stereotype = R2Const.ST_CONSEQUENCELINK;
                    connector.Notes = clink.notes;
                    connector.SupplierID = targetElement.ElementID;
                    connector.Update();
                }
                else
                {
                    Repository.WriteOutput(Properties.Resources.OUTPUT_TAB_HL7_FM, String.Format("ConsequenceLink from {0} to {1}, target Function not found.", clink.sourceID, clink.targetID), -1);
                }
            }

            // Add SeeAlso Links
            foreach (Link slink in seeAlsoLinks.Values)
            {
                if (elements.ContainsKey(slink.targetID))
                {
                    EA.Element sourceElement = elements[slink.sourceID];
                    EA.Element targetElement = elements[slink.targetID];
                    EA.Connector connector = (EA.Connector)sourceElement.Connectors.AddNew("", "Dependency");
                    connector.Stereotype = R2Const.ST_SEEALSO;
                    connector.SupplierID = targetElement.ElementID;
                    connector.Update();
                }
                else
                {
                    Repository.WriteOutput(Properties.Resources.OUTPUT_TAB_HL7_FM, String.Format("SeeAlso link from {0} to {1}, target Function not found.", slink.sourceID, slink.targetID), -1);
                }
            }

            // Add compositions
            foreach (Link composition in compositions.Values)
            {
                if (elements.ContainsKey(composition.targetID))
                {
                    EA.Element sourceElement = elements[composition.sourceID];
                    EA.Element targetElement = elements[composition.targetID];
                    EA.Connector connector = (EA.Connector)sourceElement.Connectors.AddNew("", "Aggregation");
                    connector.SupplierID = targetElement.ElementID;
                    connector.SupplierEnd.Aggregation = 2; // "composite"
                    connector.Update();
                }
                else
                {
                    // If the target is the section package then ignore the link
                    int? sectionColor = config.getSectionColorInt(composition.targetID);
                    if (sectionColor == null)
                    {
                        Repository.WriteOutput(Properties.Resources.OUTPUT_TAB_HL7_FM, String.Format("Function composition from {0} to {1}, target not found.", composition.sourceID, composition.targetID), -1);
                    }
                }
            }
            progress.Close();

            Repository.EnableUIUpdates = true;
            Repository.BatchAppend = false;
            Repository.RefreshModelView(fmPackage.PackageID);
            Repository.EnsureOutputVisible(Properties.Resources.OUTPUT_TAB_HL7_FM);
        }

        private void importFunction(EA.Element functionElement, XElement xFunction)
        {
            string functionName = getXElementValue(xFunction, "Name");
            string functionID = getXElementValue(xFunction, "ID");
            elements.Add(functionID, functionElement);

            functionElement.Name = string.Format("{0} {1}", functionID, functionName);
            functionElement.Alias = functionID;

            // apply color
            config.updateStyle(functionElement);

            string functionStatement = getXElementValue(xFunction, "Statement");
            string functionDescription = getXElementValue(xFunction, "Description");
            string functionExample = getXElementValue(xFunction, "Example");
            string functionRow = xFunction.Attribute("Row").Value;
            string notes = string.Format("$ST${0}$DE${1}$EX${2}", functionStatement, functionDescription, functionExample);
            functionElement.Notes = notes;

            switch (xFunction.Element("Type").Value)
            {
                case "H":
                    functionElement.Stereotype = R2Const.ST_HEADER;
                    break;
                case "F":
                    functionElement.Stereotype = R2Const.ST_FUNCTION;
                    break;
            }
            functionElement.Update();
            functionElement.Locked = R2Const.LOCK_ELEMENTS;

            addTaggedValue(functionElement, "Row", functionRow);
            addReferenceTags(functionElement, xFunction);
            IEnumerable<XElement> seeAlsoList = xFunction.XPathSelectElements("SeeAlso/FunctionID");
            foreach (XElement id in seeAlsoList)
            {
                addSeeAlsoLink(functionID, id.Value.Trim());
            }

            importCriteria(functionElement, xFunction);
            progress.step();
        }

        private void importCriteria(EA.Element functionElement, XElement xFunction)
        {
            string functionID = getXElementValue(xFunction, "ID");
            foreach (XElement xCriteria in xFunction.Elements("Criteria"))
            {
                string criteriaID = string.Format("{0}#{1:00}", functionID, int.Parse(getXElementValue(xCriteria, "ID")));
                string criteriaText = getXElementValue(xCriteria, "Text");
                string criteriaOpt = getXElementValue(xCriteria, "Optionality");
                string criteriaRow = xCriteria.Attribute("Row").Value;
                string criteriaCond = getXElementValue(xCriteria, "Conditional");
                string criteriaDep = getXElementValue(xCriteria, "Dependent");

                EA.Element criteriaElement = (EA.Element)functionElement.Elements.AddNew(criteriaID, "Requirement");
                elements.Add(criteriaID, criteriaElement);
                criteriaElement.Notes = criteriaText;
                criteriaElement.Stereotype = R2Const.ST_CRITERIA;
                config.updateStyle(criteriaElement);
                criteriaElement.Update();
                criteriaElement.Locked = R2Const.LOCK_ELEMENTS;

                // Create TaggedValues for extra metadata
                addTaggedValue(criteriaElement, "Row", criteriaRow);
                addTaggedValue(criteriaElement, "Optionality", criteriaOpt);
                addTaggedValue(criteriaElement, "Conditional", criteriaCond);
                addTaggedValue(criteriaElement, "Dependent", criteriaDep);
                addReferenceTags(criteriaElement, xCriteria);

                // Add a Connector from Criteria to Function
                EA.Connector connector = (EA.Connector)criteriaElement.Connectors.AddNew("", "Association");
                connector.SupplierID = functionElement.ElementID;
                connector.Update();

                IEnumerable<XElement> linkList = xCriteria.XPathSelectElements("ConsequenceLink/FunctionID");
                foreach (XElement id in linkList)
                {
                    addConsequenceLink(functionID, id.Value, String.Format("{0} is the source of this link", criteriaID));
                }
                IEnumerable<XElement> seeAlsoList = xCriteria.XPathSelectElements("SeeAlso/FunctionID");
                foreach (XElement id in seeAlsoList)
                {
                    addSeeAlsoLink(criteriaID, id.Value.Trim());
                }
            }
        }

        private void addReferenceTags(EA.Element element, XElement xElement)
        {
            /*
              <Reference>
                <Alias>EHR-S_FM_R1.1</Alias>
                <Link>
                    <FunctionID>DC.1.4</FunctionID>
                    (<CriteriaID>1</CriteriaID>)
                </Link>
                <ChangeInfo>
                    <Type>Modified</Type>
                </ChangeInfo>
              </Reference>
            */
            XElement xRef = xElement.Element("Reference");
            if (xRef != null)
            {
                string refAlias = getXElementValue(xRef, "Alias");
                string refFunctionID = getXElementValue(xRef, "Link", "FunctionID");
                string refCriteriaID = getXElementValue(xRef, "Link", "CriteriaID");
                string refChangeInfoType = getXElementValue(xRef, "ChangeInfo", "Type");

                addTaggedValue(element, "Reference.Alias", refAlias);
                addTaggedValue(element, "Reference.FunctionID", refFunctionID);
                addTaggedValue(element, "Reference.CriteriaID", refCriteriaID);
                addTaggedValue(element, "Reference.ChangeInfo", refChangeInfoType);
            }
        }

        private bool addComposition(string sourceID, string targetID)
        {
            // Check for dubbles
            string key = string.Format("{0}-{1}", sourceID, targetID);
            if (compositions.ContainsKey(key))
            {
                Repository.WriteOutput(Properties.Resources.OUTPUT_TAB_HL7_FM, String.Format("Function {0} composition to {1}, already there", sourceID, targetID), -1);
                return false;
            }
            else
            {
                compositions[key] = new Link(sourceID, targetID, null);
                return true;
            }
        }

        private bool addConsequenceLink(string sourceID, string targetID, string notes)
        {
            // Check for dubbles
            string key = string.Format("{0}-{1}", sourceID, targetID);
            if (consequenceLinks.ContainsKey(key))
            {
                Repository.WriteOutput(Properties.Resources.OUTPUT_TAB_HL7_FM, String.Format("Function {0} link to {1}, already there", sourceID, targetID), -1);
                return false;
            }
            else
            {
                consequenceLinks[key] = new Link(sourceID, targetID, notes);
                return true;
            }
        }

        private bool addSeeAlsoLink(string sourceID, string targetID)
        {
            // Check for dubbles
            string key = string.Format("{0}-{1}", sourceID, targetID);
            if (seeAlsoLinks.ContainsKey(key))
            {
                Repository.WriteOutput(Properties.Resources.OUTPUT_TAB_HL7_FM, String.Format("Function {0} see-also to {1}, already there", sourceID, targetID), -1);
                return false;
            }
            else
            {
                seeAlsoLinks[key] = new Link(sourceID, targetID, null);
                return true;
            }
        }

        private void addTaggedValue(EA.Element element, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.AddNew(name, "TaggedValue");
                if (value.Length > 255)
                {
                    value = "{truncated}" + value.Substring(0, 244);
                }
                tv.Value = value;
                tv.Update();
            }
        }

        private string getXElementValue(XElement xElement, string elname)
        {
            XElement n = xElement.Element(elname);
            if (n != null)
            {
                return n.Value.Trim();
            }
            else
            {
                return String.Empty;
            }
        }

        private string getXElementValue(XElement xElement, string elname1, string elname2)
        {
            XElement n = xElement.Element(elname1);
            if (n != null)
            {
                n = n.Element(elname2);
                if (n != null)
                {
                    return n.Value;
                }
                else
                {
                    return String.Empty;
                }
            }
            else
            {
                return String.Empty;
            }
        }

        private string showFileDialog(string title, string filter, string fileName, bool open)
        {
            FileDialog dialog;
            if (open)
            {
                dialog = new OpenFileDialog();
            }
            else
            {
                dialog = new SaveFileDialog();
            }
            dialog.Filter = filter;
            dialog.InitialDirectory = "C:";
            dialog.Title = title;
            dialog.FileName = fileName;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return String.Empty;
            }
        }
    }

    class Link
    {
        public string sourceID;
        public string targetID;
        public string notes;

        public Link(string sourceID, string targetID, string notes)
        {
            this.sourceID = sourceID;
            this.targetID = targetID;
            this.notes = notes;
        }
    }
}
