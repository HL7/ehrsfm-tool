using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HL7_FM_EA_Extension.R2ModelV2.Base;

namespace HL7_FM_EA_Extension
{
    class EAHelper : OutputListener
    {
        public static EA.Repository repository { get; set; }

        public static void LogMessage(string message, int id = -1)
        {
            string timestamp = DateTime.Now.ToString();
            repository.WriteOutput(Properties.Resources.OUTPUT_TAB_HL7_FM, string.Format("@{0} {1}", timestamp, message), id);
        }

        public static void SetTaggedValue(EA.Element element, string name, string value, string notes=null)
        {
            DeleteTaggedValue(element, name);
            EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.AddNew(name, "TaggedValue");
            tv.Value = value;
            if (notes != null)
            {
                tv.Notes = notes;
            }
            tv.Update();
        }

        public static string GetTaggedValue(EA.Element element, string name, string defaultValue = "")
        {
            EA.TaggedValue tv = element.TaggedValues.Cast<EA.TaggedValue>().FirstOrDefault(t => name.Equals(t.Name) && !string.IsNullOrEmpty(t.Value));
            if (tv != null)
            {
                return tv.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        public static string GetTaggedValueNotes(EA.Element element, string name, string defaultValue = "")
        {
            EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(name);
            if (tv != null)
            {
                return tv.Notes;
            }
            else
            {
                return defaultValue;
            }
        }

        /*
         * Delete all tagged values with <name>.
         */
        public static void DeleteTaggedValue(EA.Element element, string name)
        {
            for (short index = 0; index < element.TaggedValues.Count; index++)
            {
                EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetAt(index);
                if (name.Equals(tv.Name))
                {
                    element.TaggedValues.Delete(index);
                }
            }
            element.TaggedValues.Refresh();
        }

        // Last argument is ID of Element (used for double click!)
        public void writeOutput(string format, params object[] arg)
        {
            int ID = -1;
            object lastArg = arg[arg.Count() - 1];
            if (lastArg is int)
            {
                ID = (int) lastArg;
            }
            LogMessage(string.Format(format, arg), ID);
        }

        public static string getAssociatedBaseModelName(EA.Repository Repository, EA.Package ProfileDefinitionPackage)
        {
            EA.Package baseModelPackage = getAssociatedBaseModel(Repository, ProfileDefinitionPackage);
            if (baseModelPackage != null)
            {
                return baseModelPackage.Name;
            }
            else
            {
                return "No base model defined or linked...";
            }
        }

        public static EA.Package getAssociatedProfileDefinition(EA.Repository repository, EA.Package selectedSectionPackage)
        {
            EA.Package baseModel = repository.GetPackageByID(selectedSectionPackage.ParentID);
            EA.Connector con = baseModel.Connectors.Cast<EA.Connector>().SingleOrDefault(t => R2Const.ST_BASEMODEL.Equals(t.Stereotype) || "Usage".Equals(t.Type));
            if (con != null)
            {
                EA.Element packageElement = repository.GetElementByID(con.ClientID);
                if (R2Const.ST_FM_PROFILEDEFINITION.Equals(packageElement.Stereotype))
                {
                    // con.ClientID is the ElementID of the PackageElement
                    // Find the Package with the PackageElement by selecting the child Package in the parent Package where
                    // the ElementID is con.ClientID
                    return repository.GetPackageByID(packageElement.PackageID).Packages.Cast<EA.Package>().Single(p => p.Element.ElementID == con.ClientID);
                }
            }
            EAHelper.LogMessage("Expected <use> relationship to a <HL7-FM> Package == Base Model");
            return null;
        }

        public static EA.Package getAssociatedBaseModel(EA.Repository Repository, EA.Package ProfileDefinitionPackage)
        {
            EA.Connector con = ProfileDefinitionPackage.Connectors.Cast<EA.Connector>().SingleOrDefault(t => R2Const.ST_BASEMODEL.Equals(t.Stereotype) || "Usage".Equals(t.Type));
            if (con != null)
            {
                EA.Element packageElement = Repository.GetElementByID(con.SupplierID);
                if (R2Const.ST_FM.Equals(packageElement.Stereotype) || R2Const.ST_FM_PROFILE.Equals(packageElement.Stereotype))
                {
                    // con.SupplierID is the ElementID of the PackageElement
                    // Find the Package with the PackageElement by selecting the child Package in the parent Package where
                    // the ElementID is con.SupplierID
                    return Repository.GetPackageByID(packageElement.PackageID).Packages.Cast<EA.Package>().Single(p => p.Element.ElementID == con.SupplierID);
                }
            }
            EAHelper.LogMessage("Expected <use> relationship to a <HL7-FM> Package == Base Model");
            return null;
        }

        public static EA.Package getAssociatedOutputProfile(EA.Repository Repository, EA.Package ProfileDefinitionPackage)
        {
            EA.Connector con = ProfileDefinitionPackage.Connectors.Cast<EA.Connector>().SingleOrDefault(t => R2Const.ST_TARGETPROFILE.Equals(t.Stereotype));
            if (con != null)
            {
                EA.Element packageElement = Repository.GetElementByID(con.SupplierID);
                if (R2Const.ST_FM_PROFILE.Equals(packageElement.Stereotype))
                {
                    // con.SupplierID is the ElementID of the PackageElement
                    // Find the Package with the PackageElement by selecting the child Package in the parent Package where
                    // the ElementID is con.SupplierID
                    return Repository.GetPackageByID(packageElement.PackageID).Packages.Cast<EA.Package>().Single(p => p.Element.ElementID == con.SupplierID);
                }
            }
            EAHelper.LogMessage("Expected <create> relationship to a <HL7-FM-Profile> Package == Compiled Profile Model");
            return null;
        }
    }
}
