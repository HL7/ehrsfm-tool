using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HL7_FM_EA_Extension.Tests
{
    [TestClass]
    public class FormsUnitTests
    {
        [TestMethod]
        public void TestMergeProfilesForm()
        {
            MergeProfilesForm form = new MergeProfilesForm();
            form.PopulateAndShow();
        }
    }
}
