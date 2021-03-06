﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Xml.Xsl;
using System.Windows.Forms;
using HL7_FM_EA_Extension.R2ModelV2.Base;

namespace HL7_FM_EA_Extension
{
    class R2Validator
    {
        // Tagged value used for externalized ID
        private const string TV_MAX_ID = "MAX::ID";

        public void validate(EA.Repository repository, EA.Package rootPackage)
        {
            string sch_filepath = null;
            switch (rootPackage.StereotypeEx)
            {
                case R2Const.ST_FM:
                    sch_filepath = Main.getAppDataFullPath(@"Schematron\EHRS_FM_R2-validation.sch");
                    break;
                case R2Const.ST_FM_PROFILEDEFINITION:
                    sch_filepath = Main.getAppDataFullPath(@"Schematron\EHRS_FM_R2_FPDEF-validation.sch");
                    break;
                case R2Const.ST_FM_PROFILE:
                    sch_filepath = Main.getAppDataFullPath(@"Schematron\EHRS_FM_R2_FP-validation.sch");
                    break;
                default:
                    MessageBox.Show(string.Format("Validation not available for {0}.\nChoose ", rootPackage.Name), "Choose other package", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
            }

            repository.CreateOutputTab(Properties.Resources.OUTPUT_TAB_HL7_FM);
            repository.ClearOutput(Properties.Resources.OUTPUT_TAB_HL7_FM);
            repository.EnsureOutputVisible(Properties.Resources.OUTPUT_TAB_HL7_FM);

            // TODO: Change to memorybased!!
            // TODO: Transform once to XSL, only if the xsl is older or not existing

            // Create and load the transform with script execution enabled.
            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings {EnableScript = true};
            XmlUrlResolver resolver = new XmlUrlResolver();

            // transform the Schematron to a XSL
            string iso_sch_xsl_filepath = Main.getAppDataFullPath(@"Schematron\iso-schematron-xslt1\iso_svrl_for_xslt1.xsl");
            transform.Load(iso_sch_xsl_filepath, settings, resolver);
            string sch_xsl_filepath = Main.getAppDataFullPath(@"Schematron\EHRS_FM_R2-validation.sch.xsl");
            transform.Transform(sch_filepath, sch_xsl_filepath);

            // export to temp MAX file
            string temp_max_file = Main.getAppDataFullPath(@"Schematron\__temp__.max");
            new MAX_EA.MAXExporter3().exportPackage(repository, rootPackage, temp_max_file);

            // now execute the Schematron XSL
            transform.Load(sch_xsl_filepath, settings, resolver);
            string svrl_filepath = Main.getAppDataFullPath(@"Schematron\svrl_output.xml");
            transform.Transform(temp_max_file, svrl_filepath);

            // build element dictionary
            Dictionary<string, EA.Element> eaElementDict = new Dictionary<string, EA.Element>();
            recurseEaPackage(rootPackage, eaElementDict);

            XmlReader xReader = XmlReader.Create(svrl_filepath);
            // make sure file gets closed
            using (xReader)
            {
                XElement svrl = XElement.Load(xReader);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xReader.NameTable);
                nsmgr.AddNamespace("svrl", "http://purl.oclc.org/dsdl/svrl");

                appendSvrlMessagesToOutputTab(repository, svrl.XPathSelectElements("//svrl:successful-report", nsmgr), eaElementDict, nsmgr);
                appendSvrlMessagesToOutputTab(repository, svrl.XPathSelectElements("//svrl:failed-assert", nsmgr), eaElementDict, nsmgr);
            }
            MessageBox.Show("Validation done.\nCheck output tab for message and issues.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            repository.EnsureOutputVisible(Properties.Resources.OUTPUT_TAB_HL7_FM);
        }

        private void appendSvrlMessagesToOutputTab(EA.Repository repository, IEnumerable<XElement> xSvrlMessages, Dictionary<string, EA.Element> eaElementDict, XmlNamespaceManager nsmgr)
        {
            foreach (XElement xSvrlMessage in xSvrlMessages)
            {
                string idtxt = xSvrlMessage.XPathSelectElement("svrl:text", nsmgr).Value;
                // Not all issues are associated to an element
                XElement xSvrlDiag = xSvrlMessage.XPathSelectElement("svrl:diagnostic-reference", nsmgr);
                string code = xSvrlDiag.Attribute("diagnostic").Value;
                string message = xSvrlDiag.Value.Trim();
                if (eaElementDict.ContainsKey(idtxt))
                {
                    EA.Element element = eaElementDict[idtxt];
                    string issueName = string.Format("{0}:{1} - {2}", code, message, element.Name);
                    EAHelper.LogMessage(issueName, element.ElementID);
                }
                else
                {
                    string issueName = string.Format("{0}:{1}", code, message);
                    EAHelper.LogMessage(issueName);
                }

                //EA.ProjectIssues issue = (EA.ProjectIssues)Repository.Issues.AddNew(issueName, "ProjectIssues");
                //issue.Update();
            }
        }

        private void recurseEaPackage(EA.Package eaPackage, Dictionary<string, EA.Element> eaElementDict)
        {
            foreach (EA.Package eaSubPackage in eaPackage.Packages)
            {
                EA.TaggedValue tvSID = (EA.TaggedValue)eaSubPackage.Element.TaggedValues.GetByName(TV_MAX_ID);
                if (tvSID != null)
                {
                    eaElementDict[tvSID.Value.ToUpper()] = eaSubPackage.Element;
                }
                else
                {
                    eaElementDict[eaSubPackage.PackageID.ToString()] = eaSubPackage.Element;
                }
                recurseEaPackage(eaSubPackage, eaElementDict);
            }
            foreach (EA.Element eaElement in eaPackage.Elements)
            {
                EA.TaggedValue tvEID = (EA.TaggedValue)eaElement.TaggedValues.GetByName(TV_MAX_ID);
                if (tvEID != null)
                {
                    eaElementDict[tvEID.Value.ToUpper()] = eaElement;
                }
                else
                {
                    eaElementDict[eaElement.ElementID.ToString()] = eaElement;
                }
                recurseEaElements(eaElement, eaElementDict);
            }
        }

        private void recurseEaElements(EA.Element eaElement, Dictionary<string, EA.Element> eaElementDict)
        {
            foreach (EA.Element eaChildElement in eaElement.Elements)
            {
                EA.TaggedValue tvID = (EA.TaggedValue)eaChildElement.TaggedValues.GetByName(TV_MAX_ID);
                if (tvID != null)
                {
                    eaElementDict[tvID.Value.ToUpper()] = eaChildElement;
                }
                else
                {
                    eaElementDict[eaChildElement.ElementID.ToString()] = eaChildElement;
                }
                recurseEaElements(eaChildElement, eaElementDict);
            }
        }
    }
}
