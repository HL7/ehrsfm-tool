using System;
using System.Collections.Generic;
using System.Linq;
using HL7_MAX.MAXSchema;
using System.Xml.Serialization;
using System.IO;

namespace HL7_MAX
{
    public class MAXDiff
    {
        public void diff(string fileName1, string fileName2)
        {
            bool truncateNotes = true;
            bool showFullName2 = true;
            bool ignoreNotes = true;
            // Add parameter to ignore modified or order?

            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            StreamReader stream1 = new StreamReader(fileName1);
            StreamReader stream2 = new StreamReader(fileName2);
            // make sure file gets closed
            using (stream1)
            {
                using (stream2)
                {
                    ModelType model1 = (ModelType)serializer.Deserialize(stream1);
                    ModelType model2 = (ModelType)serializer.Deserialize(stream2);

                    var objects1 = new Dictionary<string, ObjectType>();
                    foreach (ObjectType maxObj1 in model1.objects)
                    {
                        objects1[maxObj1.id] = maxObj1;
                        if (model2.objects.SingleOrDefault(o => o.id == maxObj1.id) == null)
                        {
                            Console.WriteLine("{0} DELETED object @name \"{1}\"", maxObj1.id, maxObj1.name);
                        }
                    }

                    foreach (ObjectType maxObj2 in model2.objects)
                    {
                        string id = maxObj2.id;
                        if (showFullName2)
                        {
                            string fullName = maxObj2.name;
                            ObjectType currentObj = maxObj2;
                            do
                            {
                                currentObj = model2.objects.SingleOrDefault(o => o.id == currentObj.parentId);
                                fullName = currentObj != null ? currentObj.name + "/" + fullName : fullName;
                            }
                            while (currentObj != null);
                            id = string.Format("{0}@{1}", fullName, maxObj2.id);
                        }
                        if (!objects1.ContainsKey(maxObj2.id))
                        {
                            if(showFullName2) Console.WriteLine("{0} NEW object", id);
                            else Console.WriteLine("{0} NEW object @name \"{1}\"", id, maxObj2.name);
                        }
                        else
                        {
                            ObjectType maxObj1 = objects1[maxObj2.id];
                            if (!maxObj1.name.Equals(maxObj2.name))
                            {
                                Console.WriteLine("{0} @name\n< {1}\n> {2}", id, maxObj1.name, maxObj2.name);
                            }
                            if (!maxObj1.alias.Equals(maxObj2.alias))
                            {
                                Console.WriteLine("{0} @alias\n< {1}\n> {2}", id, maxObj1.alias, maxObj2.alias);
                            }
                            if (!maxObj1.type.Equals(maxObj2.type))
                            {
                                Console.WriteLine("{0} @type\n< {1}\n> {2}", id, maxObj1.type, maxObj2.type);
                            }
                            if (!maxObj1.stereotype.Equals(maxObj2.stereotype))
                            {
                                Console.WriteLine("{0} @stereotype\n< {1}\n> {2}", id, maxObj1.stereotype, maxObj2.stereotype);
                            }
                            if (!ignoreNotes && !notesIsEqual(maxObj1, maxObj2))
                            {
                                string notes1 = maxObj1.notes != null ? maxObj1.notes.Text[0].Replace("\n", " ") : null;
                                string notes2 = maxObj2.notes != null ? maxObj2.notes.Text[0].Replace("\n", " ") : null;
                                if (truncateNotes)
                                {
                                    if (notes1 != null && notes1.Length > 30) notes1 = notes1.Substring(0, 30) + "...";
                                    if (notes2 != null && notes2.Length > 30) notes2 = notes2.Substring(0, 30) + "...";
                                }
                                Console.WriteLine("{0} @notes\n< {1}\n> {2}", id, notes1, notes2);
                            }
                            if (maxObj2.tag != null)
                            {
                                foreach (TagType tag in maxObj2.tag)
                                {
                                    if (!tagIsEqual(tag.name, maxObj1, maxObj2))
                                    {
                                        Console.WriteLine("{0} @tag {1}\n> {2}", id, tag.name, tag.value);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void diff_DCM_NLCM(string fileName1, string fileName2)
        {
            XmlSerializer serializer = new XmlSerializer(typeof (ModelType));
            StreamReader stream1 = new StreamReader(fileName1);
            StreamReader stream2 = new StreamReader(fileName2);
            // make sure file gets closed
            using (stream1)
            {
                using (stream2)
                {
                    ModelType model1 = (ModelType)serializer.Deserialize(stream1);
                    ModelType model2 = (ModelType)serializer.Deserialize(stream2);

                    var concepts1 = new Dictionary<string, ObjectType>();
                    foreach (ObjectType conceptObj in model1.objects.Where(o => o.tag != null && o.tag.Count(t => t.name == "DCM::ConceptId" || (t.name == "DCM::DefinitionCode" && t.value.StartsWith("NL-CM:"))) == 1))
                    {
                        string conceptId = conceptObj.tag.Single(t => t.name == "DCM::ConceptId" || (t.name == "DCM::DefinitionCode" && t.value.StartsWith("NL-CM:"))).value;
                        concepts1[conceptId] = conceptObj;
                    }

                    var concepts2 = new Dictionary<string, ObjectType>();
                    foreach (ObjectType conceptObj in model2.objects.Where(o => o.tag != null && o.tag.Count(t => t.name == "DCM::ConceptId" || (t.name == "DCM::DefinitionCode" && t.value.StartsWith("NL-CM:"))) == 1))
                    {
                        string conceptId = conceptObj.tag.Single(t => t.name == "DCM::ConceptId" || (t.name == "DCM::DefinitionCode" && t.value.StartsWith("NL-CM:"))).value;
                        concepts2[conceptId] = conceptObj;
                    }

                    foreach (string conceptId1 in concepts1.Keys)
                    {
                        if (!concepts2.ContainsKey(conceptId1))
                        {
                            Console.WriteLine("- {0} {1}", conceptId1, concepts1[conceptId1].name);
                        }
                    }
                    foreach (string conceptId2 in concepts2.Keys)
                    {
                        if (!concepts1.ContainsKey(conceptId2))
                        {
                            Console.WriteLine("+ {0} {1}", conceptId2, concepts2[conceptId2].name);
                        }
                    }
                }
            }
        }

        public void diff_FM(string fileName1, string fileName2)
        {
            XmlSerializer serializer = new XmlSerializer(typeof (ModelType));
            StreamReader stream1 = new StreamReader(fileName1);
            StreamReader stream2 = new StreamReader(fileName2);
            // make sure file gets closed
            using (stream1)
            {
                using (stream2)
                {
                    ModelType model1 = (ModelType) serializer.Deserialize(stream1);
                    ModelType model2 = (ModelType) serializer.Deserialize(stream2);

                    List<string> changes = new List<string>(); ;
                    var objects1 = model1.objects.ToDictionary(t => t.id, t => t);
                    foreach (ObjectType maxObj2 in model2.objects)
                    {
                        changes.Clear();
                        string id = maxObj2.id;
                        if (!objects1.ContainsKey(id))
                        {
                            changes.Add("NEW");
                            displayObject(maxObj2, changes);
                        }
                        else
                        {
                            ObjectType maxObj1 = objects1[maxObj2.id];
                            objects1.Remove(id);

                            if (maxObj1.name != maxObj2.name)
                            {
                                changes.Add("name");
                            }
                            if (!notesIsEqual(maxObj1, maxObj2))
                            {
                                changes.Add("notes");
                            }
                            // tag Optionality, Dependent, Conditional
                            if (!tagIsEqual("Optionality", maxObj1, maxObj2))
                            {
                                changes.Add("Optionality");
                            }
                            if (!tagIsEqual("Dependent", maxObj1, maxObj2))
                            {
                                changes.Add("Dependent");
                            }
                            if (!tagIsEqual("Conditional", maxObj1, maxObj2))
                            {
                                changes.Add("Conditional");
                            }
                            
                            // split notes

                            if (changes.Count > 0)
                            {
                                displayObject(maxObj2, changes);
                            }
                        }
                    }
                    // all objects that are left in objects1 were deleted in objects2
                    changes.Clear();
                    changes.Add("DELETED");
                    foreach (ObjectType maxObj in objects1.Values)
                    {
                        displayObject(maxObj, changes);
                    }
                }
            }
        }

        private bool tagIsEqual(string tagName, ObjectType maxObj1, ObjectType maxObj2)
        {
            string value1 = string.Empty;
            if (maxObj1.tag != null)
            {
                TagType tag1 = maxObj1.tag.FirstOrDefault(t => tagName.Equals(t.name));
                if (tag1 != null)
                {
                    value1 = tag1.value;
                }
            }
            string value2 = string.Empty;
            if (maxObj2.tag != null)
            {
                TagType tag2 = maxObj2.tag.FirstOrDefault(t => tagName.Equals(t.name));
                if (tag2 != null)
                {
                    value2 = tag2.value;
                }
            }
            return value1.Equals(value2);
        }

        private bool notesIsEqual(ObjectType maxObj1, ObjectType maxObj2)
        {
            string value1 = string.Empty;
            if (maxObj1.notes != null)
            {
                if (maxObj1.notes.Text != null)
                {
                    value1 = maxObj1.notes.Text[0];
                }
            }
            string value2 = string.Empty;
            if (maxObj2.notes != null)
            {
                if (maxObj2.notes.Text != null)
                {
                    value2 = maxObj2.notes.Text[0];
                }
            }
            return value1.Equals(value2);
        }

        private void displayObject(ObjectType maxObj, List<string> changes)
        {
            Console.WriteLine("<<{0}>> {1} ({2})", maxObj.stereotype, maxObj.name, string.Join(",", changes.ToArray()));
            Console.WriteLine(maxObj.notes.Text[0]);
            Console.WriteLine();
        }
    }
}
