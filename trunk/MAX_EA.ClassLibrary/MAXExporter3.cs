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

        ProgressWindow progress = new ProgressWindow();
        int progress_max;
        List<ObjectType> objects;
        List<RelationshipType> relationships;
        EA.Repository Repository;
        Dictionary<int, int> mapPackageToObjectID = new Dictionary<int, int>();
        Dictionary<int, string> mapObjectID2MAXID = new Dictionary<int, string>();

        public void export(EA.Repository Repository)
        {
            EA.ObjectType type = Repository.GetTreeSelectedItemType();
            if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = Repository.GetTreeSelectedPackage();
                string defaultFileName = string.Format(@"D:\tmp\{0}.max.xml", package.Name);
                string fileName = showFileDialog("Select output MAX XML file", "xml files (*.xml)|*.xml", defaultFileName, false);
                if (fileName != String.Empty)
                {
                    exportPackage(Repository, package, fileName);
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Select a package");
            }
        }

        public void export(EA.Repository Repository, string fileName)
        {
            EA.ObjectType type = Repository.GetTreeSelectedItemType();
            if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = Repository.GetTreeSelectedPackage();
                exportPackage(Repository, package, fileName);
            }
        }

        public void exportPackage(EA.Repository Repository, EA.Package package, string fileName)
        {
            this.Repository = Repository;
            progress_max = 1;
            progress.setup(progress_max);
            progress.Show();
            progress.Refresh();

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
                mapObjectID2MAXID[Object_ID] = MAX_ID;
            }

            ObjectType maxObj = new ObjectType();
            if (mapObjectID2MAXID.ContainsKey(Object_ID))
            {
                maxObj.id = mapObjectID2MAXID[Object_ID];
            }
            else
            {
                maxObj.id = Object_ID.ToString();
            }
            maxObj.name = xRow.ElementValue("Name");
            maxObj.alias = xRow.ElementValue("Alias");
            string notes = xRow.ElementValue("Notes");
            if (!string.IsNullOrEmpty(notes))
            {
                maxObj.notes = new MarkupType() { Text = new String[] { notes } };
            }
            maxObj.stereotype = xRow.ElementValue("Stereotype");
            maxObj.type = ObjectTypeEnum.Package;
            maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
            maxObj.modifiedSpecified = true;
            mapPackageToObjectID[Package_ID] = Object_ID;

            // Package Tagged Values
            IEnumerable<XElement> xRows = xEADATA_tv.XPathSelectElements("//Data/Row");
            List<TagType> tags = new List<TagType>();
            foreach (XElement xTV in xRows)
            {
                string tagName = xTV.ElementValue("Property");
                if (!TV_MAX_ID.Equals(tagName))
                {
                    TagType maxTag = new TagType();
                    maxTag.name = tagName;
                    maxTag.value = xTV.ElementValue("Value");
                    maxTag.Text = new String[] { xTV.ElementValue("Notes") };
                    tags.Add(maxTag);
                }
            }
            maxObj.tag = tags.ToArray();
            objects.Add(maxObj);

            visitPackage(Package_ID);
        }

        private void visitPackage(int Package_ID)
        {
            int Package_Object_ID = mapPackageToObjectID[Package_ID];

            // select sub packages of selected package
            string sql = string.Format("SELECT p.Package_ID, p.ea_guid, p.Name, o.Alias, p.ModifiedDate, o.Stereotype, p.Notes, o.Object_ID FROM t_package p, t_object o WHERE p.Parent_ID={0} AND o.Package_ID={1} AND p.ea_guid=o.ea_guid", Package_ID, Package_ID);
            string xml = Repository.SQLQuery(sql);
            XElement xEADATA = XElement.Parse(xml, LoadOptions.None);

            // get tagged values in packages in selected package
            string sql_tv = string.Format("SELECT op.Object_ID, op.Property, op.Value, op.Notes FROM t_object o, t_objectproperties op WHERE Package_ID={0} AND o.Object_ID=op.Object_ID", Package_ID);
            string xml_tv = Repository.SQLQuery(sql_tv);
            XElement xEADATA_tv = XElement.Parse(xml_tv, LoadOptions.None);

            // update map from internal id to MAX::ID
            foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Property='{0}']", TV_MAX_ID)))
            {
                int Object_ID = xTV.ElementValueInt("Object_ID");
                string MAX_ID = xTV.ElementValue("Value");
                mapObjectID2MAXID[Object_ID] = MAX_ID;
            }

            IEnumerable<XElement> xRows = xEADATA.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count<XElement>();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                ObjectType maxObj = new ObjectType();
                int Object_ID = xRow.ElementValueInt("Object_ID");
                if (mapObjectID2MAXID.ContainsKey(Object_ID))
                {
                    maxObj.id = mapObjectID2MAXID[Object_ID];
                }
                else
                {
                    maxObj.id = Object_ID.ToString();
                }
                int Sub_Package_ID = xRow.ElementValueInt("Package_ID");
                maxObj.name = xRow.ElementValue("Name");
                maxObj.alias = xRow.ElementValue("Alias");
                maxObj.notes = new MarkupType() { Text = new String[] { xRow.ElementValue("Notes") } };
                maxObj.stereotype = xRow.ElementValue("Stereotype");
                maxObj.type = ObjectTypeEnum.Package;
                if (mapObjectID2MAXID.ContainsKey(Package_Object_ID))
                {
                    maxObj.parentId = mapObjectID2MAXID[Package_Object_ID];
                }
                else
                {
                    maxObj.parentId = Package_Object_ID.ToString();
                }
                maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
                maxObj.modifiedSpecified = true;
                List<TagType> tags = new List<TagType>();
                foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Object_ID={0}]", Object_ID)))
                {
                    string tagName = xTV.ElementValue("Property");
                    if (!TV_MAX_ID.Equals(tagName))
                    {
                        TagType maxTag = new TagType();
                        maxTag.name = tagName;
                        maxTag.value = xTV.ElementValue("Value");
                        maxTag.Text = new String[] { xTV.ElementValue("Notes") };
                        tags.Add(maxTag);
                    }
                }
                mapPackageToObjectID[Sub_Package_ID] = Object_ID;
                maxObj.tag = tags.ToArray();
                objects.Add(maxObj);
                progress.step();

                visitPackage(Sub_Package_ID);
            }

            // visit objects of this package
            visitObjects(Package_ID);
        }

        // select objects with EA.Package.PackageID as ParentID and recurse on Object_Type = 'Package'
        // "ORDER BY TPos" returns in the order that the elements are in the Package Browser
        // TPos is only set if sort order is changed manually in the Package Browser
        // Use Object_ID for sort initial order
        private void visitObjects(int Package_ID)
        {
            int Package_Object_ID = mapPackageToObjectID[Package_ID];

            // get objects in selected package
            string sql = string.Format("SELECT Object_ID, ea_guid, Object_Type, Name, Alias, Package_ID, Stereotype, ModifiedDate, Abstract, Tagged, ea_guid, ParentID FROM t_object WHERE Package_ID={0} AND Object_Type<>'Package' ORDER BY Object_ID", Package_ID);
            string xml = Repository.SQLQuery(sql);
            XElement xEADATA = XElement.Parse(xml, LoadOptions.None);

            // get tagged values in objects in selected package
            string sql_tv = string.Format("SELECT op.Object_ID, op.Property, op.Value, op.Notes FROM t_object o, t_objectproperties op WHERE Package_ID={0} AND o.Object_ID=op.Object_ID", Package_ID);
            string xml_tv = Repository.SQLQuery(sql_tv);
            XElement xEADATA_tv = XElement.Parse(xml_tv, LoadOptions.None);

            // update map from internal id to MAX::ID
            foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Property='{0}']", TV_MAX_ID)))
            {
                int Object_ID = xTV.ElementValueInt("Object_ID");
                string MAX_ID = xTV.ElementValue("Value");
                mapObjectID2MAXID[Object_ID] = MAX_ID;
            }

            // create MAX object elements
            IEnumerable<XElement> xRows = xEADATA.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count<XElement>();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                ObjectType maxObj = new ObjectType();
                int Object_ID = xRow.ElementValueInt("Object_ID");
                if (mapObjectID2MAXID.ContainsKey(Object_ID))
                {
                    maxObj.id = mapObjectID2MAXID[Object_ID];
                }
                else
                {
                    maxObj.id = Object_ID.ToString();
                }
                if (xRow.ElementValueInt("Abstract") == 1)
                {
                    maxObj.isAbstract = true;
                    maxObj.isAbstractSpecified = true;
                }
                maxObj.name = xRow.ElementValue("Name");
                maxObj.alias = xRow.ElementValue("Alias");
                EA.Element el = Repository.GetElementByID(Object_ID); // <- Bummer. t_object.Note is not available through the EA SQL API???
                maxObj.notes = new MarkupType() { Text = new String[] { el.Notes } };
                maxObj.stereotype = xRow.ElementValue("Stereotype");
                maxObj.type = (ObjectTypeEnum)Enum.Parse(typeof(ObjectTypeEnum), xRow.ElementValue("Object_Type"), false);
                int EA_ParentID = int.Parse(xRow.ElementValue("ParentID"));
                // ParentID = 0 for direct childs of a Package, ParentID is only used within a Package
                int maxParentID = EA_ParentID;
                if (EA_ParentID == 0)
                {
                    maxParentID = Package_Object_ID;
                }
                if (mapObjectID2MAXID.ContainsKey(maxParentID))
                {
                    maxObj.parentId = mapObjectID2MAXID[maxParentID];
                }
                else
                {
                    maxObj.parentId = maxParentID.ToString();
                }
                maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
                maxObj.modifiedSpecified = true;
                List<TagType> tags = new List<TagType>();
                foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Object_ID={0}]", Object_ID)))
                {
                    string tagName = xTV.ElementValue("Property");
                    if (!TV_MAX_ID.Equals(tagName))
                    {
                        TagType maxTag = new TagType();
                        maxTag.name = tagName;
                        maxTag.value = xTV.ElementValue("Value");
                        maxTag.Text = new String[] { xTV.ElementValue("Notes") };
                        tags.Add(maxTag);
                    }
                }
                maxObj.tag = tags.ToArray();

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
                    if (xAtt.ElementValueInt("Const") == 1)
                    {
                        maxAtt.isReadOnly = true;
                        maxAtt.isReadOnlySpecified = true;
                    }
                    maxAtt.Text = new String[] { xAtt.ElementValue("Notes") };

                    // get attribute tagged values for selected object
                    string sql_att_tv = string.Format("SELECT * FROM t_attributetag WHERE ElementID={0}", Attribute_ID);
                    string xml_att_tv = Repository.SQLQuery(sql_att_tv);
                    XElement xEADATA_att_tv = XElement.Parse(xml_att_tv, LoadOptions.None);
                    List<TagType> attTags = new List<TagType>();
                    foreach (XElement xTV in xEADATA_att_tv.XPathSelectElements("//Data/Row"))
                    {
                        TagType maxTag = new TagType();
                        maxTag.name = xTV.ElementValue("Property");
                        maxTag.value = xTV.ElementValue("VALUE");
                        maxTag.Text = new String[] { xTV.ElementValue("NOTES") };
                        attTags.Add(maxTag);
                    }
                    maxAtt.tag = attTags.ToArray();
                    atts.Add(maxAtt);
                }
                maxObj.attribute = atts.ToArray();
                objects.Add(maxObj);
                progress.step();
            }

            // get relationships in objects in selected package
            string sql_con = string.Format("SELECT c.Connector_ID, c.Connector_Type, c.Name, c.Notes, c.Start_Object_ID, c.SourceCard, c.SourceRole, c.End_Object_ID, c.DestCard, c.DestRole, c.Stereotype FROM t_connector c, t_object WHERE Start_Object_ID=Object_ID AND Package_ID={0}", Package_ID);
            string xml_con = Repository.SQLQuery(sql_con);
            XElement xEADATA_con = XElement.Parse(xml_con, LoadOptions.None);

            // create MAX relationship elements
            xRows = xEADATA_con.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count<XElement>();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                RelationshipType maxRel = new RelationshipType();
                //maxRel.id = xRow.ElementValueInt("Connector_ID").ToString();
                maxRel.label = xRow.ElementValue("Name");
                int sourceId = xRow.ElementValueInt("Start_Object_ID");
                if (mapObjectID2MAXID.ContainsKey(sourceId))
                {
                    maxRel.sourceId = mapObjectID2MAXID[sourceId];
                }
                else
                {
                    maxRel.sourceId = sourceId.ToString();
                }
                maxRel.sourceCard = xRow.ElementValue("SourceCard");
                maxRel.sourceLabel = xRow.ElementValue("SourceRole");
                int destId = xRow.ElementValueInt("End_Object_ID");
                if (mapObjectID2MAXID.ContainsKey(destId))
                {
                    maxRel.destId = mapObjectID2MAXID[destId];
                }
                else
                {
                    maxRel.destId = destId.ToString();
                }
                maxRel.destCard = xRow.ElementValue("DestCard");
                maxRel.destLabel = xRow.ElementValue("DestRole");
                maxRel.notes = new MarkupType() { Text = new String[] { xRow.ElementValue("Notes") } };
                maxRel.stereotype = xRow.ElementValue("Stereotype");
                maxRel.type = (RelationshipTypeEnum)Enum.Parse(typeof(RelationshipTypeEnum), xRow.ElementValue("Connector_Type"), false);
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
    }
}
