using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace HL7_FM_EA_Extension
{
    class R11Importer
    {
        private Dictionary<string, EA.Element> functions = new Dictionary<string, EA.Element>();
        private List<string> functionCompSource = new List<string>();
        private List<string> functionCompTarget = new List<string>();
        private List<string> functionLinkSource = new List<string>();
        private List<string> functionLinkTarget = new List<string>();
        private List<string> functionSeeAlsoSource = new List<string>();
        private List<string> functionSeeAlsoTarget = new List<string>();
        private Dictionary<string, int> chapterColors = new Dictionary<string, int>();

        private const bool LOCK_ELEMENTS = true;
        
        public void import(EA.Repository Repository, EA.Package rootPackage)
        {
            Repository.EnableUIUpdates = false;
            Repository.BatchAppend = true;

            Repository.CreateOutputTab(Properties.Resources.OUTPUT_TAB_HL7_FM);
            Repository.ClearOutput(Properties.Resources.OUTPUT_TAB_HL7_FM);
            Repository.EnsureOutputVisible(Properties.Resources.OUTPUT_TAB_HL7_FM);

            chapterColors["DC"] = 0x00ffff;
            chapterColors["S"] = 0x00d7ff;
            chapterColors["IN"] = 0xffcc00;

            functions.Clear();
            functionCompSource.Clear();
            functionCompTarget.Clear();
            functionLinkSource.Clear();
            functionLinkTarget.Clear();
            functionSeeAlsoSource.Clear();
            functionSeeAlsoTarget.Clear();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"D:\VisualStudio Projects\HL7\EHRSFM_EA_AddIn\EHRSFM\EHRS_FunctionalModel_Rel1.1.xml");

            EA.Package fmPackage = (EA.Package)rootPackage.Packages.AddNew("EHR-S FM R1.1", "Package");
            fmPackage.Alias = xmlDoc.SelectSingleNode("/FunctionalModel/Name").InnerText;
            fmPackage.Version = xmlDoc.SelectSingleNode("/FunctionalModel/Version").InnerText + " " + xmlDoc.SelectSingleNode("/FunctionalModel/Date").InnerText;
            fmPackage.IsNamespace = true;
            fmPackage.Update();
            fmPackage.Element.Locked = LOCK_ELEMENTS;

            XmlNodeList chapterList = xmlDoc.SelectNodes("/FunctionalModel/Chapter");
            foreach (XmlNode chapterNode in chapterList)
            {
                string name = chapterNode.SelectSingleNode("Name").InnerText;
                string alias = chapterNode.SelectSingleNode("Alias").InnerText;
                string description = chapterNode.SelectSingleNode("Description").InnerText;
                EA.Package chapterPackage = (EA.Package)fmPackage.Packages.AddNew(name, "Package");
                chapterPackage.Alias = alias;
                chapterPackage.Notes = description;
                chapterPackage.IsNamespace = true;
                chapterPackage.Update();
                if (chapterColors.ContainsKey(alias))
                {
                    chapterPackage.Element.SetAppearance(1/*Base*/, 0/*BGCOLOR*/, chapterColors[alias]);
                }
                chapterPackage.Element.Update();
                chapterPackage.Element.Locked = LOCK_ELEMENTS;

                XmlNodeList functionList = chapterNode.SelectNodes("Function");
                foreach (XmlNode functionNode in functionList)
                {
                    EA.Element functionElement = (EA.Element)chapterPackage.Elements.AddNew("", "Feature");
                    importFunctionOrHeader(functionElement, functionNode, Repository);
                    functionElement.Update();
                    functionElement.Locked = LOCK_ELEMENTS;
                }
            }

            for (int i = 0; i < functionSeeAlsoSource.Count; i++)
            {
                string sourceID = functionSeeAlsoSource[i];
                string targetID = functionSeeAlsoTarget[i];
                if (functions.ContainsKey(targetID))
                {
                    EA.Element sourceElement = functions[sourceID];
                    EA.Element targetElement = functions[targetID];
                    EA.Connector connector = (EA.Connector)sourceElement.Connectors.AddNew("", "Association");
                    connector.Stereotype = "See Also";
                    connector.SupplierID = targetElement.ElementID;
                    connector.Update();
                }
                else
                {
                    EAHelper.LogMessage(string.Format("See Also from {0} to {1}, target not found.", sourceID, targetID));
                }
            }

            for (int i = 0; i < functionLinkSource.Count; i++)
            {
                string sourceID = functionLinkSource[i];
                string targetID = functionLinkTarget[i];
                if (functions.ContainsKey(targetID))
                {
                    EA.Element sourceElement = functions[sourceID];
                    EA.Element targetElement = functions[targetID];
                    EA.Connector connector = (EA.Connector)sourceElement.Connectors.AddNew("", "Dependency");
                    connector.SupplierID = targetElement.ElementID;
                    connector.Update();
                }
                else
                {
                    EAHelper.LogMessage(string.Format("Function link from {0} to {1}, target not found.", sourceID, targetID));
                }
            }

            for (int i = 0; i < functionCompSource.Count; i++)
            {
                string sourceID = functionCompSource[i];
                string targetID = functionCompTarget[i];
                if (functions.ContainsKey(targetID))
                {
                    EA.Element sourceElement = functions[sourceID];
                    EA.Element targetElement = functions[targetID];
                    EA.Connector connector = (EA.Connector)sourceElement.Connectors.AddNew("", "Aggregation");
                    connector.SupplierID = targetElement.ElementID;
                    connector.SupplierEnd.Aggregation = 2; // "composite"
                    connector.Update();
                }
                else
                {
                    EAHelper.LogMessage(string.Format("Function composition from {0} to {1}, target not found.", sourceID, targetID));
                }
            }

            Repository.EnableUIUpdates = true;
            Repository.BatchAppend = false;

            Repository.RefreshModelView(fmPackage.PackageID);

            Repository.EnsureOutputVisible(Properties.Resources.OUTPUT_TAB_HL7_FM);
        }

        private void importHeaderFunctions(EA.Element headerElement, XmlNode headerNode, EA.Repository Repository)
        {
            string headerID = headerNode.SelectSingleNode("ID").InnerText;
            XmlNodeList functionList = headerNode.SelectNodes("Function");
            foreach (XmlNode functionNode in functionList)
            {
                EA.Element functionElement = (EA.Element)headerElement.Elements.AddNew("", "Feature");
                string functionID = functionNode.SelectSingleNode("ID").InnerText;
                addComposition(functionID, headerID, Repository);
                importFunctionOrHeader(functionElement, functionNode, Repository);
                functionElement.Update();
                functionElement.Locked = LOCK_ELEMENTS;
            }

            XmlNodeList linkList = headerNode.SelectNodes("SeeAlso/FunctionID");
            foreach (XmlNode id in linkList)
            {
                addSeeAlso(headerID, id.InnerText, Repository);
            }
        }

        private void importFunctionOrHeader(EA.Element functionElement, XmlNode functionNode, EA.Repository Repository)
        {
            string functionName = functionNode.SelectSingleNode("Name").InnerText;
            // maybe put functionID in Tagged Value?
            string functionID = functionNode.SelectSingleNode("ID").InnerText;
            functions.Add(functionID, functionElement);

            functionElement.Name = string.Format("{0} {1}", functionID, functionName);

            // apply color
            string chapter = functionID.Substring(0, functionID.IndexOf('.'));
            if (chapterColors.ContainsKey(chapter))
            {
                functionElement.SetAppearance(1/*Base*/, 0/*BGCOLOR*/, chapterColors[chapter]);
            }

            string functionStatement = getInnerText(functionNode, "Statement");
            string functionDescription = getInnerText(functionNode, "Description");
            StringBuilder sb = new StringBuilder();
            sb.Append("<b>Statement: </b>").AppendLine(functionStatement).Append("<b>Description: </b>").AppendLine(functionDescription);
            functionElement.Notes = sb.ToString();

            string functionType = functionNode.SelectSingleNode("Type").InnerText;
            if ("H".Equals(functionType))
            {
                functionElement.Stereotype = "Header";
                importHeaderFunctions(functionElement, functionNode, Repository);
            }
            else if ("F".Equals(functionType))
            {
                functionElement.Stereotype = "Function";
            }
            importCriteria(functionElement, functionNode, Repository);

            XmlNodeList linkList = functionNode.SelectNodes("SeeAlso/FunctionID");
            foreach (XmlNode id in linkList)
            {
                addSeeAlso(functionID, id.InnerText, Repository);
            }
        }

        private void importCriteria(EA.Element functionElement, XmlNode functionNode, EA.Repository Repository)
        {
            string functionID = functionNode.SelectSingleNode("ID").InnerText;
            XmlNodeList criteriaList = functionNode.SelectNodes("ConformanceCriteria");
            foreach (XmlNode criteriaNode in criteriaList)
            {
                string criteriaID = String.Format("{0}#{1}", functionID, criteriaNode.SelectSingleNode("ID").InnerText);
                string criteriaText = criteriaNode.SelectSingleNode("Text").InnerText;
                EA.Element criteriaElement = (EA.Element)functionElement.Elements.AddNew(String.Format("{0} {1}", criteriaID, criteriaText), "Requirement");
                functions.Add(criteriaID, criteriaElement);
                criteriaElement.Stereotype = "Criteria";
                // apply color
                string chapter = functionID.Substring(0, functionID.IndexOf('.'));
                if (chapterColors.ContainsKey(chapter))
                {
                    criteriaElement.SetAppearance(1/*Base*/, 0/*BGCOLOR*/, chapterColors[chapter]);
                }
                criteriaElement.Update();
                criteriaElement.Locked = LOCK_ELEMENTS;
                string criteriaOpt = criteriaNode.SelectSingleNode("Optionality").InnerText;
                EA.TaggedValue taggedValue = (EA.TaggedValue)criteriaElement.TaggedValues.AddNew("Optionality", "TaggedValue");
                taggedValue.Value = criteriaOpt;
                taggedValue.Update();

                // Add a Connector from Criteria to Function
                EA.Connector connector = (EA.Connector)criteriaElement.Connectors.AddNew("", "Association");
                connector.SupplierID = functionElement.ElementID;
                connector.Update();

                XmlNodeList linkList = criteriaNode.SelectNodes("Link/FunctionID");
                foreach (XmlNode id in linkList)
                {
                    addLink(functionID, id.InnerText, Repository);
                }
            }
        }

        private bool addSeeAlso(string sourceID, string targetID, EA.Repository Repository)
        {
            // Check for dubbles
            for (int i = 0; i < functionSeeAlsoSource.Count; i++)
            {
                if (functionSeeAlsoSource[i] == sourceID && functionSeeAlsoTarget[i] == targetID)
                {
                    EAHelper.LogMessage(string.Format("Duplicate See Also from {0} to {1}", sourceID, targetID));
                    return false;
                }
            }
            functionSeeAlsoSource.Add(sourceID);
            functionSeeAlsoTarget.Add(targetID);
            return true;
        }

        private bool addComposition(string sourceID, string targetID, EA.Repository Repository)
        {
            // Check for dubbles
            for (int i = 0; i < functionCompSource.Count; i++)
            {
                if (functionCompSource[i] == sourceID && functionCompTarget[i] == targetID)
                {
                    EAHelper.LogMessage(string.Format("Duplicate Function composition from {0} to {1}", sourceID, targetID));
                    return false;
                }
            }
            functionCompSource.Add(sourceID);
            functionCompTarget.Add(targetID);
            return true;
        }

        private bool addLink(string sourceID, string targetID, EA.Repository Repository)
        {
            // Check for dubbles
            for (int i = 0; i < functionLinkSource.Count; i++)
            {
                if (functionLinkSource[i] == sourceID && functionLinkTarget[i] == targetID)
                {
                    EAHelper.LogMessage(string.Format("Duplicate Function link from {0} to {1}", sourceID, targetID));
                    return false;
                }
            }
            functionLinkSource.Add(sourceID);
            functionLinkTarget.Add(targetID);
            return true;
        }

        public string getInnerText(XmlNode node, string xpath)
        {
            XmlNode n = node.SelectSingleNode(xpath);
            if (n != null)
            {
                return n.Value;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}
