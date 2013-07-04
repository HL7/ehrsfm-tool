using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Xsl;
using System.Xml;

namespace SchematronTry.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create and load the transform with script execution enabled.
            XslCompiledTransform transform = new XslCompiledTransform();
            XsltSettings settings = new XsltSettings();
            settings.EnableScript = true;
            XmlUrlResolver resolver = new XmlUrlResolver();

            // transform the Schematron to a XSL
            transform.Load(@"D:\VisualStudio Projects\HL7\SchematronTry.ConsoleApplication\iso-schematron-xslt1\iso_svrl_for_xslt1.xsl", settings, resolver);
            transform.Transform(@"D:\VisualStudio Projects\HL7\SchematronTry.ConsoleApplication\Test.sch",
                @"D:\VisualStudio Projects\HL7\SchematronTry.ConsoleApplication\Test.sch.xsl");

            // now execute the Schematron XSL
            const string fm_max_file = @"D:\My Documents\__R4C\__WIP\HL7 EHR-S FM Tooling (2012-aug)\EHRS_FM_R2 (import v2.2) generated-ehrs-fm-20121115-full2.max.xml";
            //const string fm_max_file = @"D:\VisualStudio Projects\HL7\SchematronTry.ConsoleApplication\fm-max.xml";

            transform.Load(@"D:\VisualStudio Projects\HL7\SchematronTry.ConsoleApplication\Test.sch.xsl", settings, resolver);
            transform.Transform(fm_max_file, @"D:\VisualStudio Projects\HL7\SchematronTry.ConsoleApplication\fm-max.xml-output");
        }
    }
}
