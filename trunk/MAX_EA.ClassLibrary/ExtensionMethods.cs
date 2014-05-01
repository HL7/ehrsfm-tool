using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MAX_EA
{
    public static class XElementExtensionMethods
    {
        // Extend XElement with a method that gets the value or null of an element and checks if the element exists
        public static string ElementValue(this XElement xElement, string name, string defaultValue = null)
        {
            string value = defaultValue;
            XElement xChildElement = xElement.Element(name);
            if (xChildElement != null)
            {
                value = xChildElement.Value;
            }
            return value;
        }

        public static int ElementValueInt(this XElement xElement, string name, int defaultValue = -1)
        {
            string value = ElementValue(xElement, name);
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            else
            {
                return int.Parse(value);
            }
        }

        public static DateTime ElementValueDateTime(this XElement xElement, string name)
        {
            DateTime result;
            try
            {
                result = DateTime.Parse(ElementValue(xElement, name), CultureInfo.CurrentCulture.DateTimeFormat);
            }
            catch (FormatException)
            {
                result = DateTime.Parse(ElementValue(xElement, name), CultureInfo.CurrentUICulture.DateTimeFormat);
            }
            return result;
        }
    }
}
