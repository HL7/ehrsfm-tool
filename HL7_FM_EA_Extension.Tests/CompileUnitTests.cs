using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using MAX_EA.MAXSchema;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL7_FM_EA_Extension.Tests
{
    [TestClass]
    public class CompileUnitTests
    {
        [TestMethod]
        public void TestCompileOV1Only()
        {
            string fileNameBase = @"c:\temp\EHRS_FM_R2_N2.max.xml";
            string fileNameInstructions = @"c:\temp\Compiler Instructions OV1Only.max.xml";
            string fileNameOutput = @"c:\temp\Compiled Profile OV1Only.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameInstructions, fileNameOutput);
        }

        [TestMethod]
        public void TestCompile()
        {
            string fileNameBase = @"c:\temp\EHRS_FM_R2_N2.max.xml";
            string fileNameInstructions = @"c:\temp\Compiler Instructions.max.xml";
            string fileNameOutput = @"c:\temp\Compiled Profile.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameInstructions, fileNameOutput);
        }
    }
}
