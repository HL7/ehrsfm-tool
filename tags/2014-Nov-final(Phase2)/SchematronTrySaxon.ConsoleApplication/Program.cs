using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saxon.Api;
using System.IO;

namespace SchematronTrySaxon.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().RunSchematron();
        }

        private void RunSchematron()
        {
            Uri baseuri = new Uri(@"D:\VisualStudio Projects\HL7\SchematronTrySaxon.ConsoleApplication\");
            Uri baseuri2 = new Uri(@"D:\VisualStudio Projects\HL7\SchematronTry.ConsoleApplication\");

            // Create a Processor instance.
            Processor processor = new Processor();
            XsltCompiler compiler = processor.NewXsltCompiler();
            DocumentBuilder builder = processor.NewDocumentBuilder();
            builder.XmlResolver = new UserXmlResolver();
            //builder.BaseUri = baseuri;

// Transform the Schematron

            // Create a transformer for the stylesheet.
            XsltTransformer transformer = compiler.Compile(new Uri(baseuri, @"iso-schematron-xslt2\iso_svrl_for_xslt2.xsl")).Load();

            // Load the source document
            XdmNode input = builder.Build(new Uri(baseuri, @"Test2.sch"));
//            XdmNode input = builder.Build(new Uri(baseuri, @"Test3.sch"));

            // Set the root node of the source document to be the initial context node
            transformer.InitialContextNode = input;

            // Create a serializer, with output to the standard output stream
            Serializer serializer = new Serializer();
            serializer.SetOutputFile(@"D:\VisualStudio Projects\HL7\SchematronTrySaxon.ConsoleApplication\Test2.sch.xsl");
//            StringWriter schxsl = new StringWriter();
//            serializer.SetOutputWriter(schxsl);

            // Transform the source XML and serialize the result document
            transformer.Run(serializer);

// Execute the Schematron

            // Create a transformer for the stylesheet.
            XsltTransformer transformer2 = compiler.Compile(new Uri(@"D:\VisualStudio Projects\HL7\SchematronTrySaxon.ConsoleApplication\Test2.sch.xsl")).Load();
            //XsltTransformer transformer2 = compiler.Compile(new StringReader(schxsl.GetStringBuilder().ToString())).Load();

            // Load the source document
//            XdmNode input2 = builder.Build(new Uri(baseuri2, @"fm-max.xml"));
            XdmNode input2 = builder.Build(new Uri(baseuri, @"XMLFile1.xml"));

            // Set the root node of the source document to be the initial context node
            transformer2.InitialContextNode = input;

            // Create a serializer, with output to the standard output stream
            Serializer serializer2 = new Serializer();
            serializer2.SetOutputFile(@"D:\VisualStudio Projects\HL7\SchematronTrySaxon.ConsoleApplication\fm-max.xml-output");

            // Transform the source XML and serialize the result document
            transformer2.Run(serializer2);
        }
    }

    public class UserXmlResolver : System.Xml.XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, String role, Type ofObjectToReturn)
        {
            Console.WriteLine(absoluteUri + " (role=" + role + ")");
            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
}
