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
    public class R2ModelV2MAXTests
    {
        [TestMethod]
        public void TestR2SectionFactoryCreate()
        {
            ObjectType objectType = new ObjectType
            {
                id = "5",
                name = "Overarching",
                alias = "OV",
                notes = new MarkupType() { Text = new string[] { "$OV$overview$AC$actors$EX$example" } },
                stereotype = "Section",
                type = ObjectTypeEnum.Package,
                parentId = "4"
            };
            R2ModelV2.Base.R2Section modelElement = (R2ModelV2.Base.R2Section)R2ModelV2.MAX.Factory.Create(objectType);

            Assert.IsNotNull(modelElement);
            Assert.AreEqual("Overarching", modelElement.Name);
            Assert.AreEqual("OV", modelElement.GetExtId());
            Assert.AreEqual("OV", modelElement.SectionId);
            Assert.AreEqual("Section", modelElement.Stereotype);
            Assert.AreEqual("overview", modelElement.Overview);
            Assert.AreEqual("", modelElement.LastModified);
            Assert.AreEqual("", modelElement.Priority);
            Assert.AreEqual("example", modelElement.Example);
            Assert.AreEqual("actors", modelElement.Actors);
            Assert.AreEqual("", modelElement.ChangeNote);
            Assert.IsFalse(modelElement.IsCompilerInstruction);
            Assert.AreEqual("Overarching", modelElement.Path);
            Assert.IsNull(modelElement.RefId);
        }

        [TestMethod]
        public void TestR2HeaderFactoryCreate()
        {
            ObjectType objectType = new ObjectType
            {
                id = "5",
                name = "OV.1 Header",
                alias = "OV.1",
                notes =
                    new MarkupType() {Text = new string[] {"$ST$statement$DE$description"}},
                stereotype = "Header",
                type = ObjectTypeEnum.Feature,
                parentId = "4"
            };
            R2ModelV2.Base.R2Function modelElement =
                (R2ModelV2.Base.R2Function) R2ModelV2.MAX.Factory.Create(objectType);

            Assert.IsNotNull(modelElement);
            Assert.AreEqual("OV.1 Header", modelElement.Name);
            Assert.AreEqual("OV.1", modelElement.GetExtId());
            Assert.AreEqual("OV.1", modelElement.FunctionId);
            Assert.AreEqual("Header", modelElement.Stereotype);
            Assert.AreEqual("", modelElement.Priority);
            Assert.AreEqual("statement", modelElement.Statement);
            Assert.AreEqual("description", modelElement.Description);
            Assert.AreEqual("", modelElement.ChangeNote);
            Assert.IsFalse(modelElement.IsCompilerInstruction);
            Assert.AreEqual("OV.1 Header", modelElement.Path);
            Assert.IsNull(modelElement.RefId);
        }

        [TestMethod]
        public void TestR2FunctionFactoryCreate()
        {
            ObjectType objectType = new ObjectType
            {
                id = "5",
                name = "OV.1 Function",
                alias = "OV.1",
                notes =
                    new MarkupType() { Text = new string[] { "$ST$statement$DE$description" } },
                stereotype = "Function",
                type = ObjectTypeEnum.Feature,
                parentId = "4"
            };
            R2ModelV2.Base.R2Function modelElement =
                (R2ModelV2.Base.R2Function)R2ModelV2.MAX.Factory.Create(objectType);

            Assert.IsNotNull(modelElement);
            Assert.AreEqual("OV.1 Function", modelElement.Name);
            Assert.AreEqual("OV.1", modelElement.GetExtId());
            Assert.AreEqual("OV.1", modelElement.FunctionId);
            Assert.AreEqual("Function", modelElement.Stereotype);
            Assert.AreEqual("", modelElement.Priority);
            Assert.AreEqual("statement", modelElement.Statement);
            Assert.AreEqual("description", modelElement.Description);
            Assert.AreEqual("", modelElement.ChangeNote);
            Assert.IsFalse(modelElement.IsCompilerInstruction);
            Assert.AreEqual("OV.1 Function", modelElement.Path);
            Assert.IsNull(modelElement.RefId);
        }

        [TestMethod]
        public void TestR2CriterionFactoryCreate()
        {
            ObjectType objectType = new ObjectType
            {
                id = "5",
                name = "OV.1#01",
                alias = "OV.1#01",
                notes =
                    new MarkupType() { Text = new string[] { "The system SHALL dididi" } },
                stereotype = "Criteria",
                type = ObjectTypeEnum.Feature,
                parentId = "4"
            };
            R2ModelV2.Base.R2Criterion modelElement =
                (R2ModelV2.Base.R2Criterion)R2ModelV2.MAX.Factory.Create(objectType);

            Assert.IsNotNull(modelElement);
            Assert.AreEqual("OV.1#01", modelElement.GetExtId());
            Assert.AreEqual("OV.1#01", modelElement.Name);
            Assert.AreEqual("OV.1", modelElement.FunctionId);
            Assert.AreEqual(1, modelElement.CriterionSeqNo);
            Assert.AreEqual("Criteria", modelElement.Stereotype);
            Assert.AreEqual("", modelElement.Priority);
            Assert.AreEqual("The system SHALL dididi", modelElement.Text);
            Assert.AreEqual("", modelElement.ChangeNote);
            Assert.IsFalse(modelElement.IsCompilerInstruction);
            Assert.AreEqual("OV.1#01", modelElement.Path);
            Assert.IsNull(modelElement.RefId);
        }

        [TestMethod]
        public void TestR2ModelLoad()
        {
            string maxFileName = @"C:\Eclipse Workspace\ehrsfm_profile\HL7_FM_EA_Extension.Tests\InputFiles\EHRS_FM_R2_N2.max.xml";
            R2ModelV2.Base.R2Model model = R2ModelV2.MAX.Factory.LoadModel(maxFileName, false);

            Assert.AreEqual(2680, model.children.Count);
            Assert.AreEqual(7, model.children.Count(t => "Section".Equals(t.Stereotype)));
            Assert.AreEqual(299, model.children.Count(t => "Function".Equals(t.Stereotype)));
            Assert.AreEqual(2341, model.children.Count(t => "Criteria".Equals(t.Stereotype)));
        }
    }
}
