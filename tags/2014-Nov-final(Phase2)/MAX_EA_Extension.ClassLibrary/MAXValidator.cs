using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Saxon.Api;

namespace MAX_EA_Extension
{
    public class MAXValidator
    {
        public bool validate(EA.Repository Repository, string maxFile, string schFile, string svrlFile)
        {
            string iso_sch_xsl_file = Main.getAppDataFullPath(@"Schematron\iso_svrl_for_xslt2.xsl");
            string sch_xsl_file = Path.Combine(Path.GetTempPath(), "max-tmp.sch.xsl");

            // Create a Processor instance.
            Processor processor = new Processor();
            XsltCompiler compiler = processor.NewXsltCompiler();
            compiler.ErrorList = new List<StaticError>();
            DocumentBuilder builder = processor.NewDocumentBuilder();

            // First transform the Schematron to XSLT
            try
            {
                // Create a transformer for the stylesheet.
                XsltTransformer transformer = compiler.Compile(new Uri(iso_sch_xsl_file)).Load();

                // Load the source document
                XdmNode input = builder.Build(new Uri(schFile));

                // Set the root node of the source document to be the initial context node
                transformer.InitialContextNode = input;

                // Create a serializer, with output to the standard output stream
                Serializer serializer = new Serializer();
                //StringWriter output = new StringWriter();
                //serializer.SetOutputWriter(output);
                serializer.SetOutputFile(sch_xsl_file);

                // Transform the source XML and serialize the result document
                transformer.Run(serializer);
            }
            catch (Exception ex)
            {
                if (Repository == null)
                {
                    Console.WriteLine(ex);
                    StringBuilder sb = new StringBuilder();
                    foreach (var error in compiler.ErrorList)
                    {
                        sb.AppendLine(error.ToString());
                    }
                    Console.WriteLine(sb);
                }
                else
                {
                    Main.LogMessage(Repository, ex.Message, 0);
                    foreach (var error in compiler.ErrorList)
                    {
                        Main.LogMessage(Repository, error.ToString(), 0);
                    }
                }
                return true;
            }

            // Now run the Schematron XSLT on the XML
            try
            {
                // Create a transformer for the stylesheet.
                XsltTransformer transformer = compiler.Compile(new Uri(sch_xsl_file)).Load();

                // Load the source document
                XdmNode input = builder.Build(new Uri(maxFile));

                // Set the root node of the source document to be the initial context node
                transformer.InitialContextNode = input;

                // Create a serializer, with output to the standard output stream
                Serializer serializer = new Serializer();
                serializer.SetOutputFile(svrlFile);

                // Transform the source XML and serialize the result document
                transformer.Run(serializer);

                // Show messages in Output Tab
                XmlReader xReader = XmlReader.Create(svrlFile);
                using (xReader)
                {
                    XElement svrl = XElement.Load(xReader);
                    XmlNamespaceManager nsmgr = new XmlNamespaceManager(xReader.NameTable);
                    nsmgr.AddNamespace("svrl", "http://purl.oclc.org/dsdl/svrl");

                    appendSvrlMessagesToOutputTab(Repository, svrl.XPathSelectElements("//svrl:successful-report", nsmgr), nsmgr);
                    appendSvrlMessagesToOutputTab(Repository, svrl.XPathSelectElements("//svrl:failed-assert", nsmgr), nsmgr);
                }
                return false;
            }
            catch (Exception ex)
            {
                if (Repository == null)
                {
                    Console.WriteLine(ex);
                    StringBuilder sb = new StringBuilder();
                    foreach (var error in compiler.ErrorList)
                    {
                        sb.AppendLine(error.ToString());
                    }
                    Console.WriteLine(sb);
                }
                else
                {
                    Main.LogMessage(Repository, ex.Message, 0);
                    foreach (var error in compiler.ErrorList)
                    {
                        Main.LogMessage(Repository, error.ToString(), 0);
                    }
                }
                return true;
            }
        }

        private void appendSvrlMessagesToOutputTab(EA.Repository repository, IEnumerable<XElement> xSvrlMessages, XmlNamespaceManager nsmgr)
        {
            foreach (XElement xSvrlMessage in xSvrlMessages)
            {
                // use svrl:failed-assert @location to find object in MAX file
                string message = xSvrlMessage.XPathSelectElement("svrl:text", nsmgr).Value;
                Main.LogMessage(repository, message, 0);

                //EA.ProjectIssues issue = (EA.ProjectIssues)Repository.Issues.AddNew(issueName, "ProjectIssues");
                //issue.Update();
            }
        }
    }
}
