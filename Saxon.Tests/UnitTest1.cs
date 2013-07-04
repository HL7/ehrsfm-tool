using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saxon.Api;
using System.IO;

namespace Saxon.TestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Uri baseuri = new Uri(@"D:\Develop\ehrsfm_profile\trunk\Saxon.Tests\");

            // Create a Processor instance.
            Processor processor = new Processor();
            XsltCompiler compiler = processor.NewXsltCompiler();
            compiler.ErrorList = new List<StaticError>();
            DocumentBuilder builder = processor.NewDocumentBuilder();
            //builder.XmlResolver = new UserXmlResolver();

            try
            {
                // Create a transformer for the stylesheet.
                XsltTransformer transformer = compiler.Compile(new Uri(baseuri, @"XSLTFile1.xslt")).Load();

                // Load the source document
                XdmNode input = builder.Build(new Uri(baseuri, @"XMLFile1.xml"));

                // Set the root node of the source document to be the initial context node
                transformer.InitialContextNode = input;

                // Create a serializer, with output to the standard output stream
                Serializer serializer = new Serializer();
                //StringWriter output = new StringWriter();
                //serializer.SetOutputWriter(output);
                serializer.SetOutputFile(@"D:\Develop\ehrsfm_profile\trunk\Saxon.Tests\XMLFile1.output.xml");

                // Transform the source XML and serialize the result document
                transformer.Run(serializer);

                //Console.WriteLine(output.ToString());
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var error in compiler.ErrorList)
                {
                    sb.AppendLine(error.ToString());
                }
                Assert.Fail(sb.ToString());
            }
        }
    }

    public class UserXmlResolver : XmlUrlResolver
    {
        public override object GetEntity(Uri absoluteUri, String role, Type ofObjectToReturn)
        {
            Console.WriteLine(absoluteUri + " (role=" + role + ")");
            return base.GetEntity(absoluteUri, role, ofObjectToReturn);
        }
    }
}
