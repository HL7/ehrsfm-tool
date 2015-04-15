using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MAX_EA.MAXSchema;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml;

namespace MAX_EA
{
    public class MAXExporter3 : Util
    {
        // Temporary tagged value used for externalized ID
        private const string TV_MAX_ID = "MAX::ID";
        private const string OUTPUT_TAB_MAX = "MAX";

        private readonly ProgressWindow progress = new ProgressWindow();
        private int progress_max;
        private bool issues = false;
        private List<ObjectType> objects;
        private List<RelationshipType> relationships;
        private EA.Repository Repository;
        private readonly Dictionary<int, int> packageToObjectIDDict = new Dictionary<int, int>();
        private readonly Dictionary<int, string> _objectID2MAXIDDict = new Dictionary<int, string>();

        public bool export(EA.Repository Repository)
        {
            EA.ObjectType type = Repository.GetContextItemType();
            if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = (EA.Package)Repository.GetContextObject();
                exportPackage(Repository, package);
            }
            else if (type == EA.ObjectType.otDiagram)
            {
                EA.Diagram diagram = (EA.Diagram)Repository.GetContextObject();
                string defaultFileName = Path.Combine(Path.GetTempPath(), string.Format(@"{0}.max.xml", diagram.Name));
                string fileName = showFileDialog("Select output MAX XML file", "xml files (*.xml)|*.xml", defaultFileName, false);
                if (fileName != String.Empty)
                {
                    exportDiagram(Repository, diagram, fileName);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Select a Package or Diagram");
            }
            return issues;
        }

        public bool export(EA.Repository Repository, string fileName)
        {
            EA.ObjectType type = Repository.GetTreeSelectedItemType();
            if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = Repository.GetTreeSelectedPackage();
                exportPackage(Repository, package, fileName);
            }
            else if (type == EA.ObjectType.otDiagram)
            {
                EA.Diagram diagram = (EA.Diagram)Repository.GetTreeSelectedObject();
                exportDiagram(Repository, diagram, fileName);
            }
            return issues;
        }

        public bool exportPackage(EA.Repository Repository, EA.Package package)
        {
            string defaultFileName = Path.Combine(Path.GetTempPath(), string.Format(@"{0}.max.xml", package.Name));
            EA.TaggedValue tvExportFile = (EA.TaggedValue)package.Element.TaggedValues.GetByName("MAX::ExportFile");
            if (tvExportFile != null)
            {
                defaultFileName = tvExportFile.Value;
            }
            string fileName = showFileDialog("Select output MAX XML file", "xml files (*.xml)|*.xml", defaultFileName, false);
            if (fileName != String.Empty)
            {
                exportPackage(Repository, package, fileName);
            }
            return issues;
        }

        public void exportPackage(EA.Repository Repository, EA.Package package, string fileName)
        {
            this.Repository = Repository;

            progress_max = 1;
            progress.setup(progress_max);
            progress.Show();

            Repository.EnableCache = true;
            // Add/update export metadata to the model
            ModelType model = new ModelType();
            model.exportDate = DateTime.Now.ToString();

            // Now export selected package
            objects = new List<ObjectType>();
            relationships = new List<RelationshipType>();
            visitSelectedPackage(package.PackageID);

            model.objects = objects.ToArray();
            model.relationships = relationships.ToArray();
            progress.Close();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\n";
            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                serializer.Serialize(writer, model);
            }

            // Add/update export metadata to the package
            EA.TaggedValue tvExportDate = (EA.TaggedValue)package.Element.TaggedValues.GetByName("MAX::ExportDate");
            if (tvExportDate == null)
            {
                tvExportDate = (EA.TaggedValue)package.Element.TaggedValues.AddNew("MAX::ExportDate", "");
            }
            tvExportDate.Value = DateTime.Now.ToString();
            tvExportDate.Update();
            EA.TaggedValue tvExportFile = (EA.TaggedValue)package.Element.TaggedValues.GetByName("MAX::ExportFile");
            if (tvExportFile == null)
            {
                tvExportFile = (EA.TaggedValue)package.Element.TaggedValues.AddNew("MAX::ExportFile", "");
            }
            tvExportFile.Value = fileName;
            tvExportFile.Update();
        }

        public void exportDiagram(EA.Repository Repository, EA.Diagram diagram, string fileName)
        {
            this.Repository = Repository;
            progress_max = 1;
            progress.Show();
            progress.setup(progress_max);

            Repository.EnableCache = true;
            // Add/update export metadata to the model
            ModelType model = new ModelType();
            model.exportDate = DateTime.Now.ToString();

            // Now export selected package
            objects = new List<ObjectType>();
            relationships = new List<RelationshipType>();
            visitDiagramObjects(diagram.DiagramID);

            model.objects = objects.ToArray();
            model.relationships = relationships.ToArray();
            progress.Close();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.NewLineChars = "\n";
            XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                serializer.Serialize(writer, model);
            }
        }

        private void visitSelectedPackage(int Package_ID)
        {
            // select sub packages of selected package
            string sql = string.Format("SELECT p.Package_ID, p.ea_guid, p.Name, o.Alias, p.ModifiedDate, o.Stereotype, p.Notes, o.Object_ID FROM t_package p, t_object o WHERE p.Package_ID={0} AND p.ea_guid=o.ea_guid", Package_ID);
            string xml = Repository.SQLQuery(sql);
            XElement xEADATA = XElement.Parse(xml, LoadOptions.None);

            XElement xRow = xEADATA.XPathSelectElement("//Data/Row");
            int Object_ID = xRow.ElementValueInt("Object_ID");

            // package tagged values
            string sql_tv = string.Format("SELECT Property, Value, Notes FROM t_objectproperties WHERE Object_ID = {0}", Object_ID);
            string xml_tv = Repository.SQLQuery(sql_tv);
            XElement xEADATA_tv = XElement.Parse(xml_tv, LoadOptions.None);

            // update map from internal id to MAX::ID
            foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Property='{0}']", TV_MAX_ID)))
            {
                string MAX_ID = xTV.ElementValue("Value");
                _objectID2MAXIDDict[Object_ID] = MAX_ID;
            }

            ObjectType maxObj = new ObjectType();
            maxObj.id = mapObjectID2MAXID(Object_ID);
            maxObj.name = xRow.ElementValue("Name");
            maxObj.alias = xRow.ElementValue("Alias");
            string notes = xRow.ElementValue("Notes");
            if (!string.IsNullOrEmpty(notes))
            {
                maxObj.notes = new MarkupType() { Text = new[] { notes } };
            }
            maxObj.stereotype = xRow.ElementValue("Stereotype");
            maxObj.type = ObjectTypeEnum.Package;
            maxObj.typeSpecified = true;
            maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
            maxObj.modifiedSpecified = true;
            packageToObjectIDDict[Package_ID] = Object_ID;

            // Package Tagged Values
            IEnumerable<XElement> xRows = xEADATA_tv.XPathSelectElements("//Data/Row");
            List<TagType> tags = new List<TagType>();
            foreach (XElement xTV in xRows)
            {
                string tagName = xTV.ElementValue("Property");
                if (!TV_MAX_ID.Equals(tagName))
                {
                    TagType maxTag = new TagType
                    {
                        name = tagName,
                        value = xTV.ElementValue("Value"),
                        Text = new[] { xTV.ElementValue("Notes") }
                    };
                    tags.Add(maxTag);
                }
            }
            maxObj.tag = tags.ToArray();
            objects.Add(maxObj);

            visitPackage(Package_ID);
        }

        private void visitPackage(int Package_ID)
        {
            int Package_Object_ID = packageToObjectIDDict[Package_ID];

            // select sub packages of selected package
            XElement xEADATA = executeQuery(string.Format("SELECT p.Package_ID, p.ea_guid, p.Name, o.Alias, p.ModifiedDate, o.Stereotype, p.Notes, o.Object_ID FROM t_package p, t_object o WHERE p.Parent_ID={0} AND o.Package_ID={1} AND p.ea_guid=o.ea_guid ORDER BY p.TPos", Package_ID, Package_ID));

            // get tagged values in packages in selected package
            XElement xEADATA_tv = executeQuery(string.Format("SELECT op.Object_ID, op.Property, op.Value, op.Notes FROM t_object o, t_objectproperties op WHERE Package_ID={0} AND o.Object_ID=op.Object_ID", Package_ID));

            // update map from internal id to MAX::ID
            foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Property='{0}']", TV_MAX_ID)))
            {
                int Object_ID = xTV.ElementValueInt("Object_ID");
                string MAX_ID = xTV.ElementValue("Value");
                _objectID2MAXIDDict[Object_ID] = MAX_ID;
            }

            IEnumerable<XElement> xRows = xEADATA.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count<XElement>();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                ObjectType maxObj = new ObjectType();
                int Object_ID = xRow.ElementValueInt("Object_ID");
                maxObj.id = mapObjectID2MAXID(Object_ID);
                int Sub_Package_ID = xRow.ElementValueInt("Package_ID");
                maxObj.name = xRow.ElementValue("Name");
                maxObj.alias = xRow.ElementValue("Alias");
                string notes = xRow.ElementValue("Notes");
                if (!string.IsNullOrEmpty(notes))
                {
                    maxObj.notes = new MarkupType() { Text = new[] { notes } };
                }
                maxObj.stereotype = xRow.ElementValue("Stereotype");
                maxObj.type = ObjectTypeEnum.Package;
                maxObj.typeSpecified = true;
                maxObj.parentId = mapObjectID2MAXID(Package_Object_ID);
                maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
                maxObj.modifiedSpecified = true;
                List<TagType> tags = new List<TagType>();
                foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Object_ID={0}]", Object_ID)))
                {
                    string tagName = xTV.ElementValue("Property");
                    if (!TV_MAX_ID.Equals(tagName))
                    {
                        TagType maxTag = new TagType 
                        {
                            name = tagName,
                            value = xTV.ElementValue("Value"),
                            Text = new[] {xTV.ElementValue("Notes")}
                        };
                        tags.Add(maxTag);
                    }
                }
                packageToObjectIDDict[Sub_Package_ID] = Object_ID;
                maxObj.tag = tags.ToArray();
                objects.Add(maxObj);
                progress.step();

                visitPackage(Sub_Package_ID);
            }

            // visit objects of this package
            visitPackageObjects(Package_ID);
        }

        // select objects with EA.Package.PackageID as ParentID and recurse on Object_Type = 'Package'
        // "ORDER BY TPos" returns in the order that the elements are in the Package Browser
        // TPos is only set if sort order is changed manually in the Package Browser
        // Use Object_ID for sort initial order
        private void visitPackageObjects(int Package_ID)
        {
            int Package_Object_ID = packageToObjectIDDict[Package_ID];

            // get objects in selected package
            XElement xEADATA = executeQuery(string.Format("SELECT Object_ID, ea_guid, Object_Type, Name, Alias, Note, Package_ID, Stereotype, ModifiedDate, Abstract, Tagged, ea_guid, ParentID FROM t_object WHERE Package_ID={0} AND Object_Type<>'Package' ORDER BY TPos,Object_ID", Package_ID));

            // get tagged values for objects in selected package
            XElement xEADATA_tv = executeQuery(string.Format("SELECT op.Object_ID, op.Property, op.Value, op.Notes FROM t_object o, t_objectproperties op WHERE Package_ID={0} AND o.Object_ID=op.Object_ID", Package_ID));

            // get relationships for objects in selected package
            XElement xEADATA_con = executeQuery(string.Format("SELECT c.Connector_ID, c.Connector_Type, c.Name, c.Notes, c.Start_Object_ID, c.SourceCard, c.SourceRole, c.End_Object_ID, c.DestCard, c.DestRole, c.Stereotype FROM t_connector c, t_object WHERE Start_Object_ID=Object_ID AND Package_ID={0}", Package_ID));

            // get ALL tagged values for relationships for objects in selected package
            XElement xEADATA_con_tv = executeQuery("SELECT ct.ElementID, ct.Property, ct.Value, ct.Notes FROM t_connectortag ct");

            visitObjects(xEADATA, xEADATA_tv, xEADATA_con, xEADATA_con_tv, Package_Object_ID);
        }

        private void visitDiagramObjects(int Diagram_ID)
        {
            // get objects in selected diagram
            XElement xEADATA = executeQuery(string.Format("SELECT o.Object_ID, ea_guid, Object_Type, Name, Alias, Note, Stereotype, ModifiedDate, Abstract, Tagged, ParentID FROM t_object o, t_diagramobjects d WHERE d.Diagram_ID = {0} AND o.Object_ID = d.Object_ID", Diagram_ID));

            // get tagged values in objects in selected diagram
            XElement xEADATA_tv = executeQuery(string.Format("SELECT op.Object_ID, op.Property, op.Value, op.Notes FROM t_diagramobjects d, t_object o, t_objectproperties op WHERE d.Diagram_ID = {0} AND o.Object_ID=op.Object_ID AND d.Object_ID=o.Object_ID", Diagram_ID));

            // get relationships for connectors in the diagram
            XElement xEADATA_con = executeQuery(string.Format("SELECT c.Connector_ID, c.Connector_Type, c.Name, c.Notes, c.Start_Object_ID, c.SourceCard, c.SourceRole, c.End_Object_ID, c.DestCard, c.DestRole, c.Stereotype FROM t_diagramlinks dl, t_connector c WHERE dl.DiagramID={0} AND dl.ConnectorID = c.Connector_ID", Diagram_ID));

            // get ALL tagged values for relationships for objects in selected package
            XElement xEADATA_con_tv = executeQuery("SELECT ct.ElementID, ct.Property, ct.Value, ct.Notes FROM t_connectortag ct");

            visitObjects(xEADATA, xEADATA_tv, xEADATA_con, xEADATA_con_tv, null);
        }

        private void visitObjects(XElement xEADATA, XElement xEADATA_tv, XElement xEADATA_con, XElement xEADATA_con_tv, int? Package_Object_ID)
        {
            // update map from internal id to MAX::ID
            foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Property='{0}']", TV_MAX_ID)))
            {
                int Object_ID = xTV.ElementValueInt("Object_ID");
                string MAX_ID = xTV.ElementValue("Value");
                _objectID2MAXIDDict[Object_ID] = MAX_ID;
            }

            // create MAX object elements
            IEnumerable<XElement> xRows = xEADATA.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                int Object_ID = xRow.ElementValueInt("Object_ID");
                string Object_Type = xRow.ElementValue("Object_Type");
                // first check if we support the Object_Type
                if (!Enum.IsDefined(typeof(ObjectTypeEnum), Object_Type))
                {
                    progress.step();
                    Repository.WriteOutput("MAX", string.Format("Ignored Object \"{0}\" not supported type \"{1}\"", Object_ID, Object_Type), -1);
                    issues = true;
                    continue;
                }

                ObjectType maxObj = new ObjectType();
                maxObj.id = mapObjectID2MAXID(Object_ID);
                if (xRow.ElementValueInt("Abstract") == 1)
                {
                    maxObj.isAbstract = true;
                    maxObj.isAbstractSpecified = true;
                }
                maxObj.name = xRow.ElementValue("Name");
                maxObj.alias = xRow.ElementValue("Alias");
                string notes = xRow.ElementValue("Note");
                if (!string.IsNullOrEmpty(notes))
                {
                    maxObj.notes = new MarkupType() { Text = new[] { notes } };
                }
                maxObj.stereotype = xRow.ElementValue("Stereotype");
                maxObj.type = (ObjectTypeEnum)Enum.Parse(typeof(ObjectTypeEnum), Object_Type, false);
                maxObj.typeSpecified = true;

                int EA_ParentID = int.Parse(xRow.ElementValue("ParentID"));
                // ParentID = 0 for direct childs of a Package, ParentID is only used within a Package
                if (EA_ParentID != 0)
                {
                    maxObj.parentId = mapObjectID2MAXID(EA_ParentID);
                }
                // Package_Object_ID = null when exporting a Diagram
                else if (EA_ParentID == 0 && Package_Object_ID != null)
                {
                    maxObj.parentId = mapObjectID2MAXID((int)Package_Object_ID);
                }
                maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
                maxObj.modifiedSpecified = true;
                List<TagType> tags = new List<TagType>();
                foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Object_ID={0}]", Object_ID)))
                {
                    string tagName = xTV.ElementValue("Property");
                    if (!TV_MAX_ID.Equals(tagName))
                    {
                        TagType maxTag = new TagType
                        {
                            name = tagName,
                            value = xTV.ElementValue("Value"),
                            Text = new[] {xTV.ElementValue("Notes")}
                        };
                        tags.Add(maxTag);
                    }
                }
                maxObj.tag = tags.ToArray();

                // If this is an Artifact of stereotype "document" get the Linked Document BinContent
                if (maxObj.type == ObjectTypeEnum.Artifact && "document".Equals(maxObj.stereotype))
                {
                    XElement xData = executeQuery(string.Format("SELECT BinContent FROM t_document WHERE ElementID='{0}'", xRow.ElementValue("ea_guid")));
                    maxObj.notes = new MarkupType() { Text = new[] { xData.XPathSelectElement("//Data/Row").ElementValue("BinContent") } };
                }

                // get attributes in objects in selected object
                string sql_att = string.Format("SELECT * FROM t_attribute WHERE Object_ID={0}", Object_ID);
                string xml_att = Repository.SQLQuery(sql_att);
                XElement xEADATA_att = XElement.Parse(xml_att, LoadOptions.None);
                List<AttributeType> atts = new List<AttributeType>();
                foreach (XElement xAtt in xEADATA_att.XPathSelectElements("//Data/Row"))
                {
                    AttributeType maxAtt = new AttributeType();
                    int Attribute_ID = xAtt.ElementValueInt("ID");
                    maxAtt.name = xAtt.ElementValue("Name");
                    maxAtt.minCard = xAtt.ElementValue("LowerBound");
                    maxAtt.maxCard = xAtt.ElementValue("UpperBound");
                    maxAtt.value = xAtt.ElementValue("Default");
                    maxAtt.type = xAtt.ElementValue("Type");
                    maxAtt.stereotype = xAtt.ElementValue("Stereotype");
                    int constValue = xAtt.ElementValueInt("Const", -1);
                    if (constValue == 1)
                    {
                        maxAtt.isReadOnly = true;
                        maxAtt.isReadOnlySpecified = true;
                    }
                    maxAtt.Text = new[] { xAtt.ElementValue("Notes") };

                    // get attribute tagged values for selected object
                    string sql_att_tv = string.Format("SELECT * FROM t_attributetag WHERE ElementID={0}", Attribute_ID);
                    string xml_att_tv = Repository.SQLQuery(sql_att_tv);
                    XElement xEADATA_att_tv = XElement.Parse(xml_att_tv, LoadOptions.None);
                    List<TagType> attTags = new List<TagType>();
                    foreach (XElement xTV in xEADATA_att_tv.XPathSelectElements("//Data/Row"))
                    {
                        TagType maxTag = new TagType
                        {
                            name = xTV.ElementValue("Property"),
                            value = xTV.ElementValue("VALUE"),
                            Text = new[] { xTV.ElementValue("NOTES") }
                        };
                        attTags.Add(maxTag);
                    }
                    maxAtt.tag = attTags.ToArray();
                    atts.Add(maxAtt);
                }
                maxObj.attribute = atts.ToArray();
                objects.Add(maxObj);
                progress.step();
            }

            // create MAX relationship elements
            xRows = xEADATA_con.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count<XElement>();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                // first check if we support the Relationship Connector_Type
                string Connector_Type = xRow.ElementValue("Connector_Type");
                if (!Enum.IsDefined(typeof(RelationshipTypeEnum), Connector_Type))
                {
                    progress.step();
                    Repository.WriteOutput("MAX", string.Format("Ignored Relationship with not supported type \"{0}\"", Connector_Type), -1);
                    issues = true;
                    continue;
                }

                RelationshipType maxRel = new RelationshipType();
                //maxRel.id = xRow.ElementValueInt("Connector_ID").ToString();
                int Connector_ID = xRow.ElementValueInt("Connector_ID");
                maxRel.label = xRow.ElementValue("Name");
                maxRel.sourceId = mapObjectID2MAXID(xRow.ElementValueInt("Start_Object_ID"));
                maxRel.sourceCard = xRow.ElementValue("SourceCard");
                maxRel.sourceLabel = xRow.ElementValue("SourceRole");
                maxRel.destId = mapObjectID2MAXID(xRow.ElementValueInt("End_Object_ID"));
                maxRel.destCard = xRow.ElementValue("DestCard");
                maxRel.destLabel = xRow.ElementValue("DestRole");
                maxRel.notes = new MarkupType() { Text = new[] { xRow.ElementValue("Notes") } };
                maxRel.stereotype = xRow.ElementValue("Stereotype");

                List<TagType> tags = new List<TagType>();
                foreach (XElement xTV in xEADATA_con_tv.XPathSelectElements(string.Format("//Data/Row[ElementID={0}]", Connector_ID)))
                {
                    string tagName = xTV.ElementValue("Property");
                    if (!TV_MAX_ID.Equals(tagName))
                    {
                        TagType maxTag = new TagType
                        {
                            name = tagName,
                            value = xTV.ElementValue("Value"),
                            Text = new[] { xTV.ElementValue("Notes") }
                        };
                        tags.Add(maxTag);
                    }
                }
                maxRel.tag = tags.ToArray();

                maxRel.type = (RelationshipTypeEnum)Enum.Parse(typeof(RelationshipTypeEnum), xRow.ElementValue("Connector_Type"), false);
                maxRel.typeSpecified = true;
                // Special type adjustments
                switch (maxRel.type)
                {
                    case RelationshipTypeEnum.Aggregation:
                        // eaCon.SupplierEnd.Aggregation = 2;
                        break;
                    case RelationshipTypeEnum.Association:
                        // eaCon.Direction = "Source -> Destination";
                        // eaCon.SupplierEnd.Navigable = "Non-Navigable";
                        break;
                }

                relationships.Add(maxRel);
                progress.step();
            }
        }

        private string mapObjectID2MAXID(int objectId)
        {
            string maxId;
            if (_objectID2MAXIDDict.ContainsKey(objectId))
            {
                maxId = _objectID2MAXIDDict[objectId];
            }
            else
            {
                try
                {
                    // Probably an element outside selected package, try to get MAX_ID tagged value
                    EA.Element eaElement = Repository.GetElementByID(objectId);
                    EA.TaggedValue tvID = (EA.TaggedValue)eaElement.TaggedValues.GetByName(TV_MAX_ID);
                    if (tvID != null)
                    {
                        maxId = tvID.Value;
                        _objectID2MAXIDDict[objectId] = maxId;
                    }
                    else
                    {
                        maxId = objectId.ToString();
                    }
                }
                catch (Exception guru)
                {
                    // Problem getting the element with this ID
                    // TODO: Log this message; figure out how this can happen
                    maxId = objectId.ToString();
                    Repository.WriteOutput("MAX", string.Format("Problem getting Object \"{0}\" from Repository: {1}", objectId, guru.Message), -1);
                    issues = true;
                }
            }
            return maxId;
        }

        private XElement executeQuery(string sql)
        {
            string xml = Repository.SQLQuery(sql);
            return XElement.Parse(xml, LoadOptions.None);
        }
    }
}
