using System;
using System.Collections.Generic;
using System.Linq;
using MAX_EA.MAXSchema;
using System.Xml.Serialization;
using System.IO;

namespace MAX_EA
{
    public class MAXDiff
    {
        public void diff(string fileName1, string fileName2)
        {
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
                    }

                    foreach (ObjectType maxObj2 in model2.objects)
                    {
                        string id = maxObj2.id;
                        if (!objects1.ContainsKey(id))
                        {
                            Console.WriteLine("[{0}] NEW object name \"{1}\"", id, maxObj2.name);
                        }
                        else
                        {
                            ObjectType maxObj1 = objects1[maxObj2.id];
                            if (maxObj1.name != maxObj2.name)
                            {
                                Console.WriteLine("[{0}] name changed \"{1}\" > \"{2}\"", id, maxObj1.name, maxObj2.name);
                            }
                            if (maxObj1.alias != maxObj2.alias)
                            {
                                Console.WriteLine("[{0}] alias changed \"{1}\" > \"{2}\"", id, maxObj1.alias, maxObj2.alias);
                            }
                            if (!maxObj1.type.Equals(maxObj2.type))
                            {
                                Console.WriteLine("[{0}] type changed {1} > {2}", id, maxObj1.type, maxObj2.type);
                            }
                            if (maxObj1.stereotype != maxObj2.stereotype)
                            {
                                Console.WriteLine("[{0}] stereotype changed \"{1}\" > \"{2}\"", id, maxObj1.stereotype, maxObj2.stereotype);
                            }
                            if (!notesIsEqual(maxObj1, maxObj2))
                            {
                                string notes1 = maxObj1.notes.Text[0].Replace("\n", "\\n");
                                string notes2 = maxObj2.notes.Text[0].Replace("\n", "\\n");
                                Console.WriteLine("[{0}] notes changed\n\t< \"{1}\"\n\t> \"{2}\"", id, notes1, notes2);
                            }
                            foreach(TagType tag in maxObj2.tag)
                            {
                                if (!tagIsEqual(tag.name, maxObj1, maxObj2))
                                {
                                    Console.WriteLine("[{0}] tag {1} changed\n\t> \"{2}\"", id, tag.name, tag.value);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void diff_FM(string fileName1, string fileName2)
        {
            // Add parameter to ignore modified or order?

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
