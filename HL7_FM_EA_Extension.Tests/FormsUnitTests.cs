using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using HL7_FM_EA_Extension.R2ModelV2.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL7_FM_EA_Extension.Tests
{
    [TestClass]
    public class FormsUnitTests
    {
        [TestMethod]
        public void TestSectionForm()
        {
            R2Section section = new R2Section();
            SectionForm form = new SectionForm();
            form.Show(section);
        }

        [TestMethod]
        public void TestFunctionForm()
        {
            MessageBox.Show("Enter 'CP.1.4 Test' in the Name field");
            R2Function function = new R2Function();
            FunctionForm form = new FunctionForm();
            form.Show(function);
            Assert.AreEqual(R2Const.ST_FUNCTION, function.Stereotype);
            Assert.AreEqual("CP.1.4", function.FunctionId);
            Assert.AreEqual("CP.1.4", function.GetAlignId());
            Assert.AreEqual("CP.1.4", function.GetExtId());
            Assert.IsNull(function.RefId);
            Assert.IsNull(function.Id);
            Assert.AreEqual("CP.1.4 Test", function.Name);
            Assert.AreEqual("", function.Description);
            Assert.AreEqual("", function.ChangeNote);
            Assert.IsFalse(function.IsCompilerInstruction);
            Assert.IsNull(function.BaseElement);
        }

        [TestMethod]
        public void TestCIFunctionForm()
        {
            R2Function function = new R2Function() { FunctionId = "CP.1.4", Name = "CP.1.4 Test", ChangeNote = "CI ChangeNote" };
            function.IsCompilerInstruction = true;
            function.BaseElement = new R2Function();
            FunctionForm form = new FunctionForm();
            form.Show(function);
            Assert.AreEqual(R2Const.ST_FUNCTION, function.Stereotype);
            Assert.AreEqual("CP.1.4", function.FunctionId);
            Assert.AreEqual("CP.1.4", function.GetAlignId());
            Assert.AreEqual("CP.1.4", function.GetExtId());
            Assert.IsNull(function.RefId);
            Assert.IsNull(function.Id);
            Assert.AreEqual("CP.1.4 Test", function.Name);
            Assert.AreEqual("", function.Description);
            Assert.AreEqual("", function.ChangeNote);
            Assert.IsFalse(function.IsCompilerInstruction);
            Assert.IsNull(function.BaseElement);
        }

        [TestMethod]
        public void TestMergeProfilesForm()
        {
            MergeProfilesForm form = new MergeProfilesForm();
            form.PopulateAndShow();
        }

        [TestMethod]
        public void TestProfileMetadataForm()
        {
            R2ProfileDefinition profDef = new R2ModelV2.MAX.R2ProfileDefinition();
            ProfileMetadataForm form = new ProfileMetadataForm();
            form.Show(profDef);
        }
    }
}
