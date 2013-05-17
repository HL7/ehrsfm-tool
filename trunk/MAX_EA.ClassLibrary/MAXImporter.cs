using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;
using System.Xml;

namespace MAX_EA
{
    public class MAXImporter : Util
    {
        public void import(EA.Repository Repository, EA.Package selectedPackage)
        {
            string fileName = showFileDialog("Select input MAX XML file", "xml files (*.xml)|*.xml", @"D:\VisualStudio Projects\HL7\MAX_EA.ClassLibrary\Input\MAX-example.xml", true);
            if (fileName == String.Empty) return;

            ProgressWindow window = new ProgressWindow();
            window.Show();
            window.Refresh();

            string sourceXmlName = fileName.Substring(fileName.LastIndexOf('\\') + 1);

            Repository.EnableUIUpdates = false;
            Repository.BatchAppend = true;

            XmlReader xReader = XmlReader.Create(fileName);
            // make sure file gets closed
            using (xReader)
            {
                XElement xModel = XElement.Load(xReader);
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(xReader.NameTable);
                nsmgr.AddNamespace("max", "http://www.umcg.nl/MAX");

                IEnumerable<XElement> xObjectElements = xModel.XPathSelectElements("//objects/object", nsmgr);
                IEnumerable<XElement> xRelationshipElements = xModel.XPathSelectElements("//relationships/relationship", nsmgr);

                window.setup(xObjectElements.Count() + xRelationshipElements.Count());

                // Don't use Linq constructs those are reeeeaaalllyyyy slow, create a helper dictionary for quick lookup
                // This construct also makes sure there will be unique elements based on the Alias|ID
                Dictionary<string, EA.Element> eaElementDict = new Dictionary<string, EA.Element>();
                Dictionary<string, EA.Package> eaPackageDict = new Dictionary<string, EA.Package>();
                recurseEaPackage(selectedPackage, eaElementDict, eaPackageDict);

                // Overwrite only if new attribute is there and not an empty string
                int objPos = 0, pkgPos = 0;
                foreach (XElement xObject in xObjectElements)
                {
                    string type = "Class";
                    XElement xType = xObject.XPathSelectElement("type", nsmgr);
                    if (xType != null && !string.IsNullOrEmpty(xType.Value))
                    {
                        type = xType.Value;
                    }
                    string id = xObject.XPathSelectElement("id", nsmgr).Value.Trim().ToUpper();

                    // first check if element already in package
                    // if not create otherwise use existing and update
                    string name = xObject.XPathSelectElement("name", nsmgr).Value;
                    EA.Element eaElement;
                    if (eaElementDict.ContainsKey(id))
                    {
                        eaElement = eaElementDict[id];
                        eaElement.Name = name;
                        eaElement.Type = type;
                    }
                    else
                    {
                        XElement xParentId = xObject.XPathSelectElement("parentId", nsmgr);
                        if ("Package".Equals(type))
                        {
                            EA.Package eaPackage;
                            if (xParentId != null)
                            {
                                string parentId = xParentId.Value.Trim();
                                eaPackage = (EA.Package)eaPackageDict[parentId].Packages.AddNew(name, type);
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
                            if (xParentId != null)
                            {
                                string parentId = xParentId.Value.Trim();
                                EA.Element parentElement = eaElementDict[parentId];
                                if ("Package".Equals(parentElement.Type))
                                {
                                    EA.Package parentPackage = eaPackageDict[parentId];
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
                        eaElement.Alias = id;
                        eaElementDict[id] = eaElement;
                    }

                    XElement xStereotype = xObject.XPathSelectElement("stereotype", nsmgr);
                    if (xStereotype != null)
                    {
                        eaElement.Stereotype = xStereotype.Value.Trim();
                    }

                    XElement xNotes = xObject.XPathSelectElement("notes", nsmgr);
                    if (xNotes != null)
                    {
                        eaElement.Notes = xNotes.Value.Trim().Replace("\n", "\r\n");
                    }
                    eaElement.Update();

                    XElement xModified = xObject.XPathSelectElement("modified", nsmgr);
                    if (xModified != null)
                    {
                        eaElement.Modified = (DateTime)xModified; // explicit convertion of the value to a DateTime
                    }

                    foreach (XElement xTag in xObject.XPathSelectElements("tag"))
                    {
                        string tagName = xTag.Attribute("name").Value.Trim();
                        EA.TaggedValue tv = (EA.TaggedValue)eaElement.TaggedValues.GetByName(tagName);
                        if (tv == null)
                        {
                            tv = (EA.TaggedValue)eaElement.TaggedValues.AddNew(tagName, "TaggedValue");
                        }
                        XAttribute xTagValue = xTag.Attribute("value");
                        if (xTagValue != null)
                        {
                            tv.Value = xTagValue.Value;
                        }
                        if (!string.IsNullOrEmpty(xTag.Value))
                        {
                            tv.Notes = xTag.Value.Trim().Replace("\n", "\r\n");
                        }
                        tv.Update();
                    }
                    int attPos = 0;
                    foreach (XElement xAtt in xObject.XPathSelectElements("attribute"))
                    {
                        string attName = xAtt.Attribute("name").Value.Trim();
                        EA.Attribute att = (EA.Attribute)eaElement.Attributes.GetByName(attName);
                        if (att == null)
                        {
                            att = (EA.Attribute)eaElement.Attributes.AddNew(attName, "");
                        }
                        att.Pos = attPos++;
                        XAttribute xAttType = xAtt.Attribute("type");
                        if (xAttType != null)
                        {
                            att.Type = xAttType.Value;
                        }
                        XAttribute xAttValue = xAtt.Attribute("value");
                        if (xAttValue != null)
                        {
                            att.Default = xAttValue.Value;
                        }
                        if (!string.IsNullOrEmpty(xAtt.Value))
                        {
                            att.Notes = xAtt.Value.Trim().Replace("\n", "\r\n");
                        }
                        XAttribute xAttStereotype = xAtt.Attribute("stereotype");
                        if (xAttStereotype != null)
                        {
                            att.Stereotype = xAttStereotype.Value;
                        }
                        att.Update();
                    }
                    window.step();

                    // add a TaggedValue sourceXmlName for traceability
                    if (eaElement.TaggedValues.GetByName(sourceXmlName) == null)
                    {
                        ((EA.TaggedValue)eaElement.TaggedValues.AddNew(sourceXmlName, "")).Update();
                    }
                    eaElement.TaggedValues.Refresh();
                }

                // now do connectors
                // don't know how to update this, just clear connectors and recreate if the import file has relationships otherwise ignore relationships
                if (xRelationshipElements.Count() > 0)
                {
                    foreach (EA.Element eaElement in selectedPackage.Elements)
                    {
                        for (int c = eaElement.Connectors.Count - 1; c >= 0; c--)
                        {
                            eaElement.Connectors.Delete((short)c);
                        }
                        eaElement.Update();
                    }

                    foreach (XElement xRel in xRelationshipElements)
                    {
                        string sourceId = xRel.XPathSelectElement("sourceId", nsmgr).Value.Trim().ToUpper();
                        EA.Element eaSourceElement;
                        if (!eaElementDict.ContainsKey(sourceId))
                        {
                            // create placeholder object; missing in objects list??
                            eaSourceElement = (EA.Element)selectedPackage.Elements.AddNew(string.Format("_{0}", sourceId), "Class");
                            eaSourceElement.Abstract = "1";
                            eaSourceElement.Alias = sourceId;
                            eaSourceElement.Update();
                            eaElementDict[sourceId] = eaSourceElement;
                        }
                        else
                        {
                            eaSourceElement = eaElementDict[sourceId];
                        }

                        string destId = xRel.XPathSelectElement("destId", nsmgr).Value.Trim().ToUpper();
                        EA.Element eaDestElement;
                        if (!eaElementDict.ContainsKey(destId))
                        {
                            // create placeholder object; missing in objects list??
                            eaDestElement = (EA.Element)selectedPackage.Elements.AddNew(string.Format("_{0}", destId), "Class");
                            eaDestElement.Abstract = "1";
                            eaDestElement.Alias = destId;
                            eaDestElement.Update();
                            eaElementDict[destId] = eaDestElement;
                        }
                        else
                        {
                            eaDestElement = eaElementDict[destId];
                        }

                        string type = "Association";
                        XElement xType = xRel.XPathSelectElement("type", nsmgr);
                        if (xType != null && !string.IsNullOrEmpty(xType.Value))
                        {
                            if ("Composition".Equals(xType.Value))
                            {
                                type = "Aggregation";
                            }
                            type = xType.Value;
                        }
                        string id = "";
                        XElement xId = xRel.XPathSelectElement("id", nsmgr);
                        if (xId != null)
                        {
                            id = xId.Value.Trim();
                        }
                        string label = "";
                        XElement xLabel = xRel.XPathSelectElement("label", nsmgr);
                        if (xLabel != null)
                        {
                            label = xLabel.Value.Trim();
                        }
                        EA.Connector eaCon = (EA.Connector)eaSourceElement.Connectors.AddNew(label, type);
                        eaCon.Alias = id;
                        eaCon.SupplierID = eaDestElement.ElementID;
                        if ("Composition".Equals(xType.Value))
                        {
                            eaCon.SupplierEnd.Aggregation = 2;
                        }

                        XElement xStereotype = xRel.XPathSelectElement("stereotype", nsmgr);
                        if (xStereotype != null)
                        {
                            eaCon.Stereotype = xStereotype.Value.Trim();
                        }

                        XElement xNotes = xRel.XPathSelectElement("notes", nsmgr);
                        if (xNotes != null)
                        {
                            eaCon.Notes = xNotes.Value.Trim().Replace("\n", "\r\n");
                        }
                        eaCon.Update();
                        window.step();
                    }
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
        }

        private void recurseEaPackage(EA.Package eaPackage, Dictionary<string, EA.Element> eaElementDict, Dictionary<string, EA.Package> eaPackageDict)
        {
            if (!string.IsNullOrEmpty(eaPackage.Alias))
            {
                eaPackageDict[eaPackage.Alias.ToUpper()] = eaPackage;
            }
            eaPackageDict[eaPackage.ParentID.ToString()] = eaPackage;

            foreach (EA.Package eaSubPackage in eaPackage.Packages)
            {
                if (!string.IsNullOrEmpty(eaSubPackage.Alias))
                {
                    eaElementDict[eaSubPackage.Alias.ToUpper()] = eaSubPackage.Element;
                }
                eaElementDict[eaSubPackage.ParentID.ToString()] = eaSubPackage.Element;
                recurseEaPackage(eaSubPackage, eaElementDict, eaPackageDict);
            }
            foreach (EA.Element eaElement in eaPackage.Elements)
            {
                if (!string.IsNullOrEmpty(eaElement.Alias))
                {
                    eaElementDict[eaElement.Alias.ToUpper()] = eaElement;
                }
                eaElementDict[eaElement.ElementID.ToString()] = eaElement;
                recurseEaElements(eaElement, eaElementDict);
            }
        }

        private void recurseEaElements(EA.Element eaElement, Dictionary<string, EA.Element> eaElementDict)
        {
            foreach (EA.Element eaChildElement in eaElement.Elements)
            {
                if (!string.IsNullOrEmpty(eaChildElement.Alias))
                {
                    eaElementDict[eaChildElement.Alias.ToUpper()] = eaChildElement;
                }
                eaElementDict[eaChildElement.ElementID.ToString()] = eaChildElement;
                recurseEaElements(eaChildElement, eaElementDict);
            }
        }
    }
}
