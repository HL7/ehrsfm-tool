using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HL7_MAX.MAXSchema;

namespace HL7_FM_CLI
{
    public static class MAXExtensionMethods
    {
        public static string GetNotes(this ObjectType objectType)
        {
            if (objectType.notes != null && objectType.notes.Text != null && objectType.notes.Text.Any())
            {
                return objectType.notes.Text[0];
            }
            else
            {
                return null;
            }
        }

        public static void SetNotes(this ObjectType objectType, string value)
        {
            objectType.notes = new MarkupType() { Text = new string[] { value } };
        }

        // Extend ObjectType with a method that gets the value or null of a tagged value and checks if it exists
        public static string GetTagValue(this ObjectType objectType, string key, string defaultValue = null)
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

        // Extend ObjectType with a method that gets the value or null of a tagged value and checks if it exists
        public static string GetTagValueNotes(this ObjectType objectType, string key, string defaultValue = null)
        {
            if (objectType.tag != null)
            {
                TagType tag = objectType.tag.FirstOrDefault(t => key.Equals(t.name));
                if (tag != null && tag.Text != null && tag.Text.Any())
                {
                    return tag.Text[0];
                }
            }
            return defaultValue;
        }

        // Extend ObjectType with a method that sets the value of a tagged value and checks if it exists
        public static void SetTagValue(this ObjectType objectType, string key, string value, string notes=null)
        {
            TagType tag = null;
            if (objectType.tag != null)
            {
                tag = objectType.tag.FirstOrDefault(t => key.Equals(t.name));
            }
            if (tag == null)
            {
                tag = new TagType {name = key};
                if (objectType.tag != null)
                {
                    List<TagType> tags = objectType.tag.ToList();
                    tags.Add(tag);
                    objectType.tag = tags.ToArray();
                }
                else
                {
                    objectType.tag = new TagType[] { tag };
                }
            }
            tag.value = value;
            if (notes != null)
            {
                tag.Text = new string[] { notes };
            }
            else
            {
                tag.Text = null;
            }
        }

        // Extend ObjectType with a method that removes a tagged value
        public static void DeleteTagValue(this ObjectType objectType, string key)
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
