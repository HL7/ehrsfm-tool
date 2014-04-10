using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MAX_EA_Extension.ClassLibrary.Tests
{
    [TestClass]
    public class ValidatorUnitTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            string xml_file = @"C:\Temp\7 Informatievoorziening v1.0.3 (Merged).max.xml";
            string sch_file = @"C:\Eclipse Workspace\NieuwEPD\9.01 Schematron\901 Validation Rules.sch";
            string svrl_file = @"C:\Eclipse Workspace\NieuwEPD\9.01 Schematron\svrl_output.xml";

            MAXValidator validator = new MAXValidator();
            validator.validate(null, xml_file, sch_file, svrl_file);
        }
    }
}
