using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAX_EA.MAXSchema;

namespace HL7_FM_EA_Extension
{
    public static class MAXExtensionMethods
    {
        // Extend ObjectType with a method that gets the value or null of a tagged value and checks if it exists
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

        // Extend ObjectType with a method that sets the value of a tagged value and checks if it exists
        public static void SetTagValue(this ObjectType objectType, string key, string value)
        {
            if (objectType.tag != null)
            {
                TagType tag = objectType.tag.FirstOrDefault(t => key.Equals(t.name));
                if (tag != null)
                {
                    tag.value = value;
                }
                else
                {
                    List<TagType> tags = objectType.tag.ToList();
                    tags.Add(new TagType { name = key, value = value });
                    objectType.tag = tags.ToArray();
                }
            }
            else
            {
                objectType.tag = new TagType[] { new TagType {name = key, value = value} };
            }
        }

        // Extend ObjectType with a method that removes a tagged value
        public static void RemoveTagValue(this ObjectType objectType, string key)
        {
            if (objectType.tag != null)
            {
                TagType tag = objectType.tag.FirstOrDefault(t => key.Equals(t.name));
                if (tag != null)
                {
                    List<TagType> tags = objectType.tag.ToList();
                    tags.Remove(tag);
                    objectType.tag = tags.ToArray();
                }
            }
        }
    }
}
