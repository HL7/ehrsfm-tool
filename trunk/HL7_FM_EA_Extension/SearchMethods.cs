using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace HL7_FM_EA_Extension
{
    class SearchMethods
    {
        public XElement FindNonSHALL(EA.Repository Repository, string SearchText)
        {
            XElement xResults = new XElement("ReportViewData");

            xResults.Add(new XElement("Fields",
                new XElement("Field", new XAttribute("name", "CLASSGUID")),
                new XElement("Field", new XAttribute("name", "CLASSTYPE")),
                new XElement("Field", new XAttribute("name", "Name")),
                new XElement("Field", new XAttribute("name", "Type")),
                new XElement("Field", new XAttribute("name", "Optionality")),
                new XElement("Field", new XAttribute("name", "Dependent")),
                new XElement("Field", new XAttribute("name", "Conditional"))
                ));

            XElement xRows = new XElement("Rows");
            xResults.Add(xRows);

            switch (Repository.GetTreeSelectedItemType())
            {
                case EA.ObjectType.otPackage:
                    EA.Package package = Repository.GetTreeSelectedPackage();
                    visitPackage(package).ForEach(xRow => xRows.Add(xRow));
                    break;
                case EA.ObjectType.otElement:
                    EA.Element element = (EA.Element)Repository.GetTreeSelectedObject();
                    visitElement(element).ForEach(xRow => xRows.Add(xRow));
                    break;
            }

            return xResults;
        }

        private List<XElement> visitPackage(EA.Package package)
        {
            List<XElement> xRows = new List<XElement>();
            foreach (EA.Package childPackage in package.Packages)
            {
                visitPackage(childPackage).ForEach(xRow => xRows.Add(xRow));
            }
            foreach (EA.Element element in package.Elements)
            {
                visitElement(element).ForEach(xRow => xRows.Add(xRow));
            }
            return xRows;
        }

        private List<XElement> visitElement(EA.Element element)
        {
            List<XElement> xRows = new List<XElement>();
            EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(R2Const.TV_OPTIONALITY);
            if (tv != null)
            {
                if (!"SHALL".Equals(tv.Value))
                {
                    string valName = string.Format("{0} {1}", element.Name, element.Notes);
                    string valType = element.Stereotype;
                    string valOptionality = tv.Value;
                    string valDependent = ((EA.TaggedValue)element.TaggedValues.GetByName(R2Const.TV_DEPENDENT)).Value;
                    string valConditional = ((EA.TaggedValue)element.TaggedValues.GetByName(R2Const.TV_CONDITIONAL)).Value;

                    xRows.Add(new XElement("Row",
                        new XElement("Field", new XAttribute("name", "CLASSGUID"), new XAttribute("value", element.ElementGUID)),
                        new XElement("Field", new XAttribute("name", "CLASSTYPE"), new XAttribute("value", element.Type)),
                        new XElement("Field", new XAttribute("name", "Name"), new XAttribute("value", valName)),
                        new XElement("Field", new XAttribute("name", "Type"), new XAttribute("value", valType)),
                        new XElement("Field", new XAttribute("name", "Optionality"), new XAttribute("value", valOptionality)),
                        new XElement("Field", new XAttribute("name", "Dependent"), new XAttribute("value", valDependent)),
                        new XElement("Field", new XAttribute("name", "Conditional"), new XAttribute("value", valConditional))
                        ));
                }
            }
            else
            {
                xRows.Add(new XElement("Row",
                    new XElement("Field", new XAttribute("name", "CLASSGUID"), new XAttribute("value", element.ElementGUID)),
                    new XElement("Field", new XAttribute("name", "CLASSTYPE"), new XAttribute("value", element.Type)),
                    new XElement("Field", new XAttribute("name", "Name"), new XAttribute("value", element.Name)),
                    new XElement("Field", new XAttribute("name", "Type"), new XAttribute("value", element.Stereotype))
                    ));

                // First the children that have no children
                // then children that do have children
                foreach (EA.Element childElement in element.Elements)
                {
                    if (childElement.Elements.Count == 0)
                    {
                        visitElement(childElement).ForEach(xRow => xRows.Add(xRow));
                    }
                }
                foreach (EA.Element childElement in element.Elements)
                {
                    if (childElement.Elements.Count > 0)
                    {
                        visitElement(childElement).ForEach(xRow => xRows.Add(xRow));
                    }
                }
                // If there are no childs, clear it
                // This makes empty Header/Functions disappear from the results
                if (xRows.Count == 1)
                {
                    xRows.Clear();
                }
            }
            return xRows;
        }
    }
}
