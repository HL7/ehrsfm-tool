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
            string fileNameBase = @"c:\temp\EHRS_FM_R2_N2.max.xml";
            string fileNameDefinition = @"c:\temp\Compiler Instructions OV1Only.max.xml";
            string fileNameOutput = @"c:\temp\Compiled Profile OV1Only.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }

        [TestMethod]
        public void TestCompile()
        {
            string fileNameBase = @"c:\temp\EHRS_FM_R2_N2.max.xml";
            string fileNameDefinition = @"c:\Temp\Compiler Instructions.max.xml";
            string fileNameOutput = @"c:\temp\Compiled Profile.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }

        [TestMethod]
        public void TestCompileMUFP()
        {
            string fileNameBase = @"c:\temp\EHRS_FM_R2_N2.max.xml";
            string fileNameDefinition = @"c:\Temp\MU Profile Definition.max.xml";
            string fileNameOutput = @"c:\temp\MU FP Compiled.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }
    }
}
