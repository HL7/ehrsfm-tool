using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAX_EA.MAXSchema;
using System.Xml.Serialization;
using System.IO;

namespace MAX_EA
{
    public class MAXImporter3 : Util
    {
        // Tagged value used for externalized ID
        private const string TV_MAX_ID = "MAX::ID";

        private EA.Repository Repository;
        private ProgressWindow window = new ProgressWindow();
        private Dictionary<string, EA.Element> eaElementDict = new Dictionary<string, EA.Element>();
        private Dictionary<string, EA.Package> eaPackageDict = new Dictionary<string, EA.Package>();

        public bool import(EA.Repository Repository, EA.Package selectedPackage)
        {
            string fileName = showFileDialog("Select input MAX XML file", "xml files (*.xml)|*.xml", @"D:\VisualStudio Projects\HL7\MAX_EA.ClassLibrary\Input\MAX-example.xml", true);
            if (fileName != String.Empty)
            {
                return import(Repository, selectedPackage, fileName);
            }
            else
            {
                return false;
            }
        }

        public bool import(EA.Repository Repository, EA.Package selectedPackage, string fileName)
        {
            this.Repository = Repository;
            window.Show();
            window.Refresh();

            bool issues = false;

            Repository.EnableUIUpdates = false;
            Repository.BatchAppend = true;
            Repository.EnableCache = true;

            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            StreamReader stream = new StreamReader(fileName);
            // make sure file gets closed
            using (stream)
            {
                ModelType model = (ModelType)serializer.Deserialize(stream);
                int max = 0;
                if (model.objects != null) max += model.objects.Count();
                if (model.relationships != null) max += model.relationships.Count();
                window.setup(max);

                // Don't use Linq constructs those are reeeeaaalllyyyy slow, create a helper dictionary for quick lookup
                // This construct also makes sure there will be unique elements based on the ID|MAX::ID
                eaElementDict.Clear();
                eaPackageDict.Clear();
                recurseEaPackage(selectedPackage, eaElementDict, eaPackageDict);

                // do objects
                if (model.objects != null && model.objects.Count() > 0)
                {
                    issues |= importObjects(selectedPackage, model.objects);
                }

                // now do relationships
                if (model.relationships != null && model.relationships.Count() > 0)
                {
                    issues |= importRelationships(selectedPackage, model.relationships);
                }

                // Add/update export metadata to the model
                EA.TaggedValue tvImportDate = (EA.TaggedValue)selectedPackage.Element.TaggedValues.GetByName("MAX::LastImportDate");
                if (tvImportDate == null)
                {
                    tvImportDate = (EA.TaggedValue)selectedPackage.Element.TaggedValues.AddNew("MAX::LastImportDate", "");
                }
                tvImportDate.Value = DateTime.Now.ToString();
                tvImportDate.Update();
                selectedPackage.Element.TaggedValues.Refresh();
            }
            Repository.EnableUIUpdates = true;
            Repository.BatchAppend = false;
            Repository.RefreshModelView(selectedPackage.PackageID);

            window.Close();
            return issues;
        }

        private bool importObjects(EA.Package selectedPackage, ObjectType[] objects)
        {
            bool issues = false;
            int objPos = 0, pkgPos = 0;
            foreach (ObjectType maxObj in objects)
            {
                string type = Enum.GetName(typeof(ObjectTypeEnum), maxObj.type);
                string id = maxObj.id.Trim().ToUpper();

                // first check if element already in package
                // if not create otherwise use existing and update
                string name = maxObj.name;
                EA.Element eaElement;
                if (eaElementDict.ContainsKey(id))
                {
                    eaElement = eaElementDict[id];
                    eaElement.Name = name;
                    eaElement.Type = type;
                }
                else
                {
                    string parentId = maxObj.parentId;
                    if ("Package".Equals(type))
                    {
                        EA.Package eaPackage;
                        if (parentId != null)
                        {
                            parentId = parentId.Trim().ToUpper();
                            if (eaPackageDict.ContainsKey(parentId))
                            {
                                eaPackage = (EA.Package)eaPackageDict[parentId].Packages.AddNew(name, type);
                            }
                            else
                            {
                                Repository.WriteOutput("MAX", string.Format("Parent Package(id={0}) for Package(id={1}) not created yet. To correct: adjust the order in the MAX XML file. Fallback to selected Package.", parentId, id), 0);
                                issues = true;
                                eaPackage = (EA.Package)selectedPackage.Packages.AddNew(name, type);
                            }
                        }
                        else
                        {
                            eaPackage = (EA.Package)selectedPackage.Packages.AddNew(name, type);
                        }
                        eaPackage.TreePos = pkgPos++;
                        eaPackage.Update();
                        eaPackageDict[id] = eaPackage;
                        eaElement = eaPackage.Element;
                    }
                    else
                    {
                        if (parentId != null)
                        {
                            parentId = parentId.Trim().ToUpper();
                            EA.Element parentElement;
                            if (eaElementDict.ContainsKey(parentId))
                            {
                                parentElement = eaElementDict[parentId];
                            }
                            else
                            {
                                Repository.WriteOutput("MAX", string.Format("Parent Element(id={0}) for Element(id={1}) not created yet. To correct: adjust the order in the MAX XML file. Fallback to selected Package.", parentId, id), 0);
                                issues = true;
                                parentElement = selectedPackage.Element;
                            }
                            if ("Package".Equals(parentElement.Type))
                            {
                                EA.Package parentPackage;
                                if (eaPackageDict.ContainsKey(parentId))
                                {
                                    parentPackage = eaPackageDict[parentId];
                                }
                                else
                                {
                                    Repository.WriteOutput("MAX", string.Format("Parent Package(id={0}) for Element(id={1}) not created yet. To correct: adjust the order in the MAX XML file. Fallback to selected Package.", parentId, id), 0);
                                    issues = true;
                                    parentPackage = selectedPackage;
                                }
                                eaElement = (EA.Element)parentPackage.Elements.AddNew(name, type);
                            }
                            else
                            {
                                eaElement = (EA.Element)parentElement.Elements.AddNew(name, type);
                            }
                        }
                        else
                        {
                            eaElement = (EA.Element)selectedPackage.Elements.AddNew(name, type);
                        }
                    }
                    eaElement.TreePos = objPos++;
                    eaElementDict[id] = eaElement;
                }
                if (maxObj.alias != null)
                {
                    eaElement.Alias = maxObj.alias;
                }
                if (maxObj.stereotype != null)
                {
                    eaElement.Stereotype = maxObj.stereotype;
                }
                if (maxObj.notes != null && maxObj.notes.Text != null && maxObj.notes.Text.Length > 0)
                {
                    eaElement.Notes = maxObj.notes.Text[0].Trim().Replace("\n", "\r\n");
                }
                if (maxObj.isAbstractSpecified)
                {
                    eaElement.Abstract = maxObj.isAbstract ? "1" : "0";
                }
                eaElement.Update();
                EA.TaggedValue tvID = (EA.TaggedValue)eaElement.TaggedValues.GetByName(TV_MAX_ID);
                if (tvID == null)
                {
                    tvID = ((EA.TaggedValue)eaElement.TaggedValues.AddNew(TV_MAX_ID, ""));
                }
                tvID.Value = id;
                tvID.Update();

                if (maxObj.modifiedSpecified)
                {
                    eaElement.Modified = (DateTime)maxObj.modified; // explicit convertion of the value to a DateTime
                }
                if (maxObj.tag != null)
                {
                    foreach (TagType maxTag in maxObj.tag)
                    {
                        string tagName = maxTag.name.Trim();
                        EA.TaggedValue tv = (EA.TaggedValue)eaElement.TaggedValues.GetByName(tagName);
                        if (tv == null)
                        {
                            tv = (EA.TaggedValue)eaElement.TaggedValues.AddNew(tagName, "TaggedValue");
                        }
                        if (maxTag.value != null)
                        {
                            tv.Value = maxTag.value;
                        }
                        if (maxTag.Text != null && maxTag.Text.Length > 0)
                        {
                            tv.Notes = maxTag.Text[0].Trim().Replace("\n", "\r\n");
                        }
                        tv.Update();
                    }
                }
                if (maxObj.attribute != null)
                {
                    int attPos = 0;
                    eaElement.Attributes.Refresh();
                    foreach (AttributeType maxAtt in maxObj.attribute)
                    {
                        string attName = maxAtt.name.Trim();
                        EA.Attribute att = (EA.Attribute)eaElement.Attributes.GetByName(attName);
                        if (att == null)
                        {
                            att = (EA.Attribute)eaElement.Attributes.AddNew(attName, "");
                        }
                        att.Pos = attPos++;
                        if (maxAtt.alias != null)
                        {
                            att.Alias = maxAtt.alias;
                        }
                        if (maxAtt.type != null)
                        {
                            att.Type = maxAtt.type;
                        }
                        if (maxAtt.minCard != null)
                        {
                            att.LowerBound = maxAtt.minCard;
                        }
                        if (maxAtt.maxCard != null)
                        {
                            att.UpperBound = maxAtt.maxCard;
                        }
                        if (maxAtt.value != null)
                        {
                            att.Default = maxAtt.value;
                        }
                        if (maxAtt.Text != null && maxAtt.Text.Length > 0)
                        {
                            att.Notes = maxAtt.Text[0].Trim().Replace("\n", "\r\n");
                        }
                        if (maxAtt.stereotype != null)
                        {
                            att.Stereotype = maxAtt.stereotype;
                        }
                        if (maxAtt.isReadOnlySpecified)
                        {
                            att.IsConst = maxAtt.isReadOnly;
                        }
                        att.Update();
                        if (maxAtt.tag != null)
                        {
                            foreach (TagType maxTag in maxAtt.tag)
                            {
                                EA.AttributeTag attTag = (EA.AttributeTag)att.TaggedValues.AddNew(maxTag.name, "");
                                attTag.Value = maxTag.value;
                                if (maxTag.Text != null && maxTag.Text.Length > 0)
                                {
                                    attTag.Notes = maxTag.Text[0].Trim().Replace("\n", "\r\n");
                                }
                                attTag.Update();
                            }
                        }
                    }
                }
                window.step();
            }
            return issues;
        }

        private bool importRelationships(EA.Package selectedPackage, RelationshipType[] relationships)
        {
            bool issues = false;

            // don't know how to update this, just clear connectors and recreate if the import file has relationships otherwise ignore relationships
            foreach (EA.Element eaElement in eaElementDict.Values)
            {
                for (int c = eaElement.Connectors.Count - 1; c >= 0; c--)
                {
                    eaElement.Connectors.Delete((short)c);
                }
                eaElement.Update();
            }

            foreach (RelationshipType maxRel in relationships)
            {
                string sourceId = maxRel.sourceId.Trim().ToUpper();
                EA.Element eaSourceElement;
                if (!eaElementDict.ContainsKey(sourceId))
                {
                    // create placeholder object; missing in objects list??
                    Repository.WriteOutput("MAX", string.Format("Abstract placeholder created for missing Source id={0}", sourceId), 0);
                    issues = true;
                    eaSourceElement = (EA.Element)selectedPackage.Elements.AddNew(string.Format("_{0}", sourceId), "Class");
                    eaSourceElement.Abstract = "1";
                    eaSourceElement.Update();
                    if (eaSourceElement.TaggedValues.GetByName(TV_MAX_ID) == null)
                    {
                        EA.TaggedValue tvID = ((EA.TaggedValue)eaSourceElement.TaggedValues.AddNew(TV_MAX_ID, ""));
                        tvID.Value = sourceId;
                        tvID.Update();
                    }
                    eaElementDict[sourceId] = eaSourceElement;
                }
                else
                {
                    eaSourceElement = eaElementDict[sourceId];
                }

                string destId = maxRel.destId.Trim().ToUpper();
                EA.Element eaDestElement;
                if (!eaElementDict.ContainsKey(destId))
                {
                    // create placeholder object; missing in objects list??
                    Repository.WriteOutput("MAX", string.Format("Abstract placeholder created for missing Dest id={0}", sourceId), 0);
                    issues = true;
                    eaDestElement = (EA.Element)selectedPackage.Elements.AddNew(string.Format("_{0}", destId), "Class");
                    eaDestElement.Abstract = "1";
                    eaDestElement.Update();
                    if (eaDestElement.TaggedValues.GetByName(TV_MAX_ID) == null)
                    {
                        EA.TaggedValue tvID = ((EA.TaggedValue)eaDestElement.TaggedValues.AddNew(TV_MAX_ID, ""));
                        tvID.Value = destId;
                        tvID.Update();
                    }
                    eaElementDict[destId] = eaDestElement;
                }
                else
                {
                    eaDestElement = eaElementDict[destId];
                }

                string type = Enum.GetName(typeof(RelationshipTypeEnum), maxRel.type);
                // Handle special types
                switch (maxRel.type)
                {
                    case RelationshipTypeEnum.Composition:
                        type = "Aggregation";
                        break;
                    case RelationshipTypeEnum.DirectedAssociation:
                        type = "Association";
                        break;
                }
                string label = maxRel.label;
                if (label != null)
                {
                    label = label.Trim();
                }
                else
                {
                    label = "";
                }
                EA.Connector eaCon = (EA.Connector)eaSourceElement.Connectors.AddNew(label, type);
                eaCon.SupplierID = eaDestElement.ElementID;
                // Handle special types
                switch (maxRel.type)
                {
                    case RelationshipTypeEnum.Composition:
                        eaCon.SupplierEnd.Aggregation = 2;
                        break;
                    case RelationshipTypeEnum.DirectedAssociation:
                        eaCon.Direction = "Source -> Destination";
                        eaCon.SupplierEnd.Navigable = "Non-Navigable";
                        break;
                }

                // Generalization doesnot have label and card
                if (!"Generalization".Equals(type))
                {
                    if (maxRel.sourceLabel != null)
                    {
                        eaCon.ClientEnd.Role = maxRel.sourceLabel;
                    }
                    if (maxRel.sourceCard != null)
                    {
                        eaCon.ClientEnd.Cardinality = maxRel.sourceCard;
                    }
                    if (maxRel.destLabel != null)
                    {
                        eaCon.SupplierEnd.Role = maxRel.destLabel;
                    }
                    if (maxRel.destCard != null)
                    {
                        eaCon.SupplierEnd.Cardinality = maxRel.destCard;
                    }
                }
                if (maxRel.stereotype != null)
                {
                    eaCon.Stereotype = maxRel.stereotype.Trim();
                }
                if (maxRel.notes != null && maxRel.notes.Text != null && maxRel.notes.Text.Length > 0)
                {
                    eaCon.Notes = maxRel.notes.Text[0].Trim().Replace("\n", "\r\n");
                }
                eaCon.Update();
                window.step();
            }
            return issues;
        }

        private void recurseEaPackage(EA.Package eaPackage, Dictionary<string, EA.Element> eaElementDict, Dictionary<string, EA.Package> eaPackageDict)
        {
            EA.TaggedValue tvID = (EA.TaggedValue)eaPackage.Element.TaggedValues.GetByName(TV_MAX_ID);
            if (tvID != null)
            {
                eaPackageDict[tvID.Value.ToUpper()] = eaPackage;
            }
            else
            {
                eaPackageDict[eaPackage.PackageID.ToString()] = eaPackage;
            }

            foreach (EA.Package eaSubPackage in eaPackage.Packages)
            {
                EA.TaggedValue tvSID = (EA.TaggedValue)eaSubPackage.Element.TaggedValues.GetByName(TV_MAX_ID);
                if (tvSID != null)
                {
                    eaElementDict[tvSID.Value.ToUpper()] = eaSubPackage.Element;
                }
                else
                {
                    eaElementDict[eaSubPackage.PackageID.ToString()] = eaSubPackage.Element;
                }
                recurseEaPackage(eaSubPackage, eaElementDict, eaPackageDict);
            }
            foreach (EA.Element eaElement in eaPackage.Elements)
            {
                EA.TaggedValue tvEID = (EA.TaggedValue)eaElement.TaggedValues.GetByName(TV_MAX_ID);
                if (tvEID != null)
                {
                    eaElementDict[tvEID.Value.ToUpper()] = eaElement;
                }
                else
                {
                    eaElementDict[eaElement.ElementID.ToString()] = eaElement;
                }
                recurseEaElements(eaElement, eaElementDict);
            }
        }

        private void recurseEaElements(EA.Element eaElement, Dictionary<string, EA.Element> eaElementDict)
        {
            foreach (EA.Element eaChildElement in eaElement.Elements)
            {
                EA.TaggedValue tvID = (EA.TaggedValue)eaChildElement.TaggedValues.GetByName(TV_MAX_ID);
                if (tvID != null)
                {
                    eaElementDict[tvID.Value.ToUpper()] = eaChildElement;
                }
                else
                {
                    eaElementDict[eaChildElement.ElementID.ToString()] = eaChildElement;
                }
                recurseEaElements(eaChildElement, eaElementDict);
            }
        }
    }
}
