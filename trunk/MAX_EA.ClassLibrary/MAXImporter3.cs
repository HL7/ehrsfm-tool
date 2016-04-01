using System;
using System.Collections.Generic;
using System.Linq;
using MAX_EA.MAXSchema;
using System.Xml.Serialization;
using System.IO;

namespace MAX_EA
{
    class MAXImporterWorkingMemory
    {
        public EA.Package eaPackage;
        public int objPos = 0;
        public int pkgPos = 0;
        public bool issues = false;
    }

    public class MAXImporter3
    {
        // Tagged value used for externalized ID
        private const string TV_MAX_ID = "MAX::ID";

        private EA.Repository Repository;
        private readonly ProgressWindow progress = new ProgressWindow();
        private readonly Dictionary<string, EA.Element> eaElementDict = new Dictionary<string, EA.Element>();
        private readonly Dictionary<string, EA.Package> eaPackageDict = new Dictionary<string, EA.Package>();
        private readonly Dictionary<string, int> idMappings = new Dictionary<string, int>();

        public bool import(EA.Repository Repository, EA.Package selectedPackage, string fileName)
        {
            this.Repository = Repository;
            progress.Show();

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
                progress.setup(max);

                // Don't use Linq constructs those are reeeeaaalllyyyy slow, create a helper dictionary for quick lookup
                // This construct also makes sure there will be unique elements based on the ID|MAX::ID
                eaElementDict.Clear();
                eaPackageDict.Clear();
                updateDicts(selectedPackage);

                // do objects
                if (model.objects != null && model.objects.Any())
                {
                    MAXImporterWorkingMemory wm = new MAXImporterWorkingMemory();
                    wm.eaPackage = selectedPackage;
                    issues |= importObjects(model.objects, wm);
                }

                // now do relationships
                if (model.relationships != null && model.relationships.Any())
                {
                    issues |= importRelationships(selectedPackage, model.relationships, true);
                }

                // Add/update import metadata to the package
                EA.TaggedValue tvImportDate = (EA.TaggedValue)selectedPackage.Element.TaggedValues.GetByName("MAX::ImportDate");
                if (tvImportDate == null)
                {
                    tvImportDate = (EA.TaggedValue)selectedPackage.Element.TaggedValues.AddNew("MAX::ImportDate", "");
                }
                tvImportDate.Value = DateTime.Now.ToString();
                tvImportDate.Update();
                EA.TaggedValue tvImportFile = (EA.TaggedValue)selectedPackage.Element.TaggedValues.GetByName("MAX::ImportFile");
                if (tvImportFile == null)
                {
                    tvImportFile = (EA.TaggedValue)selectedPackage.Element.TaggedValues.AddNew("MAX::ImportFile", "");
                }
                tvImportFile.Value = fileName;
                tvImportFile.Update();
            }
            Repository.EnableUIUpdates = true;
            Repository.BatchAppend = false;
            Repository.RefreshModelView(selectedPackage.PackageID);

            progress.Close();
            return issues;
        }

        private bool importObjects(ObjectType[] objects, MAXImporterWorkingMemory wm)
        {
            // first do objects without a parent
            foreach (ObjectType maxObj in objects.Where(maxObj => string.IsNullOrEmpty(maxObj.parentId)))
            {
                importObject(maxObj, wm);
            }
            List<ObjectType> leftOver = new List<ObjectType>();
            foreach (ObjectType maxObj in objects.Where(maxObj => !string.IsNullOrEmpty(maxObj.parentId)))
            {
                if (eaElementDict.ContainsKey(maxObj.parentId))
                {
                    importObject(maxObj, wm);
                }
                else
                {
                    leftOver.Add(maxObj);
                }
            }
            foreach (ObjectType maxObj in leftOver)
            {
                importObject(maxObj, wm);
            }
            return wm.issues;
        }

        private void importObject(ObjectType maxObj, MAXImporterWorkingMemory wm)
        {
            if (maxObj.id == null)
            {
                Repository.WriteOutput("MAX", string.Format("Skipped {0} because id missing.", maxObj.name), 0);
                wm.issues = true;
                return;
            }
            string id = maxObj.id.Trim().ToUpper();

            string name = maxObj.name;
            EA.Element eaElement;
            // if this element is unwanted then delete it or ignore it when it is not anymore in the model
            if ("#UNWANTED#".Equals(name))
            {
                if (eaElementDict.ContainsKey(id))
                {
                    eaElement = eaElementDict[id];
                    short idx = 0;
                    foreach (EA.Element pkgElement in wm.eaPackage.Elements)
                    {
                        if (pkgElement.ElementGUID == eaElement.ElementGUID)
                        {
                            wm.eaPackage.Elements.DeleteAt(idx, true);
                            return;
                        }
                        idx++;
                    }
                }
                return;
            }
            // check if element already in package
            // if not create, otherwise use existing and update
            if (eaElementDict.ContainsKey(id))
            {
                eaElement = eaElementDict[id];
                eaElement.Name = name;
                // Only change if not Package type (cannot be changed) or given
                if (maxObj.typeSpecified && !"Package".Equals(eaElement.Type))
                {
                    eaElement.Type = Enum.GetName(typeof(ObjectTypeEnum), maxObj.type);
                }
            }
            else
            {
                // when object doesnot exist, then default to "Class"
                string type = "Class";
                if (maxObj.typeSpecified)
                {
                    type = Enum.GetName(typeof(ObjectTypeEnum), maxObj.type);
                }
                string parentId = maxObj.parentId;
                if ("Package".Equals(type))
                {
                    // Name cannot be empty when creating a new Package
                    bool emptyPackageNameWorkaround = false;
                    if (string.IsNullOrEmpty(name))
                    {
                        emptyPackageNameWorkaround = true;
                        name = "_";
                    }
                    EA.Package eaPackage;
                    if (!string.IsNullOrEmpty(parentId))
                    {
                        parentId = parentId.Trim().ToUpper();
                        if (eaPackageDict.ContainsKey(parentId))
                        {
                            eaPackage = (EA.Package)eaPackageDict[parentId].Packages.AddNew(name, type);
                        }
                        else
                        {
                            Repository.WriteOutput("MAX", string.Format("Parent Package(id={0}) for Package(id={1}) not created yet. To correct: adjust the order in the MAX XML file. Fallback to selected Package.", parentId, id), 0);
                            wm.issues = true;
                            eaPackage = (EA.Package)wm.eaPackage.Packages.AddNew(name, type);
                        }
                    }
                    else
                    {
                        eaPackage = (EA.Package)wm.eaPackage.Packages.AddNew(name, type);
                    }
                    eaPackage.TreePos = wm.pkgPos++;
                    eaPackage.Update();
                    if (emptyPackageNameWorkaround)
                    {
                        eaPackage.Name = String.Empty;
                        eaPackage.Update();
                    }
                    eaPackageDict[id] = eaPackage;
                    eaElement = eaPackage.Element;
                }
                else
                {
                    if (!string.IsNullOrEmpty(parentId))
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
                            wm.issues = true;
                            parentElement = wm.eaPackage.Element;
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
                                wm.issues = true;
                                parentPackage = wm.eaPackage;
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
                        eaElement = (EA.Element)wm.eaPackage.Elements.AddNew(name, type);
                    }
                }
                eaElement.TreePos = wm.objPos++;
                eaElementDict[id] = eaElement;
            }
            if (maxObj.alias != null)
            {
                eaElement.Alias = maxObj.alias;
            }
            if (maxObj.stereotype != null)
            {
                eaElement.StereotypeEx = maxObj.stereotype;
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
                tvID = (EA.TaggedValue)eaElement.TaggedValues.AddNew(TV_MAX_ID, "");
            }
            tvID.Value = id;
            tvID.Update();

            if (maxObj.modifiedSpecified)
            {
                eaElement.Modified = maxObj.modified;
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
                    if ("#UNWANTED#".Equals(attName))
                    {
                        short idx = 0;
                        foreach (EA.Attribute eaAtt in eaElement.Attributes)
                        {
                            if (eaAtt.AttributeID.ToString().Equals(maxAtt.id))
                            {
                                eaElement.Attributes.DeleteAt(idx, true);
                                continue;
                            }
                            idx++;
                        }
                        continue;
                    }
                    // Prefer att id, but that is not always known
                    EA.Attribute att = getAttributeByIdOrName(eaElement, maxAtt.id, attName);// (EA.Attribute)eaElement.Attributes.GetByName(attName);
                    if (att == null)
                    {
                        att = (EA.Attribute)eaElement.Attributes.AddNew(attName, "");
                    }
                    att.Pos = attPos++;
                    att.Name = attName;
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
                        att.StereotypeEx = maxAtt.stereotype;
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
            progress.step();
        }

        private bool importRelationships(EA.Package selectedPackage, RelationshipType[] relationships, bool deleteExisting)
        {
            bool issues = false;

            // don't know how to update this, just clear connectors and recreate if the import file has relationships otherwise ignore relationships
            if (deleteExisting)
            {
                foreach (EA.Element eaElement in eaElementDict.Values)
                {
                    bool updateElement = false;
                    int conCount = eaElement.Connectors.Count;
                    for (short c = (short) (conCount - 1); c >= 0; c--)
                    {
                        EA.Connector con = (EA.Connector) eaElement.Connectors.GetAt(c);
                        if (idMappings.ContainsValue(con.SupplierID))
                        {
                            eaElement.Connectors.Delete(c);
                            updateElement = true;
                        }
                    }
                    if (updateElement)
                    {
                        eaElement.Update();
                    }
                }
            }

            foreach (RelationshipType maxRel in relationships)
            {
                if (maxRel.sourceId == null || maxRel.destId == null)
                {
                    Repository.WriteOutput("MAX", "Skipped relationship missing sourceId and destId", 0);
                    issues = true;
                    continue;
                }
                else if (maxRel.sourceId == null)
                {
                    Repository.WriteOutput("MAX", string.Format("Skipped relationship dest {0}, missing sourceId", maxRel.destId), 0);
                    issues = true;
                    continue;
                }
                else if (maxRel.destId == null)
                {
                    Repository.WriteOutput("MAX", string.Format("Skipped relationship source {0}, missing destId", maxRel.id), 0);
                    issues = true;
                    continue;
                }
                string sourceId = maxRel.sourceId.Trim().ToUpper();
                EA.Element eaSourceElement;
                if (!eaElementDict.ContainsKey(sourceId))
                {
                    // create abstract placeholder object; missing in objects list??
                    Repository.WriteOutput("MAX", string.Format("Placeholder created for missing source {0}", sourceId), 0);
                    issues = true;
                    eaSourceElement = (EA.Element)selectedPackage.Elements.AddNew(string.Format("_{0}", sourceId), "Class");
                    eaSourceElement.Abstract = "1";
                    eaSourceElement.Update();
                    if (eaSourceElement.TaggedValues.GetByName(TV_MAX_ID) == null)
                    {
                        EA.TaggedValue tvID = (EA.TaggedValue)eaSourceElement.TaggedValues.AddNew(TV_MAX_ID, "");
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
                    Repository.WriteOutput("MAX", string.Format("Placeholder created for missing dest {0}", destId), 0);
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

                string type = "Association";
                if (maxRel.typeSpecified)
                {
                    type = Enum.GetName(typeof(RelationshipTypeEnum), maxRel.type);
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
                    eaCon.StereotypeEx = maxRel.stereotype.Trim();
                }
                if (maxRel.notes != null && maxRel.notes.Text != null && maxRel.notes.Text.Length > 0)
                {
                    eaCon.Notes = maxRel.notes.Text[0].Trim().Replace("\n", "\r\n");
                }
                try
                {
                    eaCon.Update();
                }
                catch (Exception)
                {
                    Repository.WriteOutput("MAX", string.Format("Illegal relationship type '{0}' between {1} and {2}", type, sourceId, destId), 0);
                    issues = true;
                }
                if (maxRel.tag != null)
                {
                    foreach (TagType maxTag in maxRel.tag)
                    {
                        string tagName = maxTag.name.Trim();
                        EA.ConnectorTag tv = (EA.ConnectorTag)eaCon.TaggedValues.GetByName(tagName);
                        if (tv == null)
                        {
                            tv = (EA.ConnectorTag)eaCon.TaggedValues.AddNew(tagName, "ConnectorTag");
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
                progress.step();
            }
            return issues;
        }

        private void updateDicts(EA.Package eaPackage)
        {
            EA.TaggedValue tvID = (EA.TaggedValue)eaPackage.Element.TaggedValues.GetByName(TV_MAX_ID);
            if (tvID != null)
            {
                string maxId = tvID.Value.ToUpper();
                eaPackageDict[maxId] = eaPackage;
                eaElementDict[maxId] = eaPackage.Element;
            }
            else
            {
                string pkgElId = eaPackage.Element.ElementID.ToString();
                eaPackageDict[pkgElId] = eaPackage;
                eaElementDict[pkgElId] = eaPackage.Element;
            }
            foreach (EA.Package eaSubPackage in eaPackage.Packages)
            {
                updateDicts(eaSubPackage);
            }
            foreach (EA.Element eaElement in eaPackage.Elements)
            {
                updateElementDict(eaElement);
            }
        }

        private void updateElementDict(EA.Element eaElement)
        {
            EA.TaggedValue tvEID = (EA.TaggedValue)eaElement.TaggedValues.GetByName(TV_MAX_ID);
            if (tvEID != null)
            {
                eaElementDict[tvEID.Value.ToUpper()] = eaElement;
                idMappings[tvEID.Value.ToUpper()] = eaElement.ElementID;
            }
            else
            {
                eaElementDict[eaElement.ElementID.ToString()] = eaElement;
            }
            foreach (EA.Element eaChildElement in eaElement.Elements)
            {
                updateElementDict(eaChildElement);
            }
        }

        private EA.Attribute getAttributeByName(EA.Element eaElement, string attName)
        {
            foreach (EA.Attribute eaAttribute in eaElement.Attributes)
            {
                if (attName.Equals(eaAttribute.Name))
                {
                    return eaAttribute;
                }
            }
            return null;
        }

        private EA.Attribute getAttributeByIdOrName(EA.Element eaElement, string attId, string attName)
        {
            foreach (EA.Attribute eaAttribute in eaElement.Attributes)
            {
                if (eaAttribute.AttributeID.ToString().Equals(attId))
                {
                    return eaAttribute;
                }
                if (attName.Equals(eaAttribute.Name))
                {
                    return eaAttribute;
                }
            }
            return null;
        }
    }
}
