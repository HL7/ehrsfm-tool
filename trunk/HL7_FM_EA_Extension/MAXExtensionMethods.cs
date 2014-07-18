using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAX_EA.MAXSchema;

namespace HL7_FM_EA_Extension
{
    public static class MAXExtensionMethods
    {
        // Extend ObjetType with a method that gets the value or null of a tagged value and checks if it exists
        public static string TagValue(this ObjectType objectType, string key, string defaultValue = null)
        {
            if (objectType.tag != null)
            {
                TagType tag = objectType.tag.FirstOrDefault(t => key.Equals(t.name));
                if (tag != null)
                {
                    return tag.value;
                }
            }
            return defaultValue;
        }
    }
}
