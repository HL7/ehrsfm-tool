using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL7_FM_EA_Extension.Tests
{
    [TestClass]
    public class R2ModelV2BaseTest
    {
        [TestMethod]
        public void TestR2CriterionWithoutDefaults()
        {
            var criterion = new R2ModelV2.Base.R2Criterion
                                {
                                    FunctionId = "CP.1.2",
                                    CriterionSeqNo = 1,
                                    Text = "The system SHALL bladibla",
                                    Row = 1,
                                    Conditional = false,
                                    Dependent = false,
                                    Optionality = "SHALL"
                                };

            Assert.AreEqual("CP.1.2", criterion.FunctionId);
            Assert.AreEqual(1, criterion.CriterionSeqNo);
            Assert.AreEqual("The system SHALL bladibla", criterion.Text);
            Assert.AreEqual(1, criterion.Row);
            Assert.AreEqual(false, criterion.Conditional);
            Assert.AreEqual(false, criterion.Dependent);
            Assert.AreEqual("SHALL", criterion.Optionality);
            Assert.AreEqual(R2ModelV2.Base.R2Const.EmptyPriority, criterion.Priority);
            Assert.IsNull(criterion.RefId);
        }

        [TestMethod]
        public void TestR2CriterionWithDefaults()
        {
            var defaults = new R2ModelV2.Base.R2Criterion
            {
                FunctionId = "CP.1.2",
                CriterionSeqNo = 1,
                Text = "The system SHALL bladibla",
                Row = 1,
                Conditional = false,
                Dependent = false,
                Optionality = "SHALL"
            };
            var criterion = new R2ModelV2.Base.R2Criterion();
            criterion.BaseElement = defaults;

            Assert.AreEqual("CP.1.2", criterion.FunctionId);
            Assert.AreEqual(1, criterion.CriterionSeqNo);
            Assert.AreEqual("The system SHALL bladibla", criterion.Text);
            Assert.AreEqual(1, criterion.Row);
            Assert.AreEqual(false, criterion.Conditional);
            Assert.AreEqual(false, criterion.Dependent);
            Assert.AreEqual("SHALL", criterion.Optionality);
            Assert.AreEqual(R2ModelV2.Base.R2Const.EmptyPriority, criterion.Priority);
            Assert.IsNull(criterion.RefId);
        }

        [TestMethod]
        public void TestR2CriterionWithDefaultsOverruled()
        {
            var defaults = new R2ModelV2.Base.R2Criterion
            {
                FunctionId = "CP.1.2",
                CriterionSeqNo = 1,
                Text = "The system SHALL bladibla",
                Row = 1,
                Conditional = false,
                Dependent = false,
                Optionality = "SHALL"
            };
            var criterion = new R2ModelV2.Base.R2Criterion
                                {
                                    BaseElement = defaults,
                                    FunctionId = "CP.1.3",
                                    Text = "Changed",
                                    Row = 10,
                                    Optionality = "SHOULD",
                                    Priority = "EN"
                                };
            //criterion.IsCompilerInstruction = true;


            Assert.AreEqual("CP.1.3", criterion.FunctionId);
            Assert.AreEqual(1, criterion.CriterionSeqNo);
            Assert.AreEqual("Changed", criterion.Text);
            Assert.AreEqual(10, criterion.Row);
            Assert.AreEqual(false, criterion.Conditional);
            Assert.AreEqual(false, criterion.Dependent);
            Assert.AreEqual("SHOULD", criterion.Optionality);
            Assert.AreEqual("EN", criterion.Priority);
            Assert.IsNull(criterion.RefId);
        }

        [TestMethod]
        public void TestR2CriterionWithDefaultsOverruledAndBack()
        {
            var defaults = new R2ModelV2.Base.R2Criterion
            {
                FunctionId = "CP.1.2",
                CriterionSeqNo = 1,
                Text = "The system SHALL bladibla",
                Row = 1,
                Conditional = false,
                Dependent = false,
                Optionality = "SHALL"
            };
            var criterion = new R2ModelV2.Base.R2Criterion();
            criterion.BaseElement = defaults;
            //criterion.IsCompilerInstruction = true;

            // overrule and erase to see if fallback to defaults works
            criterion.FunctionId = "CP.1.3";
            criterion.FunctionId = null;
            criterion.Text = "Changed";
            criterion.Text = "";
            criterion.Row = 10;
            criterion.Row = 1;
            criterion.Optionality = "SHOULD";
            criterion.Optionality = "";
            criterion.Priority = "EN";
            criterion.Priority = "";

            Assert.AreEqual("CP.1.2", criterion.FunctionId);
            Assert.AreEqual(1, criterion.CriterionSeqNo);
            Assert.AreEqual("The system SHALL bladibla", criterion.Text);
            Assert.AreEqual(1, criterion.Row);
            Assert.AreEqual(false, criterion.Conditional);
            Assert.AreEqual(false, criterion.Dependent);
            Assert.AreEqual("SHALL", criterion.Optionality);
            Assert.AreEqual("", criterion.Priority);
            Assert.IsNull(criterion.RefId);
        }

        [TestMethod]
        public void TestR2CriterionRowAndDefault()
        {
            var defaults = new R2ModelV2.Base.R2Criterion
                               {
                                   Row = 1,
                               };
            var criterion = new R2ModelV2.Base.R2Criterion();
            Assert.AreEqual(0, criterion.Row);
            Assert.AreEqual(1, defaults.Row);
            criterion.BaseElement = defaults;
            Assert.AreEqual(1, criterion.Row);
        }
    }
}
