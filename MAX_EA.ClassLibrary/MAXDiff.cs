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
                            if (maxObj1.notes.Text[0] != maxObj2.notes.Text[0])
                            {
                                string notes1 = maxObj1.notes.Text[0].Replace("\n", "\\n");
                                string notes2 = maxObj2.notes.Text[0].Replace("\n", "\\n");
                                Console.WriteLine("[{0}] notes changed\n\t< \"{1}\"\n\t> \"{2}\"", id, notes1, notes2);
                            }
                        }
                    }
                }
            }
        }
    }
}
