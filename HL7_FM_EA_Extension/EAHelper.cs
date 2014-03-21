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

        public static void updateTaggedValue(EA.Element element, string name, string value, string notes=null)
        {
            if (!string.IsNullOrEmpty(value))
            {
                EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(name);
                if (tv == null)
                {
                    tv = (EA.TaggedValue)element.TaggedValues.AddNew(name, "TaggedValue");
                }
                tv.Value = value;
                if (notes != null)
                {
                    tv.Notes = notes;
                }
                tv.Update();
            }
        }

        public static string getTaggedValue(EA.Element element, string name, string defaultValue = "")
        {
            EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(name);
            if (tv != null)
            {
                return tv.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        public static string getTaggedValueNotes(EA.Element element, string name, string defaultValue = "")
        {
            EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(name);
            if (tv != null)
            {
                return tv.Notes;
            }
            else
            {
                return defaultValue;
            }
        }

        public static void removeTaggedValue(EA.Element element, string name)
        {
            for (short index = 0; index < element.TaggedValues.Count; index++)
            {
                EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetAt(index);
                if (name.Equals(tv.Name))
                {
                    element.TaggedValues.Delete(index);
                    element.TaggedValues.Refresh();
                    return;
                }
            }
        }
    }
}
