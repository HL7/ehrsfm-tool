using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HL7_FM_EA_Extension.R2ModelV2.Base;

namespace HL7_FM_EA_Extension
{
    /**
     * This is the implementation of the Base Model with EA as SourceObjects
     */
    namespace R2ModelV2.EA_API
    {
        public class Factory
        {
            /**
                * Factory method to create correct model class based on the EA.Element
                */
            public static R2ModelElement Create(EA.Repository repository, EA.Element element)
            {
                R2ModelElement modelElement = null;
                switch (element.Stereotype)
                {
                    case R2Const.ST_SECTION:
                        modelElement = new R2Section(element);
                        break;
                    case R2Const.ST_HEADER:
                    case R2Const.ST_FUNCTION:
                        modelElement = new R2Function(element);
                        break;
                    case R2Const.ST_CRITERION:
                        modelElement = new R2Criterion(element);
                        break;
                    case R2Const.ST_COMPILERINSTRUCTION:
                        EA.Connector generalization =
                            element.Connectors.Cast<EA.Connector>().SingleOrDefault(
                                t => "Generalization".Equals(t.Type));
                        if (generalization == null)
                        {
                            MessageBox.Show("Generalization to Base Element missing");
                            return null;
                        }
                        EA.Element baseElement = repository.GetElementByID(generalization.SupplierID);
                        switch (baseElement.Stereotype)
                        {
                            case R2Const.ST_SECTION:
                                modelElement = new R2Section(element);
                                modelElement.Defaults = new R2Section(baseElement);
                                break;
                            case R2Const.ST_HEADER:
                            case R2Const.ST_FUNCTION:
                                modelElement = new R2Function(element);
                                modelElement.Defaults = new R2Function(baseElement);
                                break;
                            case R2Const.ST_CRITERION:
                                modelElement = new R2Criterion(element);
                                modelElement.Defaults = new R2Criterion(baseElement);
                                break;
                        }
                        modelElement.IsCompilerInstruction = true;
                        if (repository != null)
                        {
                            modelElement.Defaults.Path = GetModelElementPath(repository, baseElement);
                        }

                        // The ProfileType is needed for R2FunctionCI's in the FunctionForm
                        if (modelElement is R2Function)
                        {
                            R2Function function = (R2Function) modelElement;
                            EA.Package ProfileDefinitionPackage =
                                repository.GetPackageByID(((EA.Element) function.SourceObject).PackageID);
                            if (R2Const.ST_FM_PROFILEDEFINITION.Equals(ProfileDefinitionPackage.StereotypeEx))
                            {
                                function.ProfileType = EAHelper.getTaggedValue(ProfileDefinitionPackage.Element, "Type");
                            }
                        }
                        break;
                }
                if (modelElement != null && repository != null)
                {
                    modelElement.Path = GetModelElementPath(repository, element);
                }
                return modelElement;
            }

            /**
             * Create a string containing the element path joined with '/' up to the <HL7-FM> stereotyped package.
             * This is used for as title for Section/Header/Function/Criteria Forms.
             */
            static string GetModelElementPath(EA.Repository repository, EA.Element element)
            {
                List<string> path = new List<string>();
                EA.Element el = element;
                while (el != null && !R2Const.ST_FM.Equals(el.Stereotype))
                {
                    path.Insert(0, el.Name);
                    if (el.ParentID == 0)
                    {
                        // Don't include package name, that is obvious from the Header/Function Code.
                        // el = Repository.GetPackageByID(el.PackageID).Element;
                        break;
                    }
                    else
                    {
                        el = repository.GetElementByID(el.ParentID);
                    }
                }
                return string.Join(" / ", path.ToArray());
            }
        }

        public class R2Section : Base.R2Section
        {
            public R2Section(EA.Element sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                EA.Element element = (EA.Element) SourceObject;
                Stereotype = element.Stereotype;
                LastModified = Util.FormatLastModified(element.Modified);
                Priority = EAHelper.getTaggedValue(element, R2Const.TV_PRIORITY, R2Const.EmptyPriority);
                ChangeNote = EAHelper.getTaggedValueNotes(element, R2Const.TV_CHANGENOTE);
                SectionId = element.Alias;
                Name = element.Name;
                string notes = element.Notes;
                Dictionary<string, string> noteParts = Util.SplitNotes(notes);
                Overview = noteParts.ContainsKey("OV") ? noteParts["OV"] : "";
                Example = noteParts.ContainsKey("EX") ? noteParts["EX"] : "";
                Actors = noteParts.ContainsKey("AC") ? noteParts["AC"] : "";
            }

            public override void SaveToSource()
            {
                EA.Element element = (EA.Element)SourceObject;
                if (!isDefault(PropertyName.SectionId)) element.Alias = get(PropertyName.SectionId);
                if (!isDefault(PropertyName.Name)) element.Name = get(PropertyName.Name);
                if (!isDefault(PropertyName.Overview) || !isDefault(PropertyName.Example) || !isDefault(PropertyName.Actors))
                    element.Notes = string.Format("$OV${0}$EX${1}$AC${2}", get(PropertyName.Overview), get(PropertyName.Example), get(PropertyName.Actors));
                element.Update();
                string priority = get(PropertyName.Priority);
                if (!string.IsNullOrEmpty(priority)) EAHelper.updateTaggedValue(element, R2Const.TV_PRIORITY, priority);
                else EAHelper.deleteTaggedValue(element, R2Const.TV_PRIORITY);
                if (isSet(PropertyName.ChangeNote)) EAHelper.updateTaggedValue(element, R2Const.TV_CHANGENOTE, "<memo>", get(PropertyName.ChangeNote));
                else EAHelper.deleteTaggedValue(element, R2Const.TV_CHANGENOTE);
                // TODO: update visual style
                R2Config.config.updateStyle(element);
            }
        }

        public class R2Function : Base.R2Function
        {
            public R2Function(EA.Element sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                EA.Element element = (EA.Element)SourceObject;
                Stereotype = element.Stereotype;
                LastModified = Util.FormatLastModified(element.Modified);
                Priority = EAHelper.getTaggedValue(element, R2Const.TV_PRIORITY, R2Const.EmptyPriority);
                ChangeNote = EAHelper.getTaggedValueNotes(element, R2Const.TV_CHANGENOTE);
                FunctionId = element.Alias;
                Name = element.Name;
                string notes = element.Notes;
                Dictionary<string, string> noteParts = Util.SplitNotes(notes);
                Statement = noteParts.ContainsKey("ST") ? noteParts["ST"] : "";
                Description = noteParts.ContainsKey("DE") ? noteParts["DE"] : "";
            }

            public override void SaveToSource()
            {
                EA.Element element = (EA.Element)SourceObject;
                if (!isDefault(PropertyName.FunctionId)) element.Alias = get(PropertyName.FunctionId);
                if (!isDefault(PropertyName.Name)) element.Name = get(PropertyName.Name);
                if (!isDefault(PropertyName.Statement) || !isDefault(PropertyName.Description))
                    element.Notes = string.Format("$ST${0}$DE${1}", get(PropertyName.Statement), get(PropertyName.Description));
                element.Update();
                string priority = get(PropertyName.Priority);
                if (!string.IsNullOrEmpty(priority)) EAHelper.updateTaggedValue(element, R2Const.TV_PRIORITY, priority);
                else EAHelper.deleteTaggedValue(element, R2Const.TV_PRIORITY);
                if (isSet(PropertyName.ChangeNote)) EAHelper.updateTaggedValue(element, R2Const.TV_CHANGENOTE, "<memo>", get(PropertyName.ChangeNote));
                else EAHelper.deleteTaggedValue(element, R2Const.TV_CHANGENOTE);
                // TODO: update visual style
                R2Config.config.updateStyle(element);
            }
        }

        public class R2Criterion : Base.R2Criterion
        {
            public R2Criterion(EA.Element sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                EA.Element element = (EA.Element) SourceObject;
                Stereotype = element.Stereotype;
                LastModified = Util.FormatLastModified(element.Modified);
                Priority = EAHelper.getTaggedValue(element, R2Const.TV_PRIORITY, R2Const.EmptyPriority);
                ChangeNote = EAHelper.getTaggedValueNotes(element, R2Const.TV_CHANGENOTE);
                Name = element.Name;
                Text = element.Notes;
                // Row
                string value = EAHelper.getTaggedValue(element, R2Const.TV_ROW).Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    Row = decimal.Parse(value);
                }

                string conditionalValue = EAHelper.getTaggedValue(element, R2Const.TV_CONDITIONAL);
                if (conditionalValue != null)
                {
                    Conditional = "Y".Equals(conditionalValue);
                }
                else
                {
                    _values.Remove(PropertyName.Conditional);
                }
                string dependentValue = EAHelper.getTaggedValue(element, R2Const.TV_DEPENDENT);
                if (dependentValue != null)
                {
                    Dependent = "Y".Equals(dependentValue);
                }
                else
                {
                    _values.Remove(PropertyName.Dependent);
                }
                Optionality = EAHelper.getTaggedValue(element, R2Const.TV_OPTIONALITY, "");
            }

            public override void SaveToSource()
            {
                EA.Element element = (EA.Element) SourceObject;
                if (!isDefault(PropertyName.Name)) element.Name = Name;
                if (!isDefault(PropertyName.Text)) element.Notes = get(PropertyName.Text);
                else element.Notes = "";
                element.Update();
                if (!isDefault(PropertyName.Row)) EAHelper.updateTaggedValue(element, R2Const.TV_ROW, Row.ToString());
                else EAHelper.deleteTaggedValue(element, R2Const.TV_ROW);
                if (!isDefault(PropertyName.Conditional)) EAHelper.updateTaggedValue(element, R2Const.TV_CONDITIONAL, Conditional ? "Y" : "N");
                else EAHelper.deleteTaggedValue(element, R2Const.TV_CONDITIONAL);
                if (!isDefault(PropertyName.Dependent)) EAHelper.updateTaggedValue(element, R2Const.TV_DEPENDENT, Dependent ? "Y" : "N");
                else EAHelper.deleteTaggedValue(element, R2Const.TV_DEPENDENT);
                if (!isDefault(PropertyName.Optionality)) EAHelper.updateTaggedValue(element, R2Const.TV_OPTIONALITY, Optionality);
                else EAHelper.deleteTaggedValue(element, R2Const.TV_OPTIONALITY);
                string priority = get(PropertyName.Priority);
                if (!string.IsNullOrEmpty(priority)) EAHelper.updateTaggedValue(element, R2Const.TV_PRIORITY, priority);
                else EAHelper.deleteTaggedValue(element, R2Const.TV_PRIORITY);
                if (isSet(PropertyName.ChangeNote)) EAHelper.updateTaggedValue(element, R2Const.TV_CHANGENOTE, "<memo>", get(PropertyName.ChangeNote));
                else EAHelper.deleteTaggedValue(element, R2Const.TV_CHANGENOTE);
                // TODO: update visual style
                R2Config.config.updateStyle(element);
            }
        }
    }
}
