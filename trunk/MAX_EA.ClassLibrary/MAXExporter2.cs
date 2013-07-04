using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MAX_EA
{
    public class MAXExporter2 : Util
    {
        ProgressWindow progress = new ProgressWindow();
        bool useGUID = false;
        int progress_max;
        XElement xModel, xObjects, xRelationships;
        EA.Repository Repository;
        Dictionary<int, int> mapPackageToObjectID = new Dictionary<int, int>();

        public void export(EA.Repository Repository, bool useGUID)
        {
            this.Repository = Repository;
            string fileName = showFileDialog("Select output MAX XML file", "xml files (*.xml)|*.xml", @"D:\VisualStudio Projects\HL7\MAX_EA.ClassLibrary\Output\MAX-example.xml", false);
            if (fileName != String.Empty)
            {
                export(Repository, useGUID, fileName);
            }
        }

        public void export(EA.Repository Repository, bool useGUID, string fileName)
        {
            this.Repository = Repository;
            this.useGUID = useGUID;
            EA.ObjectType type = Repository.GetTreeSelectedItemType();
            if (type == EA.ObjectType.otPackage)
            {
                progress_max = 1;
                progress.setup(progress_max);
                progress.Show();
                progress.Refresh();

                XNamespace max = XNamespace.Get("http://www.umcg.nl/MAX");
                xModel = new XElement(max + "model",
                    new XAttribute(XNamespace.Xmlns + "max", "http://www.umcg.nl/MAX")); // make sure prefix 'max' is used for MAX!
                xObjects = new XElement("objects");
                xRelationships = new XElement("relationships");
                xModel.Add(xObjects);
                xModel.Add(xRelationships);

                Repository.EnableCache = true;
                EA.Package package = Repository.GetTreeSelectedPackage();
                // Add/update export metadata to the model
                xModel.Add(new XAttribute("exportDate", DateTime.Now.ToString()));
                // Now export selected package
                visitSelectedPackage(package.PackageID);
                progress.Close();

                xModel.Save(fileName);
            }
        }

        private void visitSelectedPackage(int Package_ID)
        {
            // select sub packages of selected package
            string sql = string.Format("SELECT p.Package_ID, p.ea_guid, p.Name, o.Alias, p.ModifiedDate, o.Stereotype, p.Notes, o.Object_ID FROM t_package p, t_object o WHERE p.Package_ID={0} AND p.ea_guid=o.ea_guid", Package_ID);
            string xml = Repository.SQLQuery(sql);
            XElement xEADATA = XElement.Parse(xml, LoadOptions.None);

            // TODO: Tagged Values

            XElement xRow = xEADATA.XPathSelectElement("//Data/Row");
            int Object_ID = xRow.ElementValueInt("Object_ID");
            MAXObject maxObj = new MAXObject();
            maxObj.id = Object_ID;
            maxObj.name = xRow.ElementValue("Name");
            maxObj.alias = xRow.ElementValue("Alias");
            maxObj.notes = xRow.ElementValue("Notes");
            maxObj.stereotype = xRow.ElementValue("Stereotype");
            maxObj.type = "Package";
            maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
            mapPackageToObjectID[Package_ID] = Object_ID;
            xObjects.Add(maxObj.asXML());

            visitPackage(Package_ID);
        }

        private void visitPackage(int Package_ID)
        {
            int Package_Object_ID = mapPackageToObjectID[Package_ID];

            // select sub packages of selected package
            string sql = string.Format("SELECT p.Package_ID, p.ea_guid, p.Name, o.Alias, p.ModifiedDate, o.Stereotype, p.Notes, o.Object_ID FROM t_package p, t_object o WHERE p.Parent_ID={0} AND o.Package_ID={1} AND p.ea_guid=o.ea_guid", Package_ID, Package_ID);
            string xml = Repository.SQLQuery(sql);
            XElement xEADATA = XElement.Parse(xml, LoadOptions.None);

            // get tagged values in objects in selected package
            string sql_tv = string.Format("SELECT op.Object_ID, op.Property, op.Value, op.Notes FROM t_object o, t_objectproperties op WHERE Package_ID={0} AND o.Object_ID=op.Object_ID", Package_ID);
            string xml_tv = Repository.SQLQuery(sql_tv);
            XElement xEADATA_tv = XElement.Parse(xml_tv, LoadOptions.None);

            IEnumerable<XElement> xRows = xEADATA.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count<XElement>();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                MAXObject maxObj = new MAXObject();
                maxObj.id = xRow.ElementValueInt("Object_ID");
                int Sub_Package_ID = xRow.ElementValueInt("Package_ID");
                maxObj.name = xRow.ElementValue("Name");
                maxObj.alias = xRow.ElementValue("Alias");
                maxObj.notes = xRow.ElementValue("Notes");
                maxObj.stereotype = xRow.ElementValue("Stereotype");
                maxObj.type = "Package";
                maxObj.parentId = Package_Object_ID;
                maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
                foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Object_ID={0}]", maxObj.id)))
                {
                    MAXTag maxTag = new MAXTag();
                    maxTag.name = xTV.ElementValue("Property");
                    maxTag.value = xTV.ElementValue("Value");
                    maxTag.notes = xTV.ElementValue("Notes");
                    maxObj.tags.Add(maxTag);
                }
                mapPackageToObjectID[Sub_Package_ID] = maxObj.id;
                xObjects.Add(maxObj.asXML());
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

            // create MAX object elements
            IEnumerable<XElement> xRows = xEADATA.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count<XElement>();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                int Object_ID = xRow.ElementValueInt("Object_ID");
                MAXObject maxObj = new MAXObject();
                maxObj.id = Object_ID;
                maxObj.name = xRow.ElementValue("Name");
                maxObj.alias = xRow.ElementValue("Alias");
                EA.Element el = Repository.GetElementByID(Object_ID); // <- Bummer. t_object.Note is not available through the EA SQL API???
                maxObj.notes = el.Notes;
                maxObj.stereotype = xRow.ElementValue("Stereotype");
                maxObj.type = xRow.ElementValue("Object_Type");
                int EA_ParentID = int.Parse(xRow.ElementValue("ParentID"));
                // ParentID = 0 for direct childs of a Package, ParentID is only used within a Package
                int maxParentID = EA_ParentID;
                if (EA_ParentID == 0)
                {
                    maxParentID = Package_Object_ID;
                }
                maxObj.parentId = maxParentID;
                maxObj.modified = xRow.ElementValueDateTime("ModifiedDate");
                foreach (XElement xTV in xEADATA_tv.XPathSelectElements(string.Format("//Data/Row[Object_ID={0}]", Object_ID)))
                {
                    MAXTag maxTag = new MAXTag();
                    maxTag.name = xTV.ElementValue("Property");
                    maxTag.value = xTV.ElementValue("Value");
                    maxTag.notes = xTV.ElementValue("Notes");
                    maxObj.tags.Add(maxTag);
                }
                // TODO: Attributes
                xObjects.Add(maxObj.asXML());
                progress.step();
            }

            // get relationships in objects in selected package
            string sql_con = string.Format("SELECT c.Connector_ID, c.Connector_Type, c.Name, c.Notes, c.Start_Object_ID, c.End_Object_ID, c.Stereotype FROM t_connector c, t_object WHERE Start_Object_ID=Object_ID AND Package_ID={0}", Package_ID);
            string xml_con = Repository.SQLQuery(sql_con);
            XElement xEADATA_con = XElement.Parse(xml_con, LoadOptions.None);

            // create MAX relationship elements
            xRows = xEADATA_con.XPathSelectElements("//Data/Row");
            progress_max += xRows.Count<XElement>();
            progress.setup(progress_max);
            foreach (XElement xRow in xRows)
            {
                MAXRelationship maxRel = new MAXRelationship();
                maxRel.id = xRow.ElementValueInt("Connector_ID");
                maxRel.label = xRow.ElementValue("Name");
                maxRel.sourceId = xRow.ElementValueInt("Start_Object_ID");
                maxRel.destId = xRow.ElementValueInt("End_Object_ID");
                maxRel.notes = xRow.ElementValue("Notes");
                maxRel.stereotype = xRow.ElementValue("Stereotype");
                maxRel.type = xRow.ElementValue("Connector_Type");

                xRelationships.Add(maxRel.asXML());
                progress.step();
            }
        }
    }
}
