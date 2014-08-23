using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MAX_EA.ClassLibrary.Tests
{
    [TestClass]
    public class DiffUnitTests
    {
        [TestMethod]
        public void TestDiff()
        {
            string fileName1 = @"c:\temp\EHRS_FM_R2_N1.max.xml";
            string fileName2 = @"c:\temp\EHRS_FM_R2_N2.max.xml";

            MAXDiff diff = new MAXDiff();
            diff.diff(fileName1, fileName2);
        }

        [TestMethod]
        public void TestDiffPriRiSta()
        {
            string fileName1 = @"C:\Eclipse Workspace\NieuwEPD\PriRiSta\PriRiSta.max.out.out.xml";
            string fileName2 = @"C:\Eclipse Workspace\NieuwEPD\PriRiSta\PriRiSta Sonja.max.xml";

            MAXDiff diff = new MAXDiff();
            diff.diff(fileName1, fileName2);
        }

        [TestMethod]
        public void TestDiff_FM()
        {
            string fileName1 = @"c:\temp\EHRS_FM_R2_N2.max.xml";
            string fileName2 = @"C:\temp\EHRS_FM_R2 January 2014.max.xml";

            MAXDiff diff = new MAXDiff();
            diff.diff_FM(fileName1, fileName2);
        }

        [TestMethod]
        public void TestDiff_FM_MU()
        {
            string fileName1 = @"C:\Eclipse Workspace\ehrsfm_profile\publication\source\MU FP compiled-profile 7162014-3.max.xml";
            string fileName2 = @"C:\Eclipse Workspace\ehrsfm_profile\publication\source\ehr-mu-compiled-fp.max";

            MAXDiff diff = new MAXDiff();
            diff.diff_FM(fileName1, fileName2);
        }
    }
}
