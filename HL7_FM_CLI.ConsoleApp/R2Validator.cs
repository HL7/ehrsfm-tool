using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.IO;
using System.Xml.Xsl;
using System.Xml.Serialization;
using HL7_MAX.MAXSchema;
using HL7_FM_CLI.R2ModelV2.Base;

namespace HL7_FM_CLI
{
    class R2Validator
    {
        // Tagged value used for externalized ID
        private const string TV_MAX_ID = "MAX::ID";

        public void validate(string filename)
        {
            // read the max file
            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            StreamReader stream = new StreamReader(filename);
            ModelType model = (ModelType)serializer.Deserialize(stream);

            // root object is object without parent
            ObjectType rootObject = model.objects.SingleOrDefault(o => o.parentId == null);

            // based on stereotype pick right validation schematron
            string sch_filepath = null;
            switch (rootObject.stereotype)
            {
                case R2Const.ST_FM:
                    sch_filepath = getSchematronPath("EHRS_FM_R2-validation.sch");
                    break;
                case R2Const.ST_FM_PROFILEDEFINITION:
                    sch_filepath = getSchematronPath("EHRS_FM_R2_FPDEF-validation.sch");
                    break;
                case R2Const.ST_FM_PROFILE:
                    sch_filepath = getSchematronPath("EHRS_FM_R2_FP-validation.sch");
                    break;
                default:
                    Console.WriteLine("Validation not available for {0}.");
                    return;
            }

            // TODO: Change to memorybased!!
            // TODO: Transform once to XSL, only if the xsl is older or not existing

            // Create and load the transform with script execution enabled.
            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings {EnableScript = true};
            XmlUrlResolver resolver = new XmlUrlResolver();

            // transform the Schematron to a XSL
            string iso_sch_xsl_filepath = getSchematronPath("iso-schematron-xslt1/iso_svrl_for_xslt1.xsl");
            transform.Load(iso_sch_xsl_filepath, settings, resolver);
            string sch_xsl_filepath = getSchematronPath("EHRS_FM_R2-validation.sch.xsl");
            transform.Transform(sch_filepath, sch_xsl_filepath);

            // now execute the Schematron XSL
            transform.Load(sch_xsl_filepath, settings, resolver);
            string svrl_filepath = getSchematronPath("svrl_output.xml");
            transform.Transform(filename, svrl_filepath);

            // build maxObject dictionary
            var maxObjectDict = new Dictionary<string, ObjectType>();
            foreach (ObjectType maxObj1 in model.objects)
            {
                maxObjectDict[maxObj1.id] = maxObj1;
            }

            XmlReader xReader = XmlReader.Create(svrl_filepath);
            XmlReader xReader2 = XmlReader.Create(filename);
            // make sure file gets closed
            using (xReader)
            {
                XElement svrl = XElement.Load(xReader);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xReader.NameTable);
                nsmgr.AddNamespace("svrl", "http://purl.oclc.org/dsdl/svrl");

                XElement max = XElement.Load(xReader2);

                appendSvrlMessagesToConsole(svrl.XPathSelectElements("//svrl:successful-report", nsmgr), maxObjectDict, nsmgr, max);
                appendSvrlMessagesToConsole(svrl.XPathSelectElements("//svrl:failed-assert", nsmgr), maxObjectDict, nsmgr, max);
            }
            Console.WriteLine("Validation done.");
        }

        private void appendSvrlMessagesToConsole(IEnumerable<XElement> xSvrlMessages, Dictionary<string, ObjectType> maxObjectDict, XmlNamespaceManager nsmgr, XElement max)
        {
            foreach (XElement xSvrlMessage in xSvrlMessages)
            {
                string idtxt = xSvrlMessage.XPathSelectElement("svrl:text", nsmgr).Value;
                // Not all issues are associated to an element
                XElement xSvrlDiag = xSvrlMessage.XPathSelectElement("svrl:diagnostic-reference", nsmgr);
                string code = xSvrlDiag.Attribute("diagnostic").Value;
                string message = xSvrlDiag.Value.Trim();
                if (maxObjectDict.ContainsKey(idtxt))
                {
                    ObjectType maxObject = maxObjectDict[idtxt];
                    string issueName = string.Format("{0}:{1} - {2}", code, message, maxObject.name);
                    Console.WriteLine("{0} {1}", idtxt, issueName);
                }
                else
                {
                    // *[local-name()='model' and namespace-uri()='http://www.umcg.nl/MAX']/objects/object[357]
                    string location = xSvrlMessage.Attribute("location").Value;
                    string xpath = location.Substring(location.IndexOf("/objects/"));
                    string name = max.XPathSelectElement("/" + xpath + "/name").Value;

                    string issueName = string.Format("{0}:{1} - {2}", code, message, name);
                    Console.WriteLine(issueName);
                }
            }
        }

        private string getSchematronPath(string path)
        {
            // TODO: figure out how to expand this path
            return Path.Join("Schematron", path);
        }
    }
}
