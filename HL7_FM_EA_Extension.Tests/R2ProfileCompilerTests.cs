using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MAX_EA.MAXSchema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL7_FM_EA_Extension.Tests
{
    [TestClass]
    public class R2ProfileCompilerTests
    {
        [TestMethod]
        public void TestCompileOV1Only()
        {
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\\EHRS_FM_R2_N2.max.xml";
            string fileNameDefinition = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\\OV1 Only FP.max.xml";
            string fileNameOutput = @"c:\temp\OV1 Only FP Compiled.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }

        [TestMethod]
        public void TestCompile()
        {
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2_N2.max.xml";
            string fileNameDefinition = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\Compiler Instructions.max.xml";
            string fileNameOutput = @"c:\temp\Compiled Profile.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }

        [TestMethod]
        public void TestCompileMUFP()
        {
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\\EHRS_FM_R2_N2.max.xml";
            string fileNameDefinition = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\\MU Profile Definition.max.xml";
            string fileNameOutput = @"c:\temp\MU FP Compiled.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }
    }
}
