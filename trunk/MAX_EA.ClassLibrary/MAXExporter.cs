using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;

namespace MAX_EA
{
    public class MAXExporter : Util
    {
        XNamespace max = XNamespace.Get("http://www.umcg.nl/MAX");
        EA.Repository Repository;
        string fileName;

        public void export(EA.Repository Repository)
        {
            this.Repository = Repository;

            EA.ObjectType type = Repository.GetTreeSelectedItemType();
            if (type == EA.ObjectType.otDiagram || type == EA.ObjectType.otPackage)
            {
                fileName = showFileDialog("Select output MAX XML file", "xml files (*.xml)|*.xml", @"D:\VisualStudio Projects\HL7\MAX_EA.ClassLibrary\Output\MAX-example.xml", false);
                if (fileName == String.Empty) return;

                System.Threading.Thread thread = new System.Threading.Thread(new System.Threading.ThreadStart(_export));
                thread.Start();
            }
            // else popup; unsupported selection
        }

        private void _export()
        {
            Repository.EnableUIUpdates = true;
            Repository.EnableCache = true;

            ProgressWindow window = new ProgressWindow();
            window.Show();
            window.Refresh();

            EA.ObjectType type = Repository.GetTreeSelectedItemType();

            XElement xModel = new XElement(max + "model",
                new XAttribute(XNamespace.Xmlns + "max", "http://www.umcg.nl/MAX")); // make sure prefix 'max' is used for MAX!
            XElement xObjects = new XElement("objects");
            xModel.Add(xObjects);

            Dictionary<int, EA.Element> elements = new Dictionary<int, EA.Element>();

            if (type == EA.ObjectType.otDiagram)
            {
                // do the elements in the diagram
                EA.Diagram diagram = (EA.Diagram)Repository.GetTreeSelectedObject();
                foreach (EA.DiagramObject dobj in diagram.DiagramObjects)
                {
                    int elId = dobj.ElementID;
                    EA.Element el = Repository.GetElementByID(elId);
                    elements[elId] = el;
                }
            }
            else if (type == EA.ObjectType.otPackage)
            {
                EA.Package package = (EA.Package)Repository.GetTreeSelectedObject();

                // Add/update export metadata to the model
                EA.TaggedValue tvExportDate = (EA.TaggedValue)package.Element.TaggedValues.GetByName("MAX::LastExportDate");
                if (tvExportDate == null)
                {
                    tvExportDate = (EA.TaggedValue)package.Element.TaggedValues.AddNew("MAX::LastExportDate", "");
                }
                tvExportDate.Value = DateTime.Now.ToString();
                tvExportDate.Update();
                package.Element.TaggedValues.Refresh();

                // count the packages and elements and hierarchy
                // TODO: also include Packages
                recursePackage(elements, package);
            }
            window.setup(elements.Count * 2);

            // now create xObject elements from EA.Elements
            foreach (EA.Element el in elements.Values)
            {
                XElement xObject = new XElement("object");
                xObjects.Add(xObject);
                xObject.Add(new XElement("id", el.ElementID)); // or use Alias as ID?
                xObject.Add(new XElement("name", el.Name));
                if (!string.IsNullOrEmpty(el.Alias))
                {
                    xObject.Add(new XElement("alias", el.Alias));
                }
                if (!string.IsNullOrEmpty(el.Notes))
                {
                    xObject.Add(new XElement("notes", el.Notes));
                }
                if (!string.IsNullOrEmpty(el.Stereotype))
                {
                    xObject.Add(new XElement("stereotype", el.Stereotype));
                }
                xObject.Add(new XElement("type", el.Type));
                if (el.ParentID != 0)
                {
                    if (elements.ContainsKey(el.ParentID))
                    {
                        xObject.Add(new XElement("parentId", el.ParentID));
                    }
                }
                else if (el.PackageID != 0)
                {
                    EA.Package parentPackage = Repository.GetPackageByID(el.PackageID);
                    int parentElementID = parentPackage.Element.ElementID;
                    if (elements.ContainsKey(parentElementID))
                    {
                        xObject.Add(new XElement("parentId", parentElementID));
                    }
                }
                xObject.Add(new XElement("modified", el.Modified));

                // handle Element tagged values
                foreach (EA.TaggedValue tv in el.TaggedValues)
                {
                    XElement xTag = new XElement("tag", new XAttribute("name", tv.Name), new XAttribute("value", tv.Value));
                    xObject.Add(xTag);
                    if (!string.IsNullOrEmpty(tv.Notes))
                    {
                        xTag.Value = tv.Notes;
                    }
                }

                // handle Element attributes
                foreach (EA.Attribute att in el.Attributes)
                {
                    XElement xAttribute = new XElement("attribute");
                    xObjects.Add(xAttribute);
                    xAttribute.Add(new XElement("id", att.AttributeID));
                    xAttribute.Add(new XElement("name", att.Name));
                    if (!string.IsNullOrEmpty(att.Notes))
                    {
                        xAttribute.Add(new XElement("notes", att.Notes));
                    }
                    if (!string.IsNullOrEmpty(att.Stereotype))
                    {
                        xAttribute.Add(new XElement("stereotype", att.Stereotype));
                    }
                    xAttribute.Add(new XElement("type", att.Type));
                    xAttribute.Add(new XElement("parentId", el.ElementID));
                }
                window.step();
            }

            XElement xRelationships = new XElement("relationships");
            xModel.Add(xRelationships);

            // now handle the relationships between elements
            List<string> graphs = new List<string>();
            foreach (int elId in elements.Keys)
            {
                EA.Element el = elements[elId];
                string name = el.Name;
                foreach (EA.Connector con in el.Connectors)
                {
                    // Don't know why, but there are a lot of connector to self
                    if (elId != con.SupplierID && elements.ContainsKey(con.SupplierID))
                    {
                        EA.Element el2 = elements[con.SupplierID];
                        string stereotype = con.Stereotype;

                        XElement xRelationship = new XElement("relationship");
                        xRelationships.Add(xRelationship);
                        xRelationship.Add(new XElement("id", con.ConnectorID)); // or use Alias?
                        if (!string.IsNullOrEmpty(con.Name))
                        {
                            xRelationship.Add(new XElement("label", con.Name));
                        }
                        xRelationship.Add(new XElement("sourceId", el.ElementID)); // or use Alias?
                        xRelationship.Add(new XElement("destId", el2.ElementID)); // or use Alias?
                        if (!string.IsNullOrEmpty(con.Notes))
                        {
                            xRelationship.Add(new XElement("notes", con.Notes));
                        }
                        if (!string.IsNullOrEmpty(con.Stereotype))
                        {
                            xRelationship.Add(new XElement("stereotype", con.Stereotype));
                        }
                        xRelationship.Add(new XElement("type", con.Type));
                    }
                }
                window.step();
            }
            xModel.Save(fileName);
            window.Close();
            Repository.EnableCache = false;
        }

        private void recursePackage(Dictionary<int, EA.Element> elements, EA.Package package)
        {
            elements[package.Element.ElementID] = package.Element;
            foreach (EA.Package childPackage in package.Packages)
            {
                recursePackage(elements, childPackage);
            }
            foreach (EA.Element element in package.Elements)
            {
                recurseElement(elements, element);
            }
        }

        private void recurseElement(Dictionary<int, EA.Element> elements, EA.Element element)
        {
            elements[element.ElementID] = element;
            foreach (EA.Element childElement in element.Elements)
            {
                recurseElement(elements, childElement);
            }
        }
    }
}
