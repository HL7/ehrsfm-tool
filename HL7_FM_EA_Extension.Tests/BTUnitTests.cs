using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL7_FM_EA_Extension.Tests
{
    [TestClass]
    public class BTUnitTests
    {
        [TestMethod]
        public void TestCompileBT1415()
        {
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2 January 2014_Final.max.xml";
            string fileNameDefinition = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\Testing Profile Definition BT 14 & 15.max.xml";
            string fileNameOutput = @"c:\temp\Testing Compiled Profile BT 14 & 15.max";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }

        [TestMethod]
        public void TestCompileBT1819()
        {
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2 January 2014_Final.max.xml";
            string fileNameDefinition = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\Testing Profile Definition BT 18 & 19.max.xml";
            string fileNameOutput = @"c:\temp\Testing Compiled Profile BT 18 & 19.max";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }

        [TestMethod]
        public void TestCompileBT2021()
        {
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2 January 2014_Final.max.xml";
            string fileNameDefinition = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\Testing Profile Definition BT 20 & 21.max.xml";
            string fileNameOutput = @"c:\temp\Testing Compiled Profile BT 20 & 21.max";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }
    }
}
