using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7_FM_EA_Extension
{
    class R2Const
    {
        public const string ST_FM = "HL7-FM";
        public const string ST_FM_PROFILEDEFINITION = "HL7-FM-ProfileDefinition";
        public const string ST_BASEMODEL = "use";
        public const string ST_TARGETPROFILE = "create";
        public const string ST_FM_PROFILE = "HL7-FM-Profile";
        public const string ST_SECTION = "Section";
        public const string ST_HEADER = "Header";
        public const string ST_FUNCTION = "Function";
        public const string ST_CRITERIA = "Criteria";
        public const string ST_CONSEQUENCELINK = "ConsequenceLink";
        public const string ST_SEEALSO = "SeeAlso";

        public const bool LOCK_ELEMENTS = false; // TODO: change to true when ballot done?
    }
}
