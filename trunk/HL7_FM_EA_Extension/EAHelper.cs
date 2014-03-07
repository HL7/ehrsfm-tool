using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7_FM_EA_Extension
{
    class EAHelper
    {
        public static void LogMessage(EA.Repository repository, string message, int id = 0)
        {
            string timestamp = DateTime.Now.ToString();
            repository.WriteOutput(Properties.Resources.OUTPUT_TAB_HL7_FM, string.Format("@{0} {1}", timestamp, message), id);
        }
    }
}
