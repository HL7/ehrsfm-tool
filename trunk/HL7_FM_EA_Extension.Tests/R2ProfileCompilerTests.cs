using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
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
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2 January 2014_Final.max.xml";
            string fileNameDefinition = @"c:\temp\MU Profile Definition 7162014.max.xml";
            string fileNameOutput = @"c:\temp\MU FP Compiled.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
        }

        [TestMethod]
        public void TestCompileItalyFP()
        {
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2 January 2014_Final.max.xml";
            string fileNameDefinition = @"C:\My Documents\__R4C 2014\EHR-S FM Italy FP (2014-jun)\Italy FP Profile Definition.max.xml";
            string fileNameOutput = @"c:\temp\Italy FP Compiled.max.xml";
            new R2ProfileCompiler().Compile(fileNameBase, fileNameDefinition, fileNameOutput);
            
        }

        [TestMethod]
        public void TestPreCompileMAXFile()
        {
            string fileNameBase = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2_N2.max.xml";
            string fileNameDefinition = @"c:\temp\MU Profile Definition 7162014.max.xml"; 
            //string fileNameDefinition = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\Compiler Instructions.max.xml";

            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            StreamReader streamBase = new StreamReader(fileNameBase);
            StreamReader streamDef = new StreamReader(fileNameDefinition);
            var modelBase = (ModelType)serializer.Deserialize(streamBase);
            var modelDef = (ModelType)serializer.Deserialize(streamDef);
            List<ObjectType> objects = new List<ObjectType>();
            objects.AddRange(modelBase.objects);
            objects.AddRange(modelDef.objects);
            List<RelationshipType> relationships = new List<RelationshipType>();
            relationships.AddRange(modelBase.relationships);
            relationships.AddRange(modelDef.relationships);
            modelBase.objects = objects.ToArray();
            modelBase.relationships = relationships.ToArray();
            StreamWriter writer = new StreamWriter(@"c:\temp\pre-compile.max");
            using (writer)
            {
                serializer.Serialize(writer, modelBase);
            }
        }
    }
}
