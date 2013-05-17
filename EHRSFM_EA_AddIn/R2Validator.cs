using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Xml.Xsl;
using System.Windows.Forms;

namespace R4C_EHRSFM_EA_AddIn
{
    class R2Validator
    {
        public void validate(EA.Repository Repository, EA.Package rootPackage)
        {
            if (!R2Const.ST_FM.Equals(rootPackage.StereotypeEx))
            {
                MessageBox.Show("Select an <HL7-FM> Package.\nValidation works on full FM.");
                return;
            }

            Repository.CreateOutputTab(Properties.Resources.OUTPUT_NM_EHRSFM);
            Repository.ClearOutput(Properties.Resources.OUTPUT_NM_EHRSFM);
            Repository.EnsureOutputVisible(Properties.Resources.OUTPUT_NM_EHRSFM);

            // TODO: Change to memorybased!!
            // TODO: Transform once to XSL, only if the xsl is older or not existing

            // Create and load the transform with script execution enabled.
            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings();
            settings.EnableScript = true;
            XmlUrlResolver resolver = new XmlUrlResolver();

            // transform the Schematron to a XSL
            string iso_sch_xsl_filepath = getAppDataFullPath(@"iso-schematron-xslt1\iso_svrl_for_xslt1.xsl");
            transform.Load(iso_sch_xsl_filepath, settings, resolver);
            string sch_filepath = getAppDataFullPath("EHRS_FM_R2-validation (2013-apr-18).sch");
            string sch_xsl_filepath = getAppDataFullPath("EHRS_FM_R2-validation.sch.xsl");
            transform.Transform(sch_filepath, sch_xsl_filepath);

            // export to temp max.xml file
            string fm_max_file = getAppDataFullPath("temp.max.xml");
            new MAX_EA.MAXExporter2().export(Repository, false, fm_max_file);

            // now execute the Schematron XSL
            transform.Load(sch_xsl_filepath, settings, resolver);
            string svrl_filepath = getAppDataFullPath("svrl_output.xml");
            transform.Transform(fm_max_file, svrl_filepath);

            XmlReader xReader = XmlReader.Create(svrl_filepath);
            // make sure file gets closed
            using (xReader)
            {
                XElement svrl = XElement.Load(xReader);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xReader.NameTable);
                nsmgr.AddNamespace("svrl", "http://purl.oclc.org/dsdl/svrl");

                IEnumerable<XElement> xSvrlSuccessEnum = svrl.XPathSelectElements("//svrl:successful-report", nsmgr);
                foreach (XElement xSvrlSuccess in xSvrlSuccessEnum)
                {
                    string idtxt = xSvrlSuccess.XPathSelectElement("svrl:text", nsmgr).Value;
                    int ID = int.Parse(idtxt);
                    EA.Element element = Repository.GetElementByID(ID);
                    XElement xSvrlDiag = xSvrlSuccess.XPathSelectElement("svrl:diagnostic-reference", nsmgr);
                    string code = xSvrlDiag.Attribute("diagnostic").Value;
                    string message = xSvrlDiag.Value.Trim();
                    string issueName = string.Format("{0}:{1} - {2}", code, message, element.Name);
                    Repository.WriteOutput(Properties.Resources.OUTPUT_NM_EHRSFM, issueName, ID);

                    //EA.ProjectIssues issue = (EA.ProjectIssues)Repository.Issues.AddNew(issueName, "ProjectIssues");
                    //issue.Update();
                }

                IEnumerable<XElement> xSvrlFailEnum = svrl.XPathSelectElements("//svrl:failed-assert", nsmgr);
                foreach (XElement xSvrlFail in xSvrlFailEnum)
                {
                    string idtxt = xSvrlFail.XPathSelectElement("svrl:text", nsmgr).Value;
                    // BEGIN: workaround for 'Criteria'-issue
                    if (idtxt.IndexOf('C') != -1)
                    {
                        idtxt = idtxt.Substring(0, idtxt.IndexOf('C'));
                    }
                    // END: workaround for 'Criteria'-issue
                    int ID = int.Parse(idtxt);
                    EA.Element element = Repository.GetElementByID(ID);
                    XElement xSvrlDiag = xSvrlFail.XPathSelectElement("svrl:diagnostic-reference", nsmgr);
                    string code = xSvrlDiag.Attribute("diagnostic").Value;
                    string message = xSvrlDiag.Value.Trim();
                    string issueName = string.Format("{0}:{1} - {2}", code, message, element.Name);
                    Repository.WriteOutput(Properties.Resources.OUTPUT_NM_EHRSFM, issueName, ID);

                    //EA.ProjectIssues issue = (EA.ProjectIssues)Repository.Issues.AddNew(issueName, "ProjectIssues");
                    //issue.Update();
                }
            }
            MessageBox.Show("Validation done.\nCheck \"Project Status/issues\" or output tab for issues.");

            Repository.EnsureOutputVisible(Properties.Resources.OUTPUT_NM_EHRSFM);
        }

        private void validateSvrlReportFile(EA.Repository Repository)
        {
            Repository.CreateOutputTab(Properties.Resources.OUTPUT_NM_EHRSFM);
            Repository.ClearOutput(Properties.Resources.OUTPUT_NM_EHRSFM);
            Repository.EnsureOutputVisible(Properties.Resources.OUTPUT_NM_EHRSFM);

            // TODO: show options screen
            // TODO: convert UML model to MAX
            // TODO: run schematron on MAX
            string svrl_filepath = getAppDataFullPath("svrl-report.xml");

            // put svrl-report in EA Console
            XmlReader xReader = XmlReader.Create(svrl_filepath);
            // make sure file gets closed
            using (xReader)
            {
                XElement svrl = XElement.Load(xReader);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xReader.NameTable);
                nsmgr.AddNamespace("svrl", "http://purl.oclc.org/dsdl/svrl");

                IEnumerable<XElement> xSvrlSuccessEnum = svrl.XPathSelectElements("//svrl:successful-report", nsmgr);
                foreach (XElement xSvrlSuccess in xSvrlSuccessEnum)
                {
                    XElement xSvrlText = xSvrlSuccess.XPathSelectElement("svrl:text", nsmgr);
                    Repository.WriteOutput(Properties.Resources.OUTPUT_NM_EHRSFM, string.Format("SUCCESS: {0}", xSvrlText.Value.Trim()), -1);
                }

                IEnumerable<XElement> xSvrlFailEnum = svrl.XPathSelectElements("//svrl:failed-assert", nsmgr);
                foreach (XElement xSvrlFail in xSvrlFailEnum)
                {
                    string flag = "FAIL";
                    XAttribute xFlag = xSvrlFail.Attribute(XName.Get("flag"));
                    if (xFlag != null)
                    {
                        switch (xFlag.Value)
                        {
                            case "warning": flag = "WARNING"; break;
                            case "error": flag = "ERROR"; break;
                        }
                    }
                    XElement xSvrlText = xSvrlFail.XPathSelectElement("svrl:text", nsmgr);
                    Repository.WriteOutput(Properties.Resources.OUTPUT_NM_EHRSFM, string.Format("{0}: {1}", flag, xSvrlText.Value.Trim()), -1);
                }
            }

            Repository.EnsureOutputVisible(Properties.Resources.OUTPUT_NM_EHRSFM);
        }

        private string getAppDataFullPath(string filename)
        {
            string filepath;
            // Check if in developer mode
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Devel path
                filepath = string.Format(@"D:\VisualStudio Projects\HL7\EHRSFM_EA_AddIn\EHRSFM_EA_AddIn_Files\{0}", filename);
            }
            else
            {
                filepath = string.Format(@"{0}\EHRSFM_EA_AddIn_Files\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), filename);
            }
            return filepath;
        }
    }
}
