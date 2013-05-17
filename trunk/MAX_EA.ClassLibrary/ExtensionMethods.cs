using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MAX_EA
{
    public static class XElementExtensionMethods
    {
        // Extend XElement with a method that gets the value or null of an element and checks if the element exists
        public static string ElementValue(this XElement xElement, string name)
        {
            string value = null;
            XElement xChildElement = xElement.Element(name);
            if (xChildElement != null)
            {
                value = xChildElement.Value;
            }
            return value;
        }

        public static int ElementValueInt(this XElement xElement, string name)
        {
            return int.Parse(ElementValue(xElement, name));
        }

        public static DateTime ElementValueDateTime(this XElement xElement, string name)
        {
            return DateTime.Parse(ElementValue(xElement, name));
        }
    }
}
