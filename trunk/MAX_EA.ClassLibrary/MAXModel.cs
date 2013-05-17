using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MAX_EA
{
    class MAXBase
    {
        // Only for Package and Element, not needed for Tagged Values, since you cannot use formatting there
        protected void addUnescapedNotes(XElement xEl, string notes)
        {
            if (!string.IsNullOrEmpty(notes))
            {
                // Make sure notes are xml-unescaped. TODO: Add xhtml namespace? And how?
                XElement xNotes = XElement.Parse(string.Format("<notes>{0}</notes>", notes.Replace("&", "&amp;")));
                //XElement xNotes = new XElement("notes", notes);
                xEl.Add(xNotes);
            }
        }
    }

    class MAXObject : MAXBase
    {
        public int id;
        public string name;
        public string alias;
        public string notes;
        public string stereotype;
        public string type;
        public int? parentId;
        public DateTime modified;
        public List<MAXTag> tags = new List<MAXTag>();

        public XElement asXML()
        {
            XElement xObject = new XElement("object");
            xObject.Add(new XElement("id", id));
            xObject.Add(new XElement("name", name));
            if (!string.IsNullOrEmpty(alias))
            {
                xObject.Add(new XElement("alias", alias));
            }
            addUnescapedNotes(xObject, notes);
            if (!string.IsNullOrEmpty(stereotype))
            {
                xObject.Add(new XElement("stereotype", stereotype));
            }
            xObject.Add(new XElement("type", type));
            if (parentId != null)
            {
                xObject.Add(new XElement("parentId", parentId));
            }
            xObject.Add(new XElement("modified", modified));
            foreach (MAXTag maxTag in tags)
            {
                xObject.Add(maxTag.asXML());
            }
            return xObject;
        }
    }

    class MAXTag : MAXBase
    {
        public string name;
        public string value;
        public string notes;

        public XElement asXML()
        {
            XElement xTag = new XElement("tag", new XAttribute("name", name));
            if (value != null)
            {
                xTag.Add(new XAttribute("value", value));
            }

            if (!string.IsNullOrEmpty(notes))
            {
                xTag.Value = notes;
            }
            return xTag;
        }
    }

    class MAXRelationship : MAXBase
    {
        public int id;
        public string label;
        public int sourceId;
        public int destId;
        public string notes;
        public string stereotype;
        public string type;

        public XElement asXML()
        {
            XElement xRelationship = new XElement("relationship");
            xRelationship.Add(new XElement("id", id));
            if (!string.IsNullOrEmpty(label))
            {
                xRelationship.Add(new XElement("label", label));
            }
            xRelationship.Add(new XElement("sourceId", sourceId));
            xRelationship.Add(new XElement("destId", destId));
            addUnescapedNotes(xRelationship, notes);
            if (!string.IsNullOrEmpty(stereotype))
            {
                xRelationship.Add(new XElement("stereotype", stereotype));
            }
            xRelationship.Add(new XElement("type", type));
            return xRelationship;
        }
    }
}
