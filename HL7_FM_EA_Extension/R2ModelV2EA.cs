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
                    case R2Const.ST_FM_PROFILEDEFINITION:
                        modelElement = new R2ProfileDefinition(element);
                        break;
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
                        int genCount = element.Connectors.Cast<EA.Connector>().Count(c => "Generalization".Equals(c.Type));
                        if (genCount == 0)
                        {
                            // Try Dependency/Generalization in case this is a Section Compiler instruction 
                            genCount = element.Connectors.Cast<EA.Connector>().Count(c => "Dependency".Equals(c.Type) && "Generalization".Equals(c.Name));
                            if (genCount == 0)
                            {
                                MessageBox.Show(string.Format("{0} is a Compiler Instruction.\nExpected one(1) Generalization to a Base Element.\nFix this manually.", element.Name), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return null;
                            }
                        }
                        else if (genCount > 1)
                        {
                            MessageBox.Show(string.Format("{0} is a Compiler Instruction.\nExpected one(1) Generalization, but got {1}.\nFix this manually.", element.Name, genCount), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }
                        EA.Connector generalization = element.Connectors.Cast<EA.Connector>().SingleOrDefault(c => "Generalization".Equals(c.Type));
                        // Try Dependency/Generalization in case this is a Section Compiler instruction 
                        if (generalization == null)
                        {
                            generalization = element.Connectors.Cast<EA.Connector>().SingleOrDefault(c => "Dependency".Equals(c.Type) && "Generalization".Equals(c.Name));
                        }
                        EA.Element baseElement = repository.GetElementByID(generalization.SupplierID);
                        switch (baseElement.Stereotype)
                        {
                            case R2Const.ST_SECTION:
                                modelElement = new R2Section(element);
                                modelElement.BaseElement = new R2Section(baseElement);
                                break;
                            case R2Const.ST_HEADER:
                            case R2Const.ST_FUNCTION:
                                modelElement = new R2Function(element);
                                modelElement.BaseElement = new R2Function(baseElement);
                                break;
                            case R2Const.ST_CRITERION:
                                modelElement = new R2Criterion(element);
                                modelElement.BaseElement = new R2Criterion(baseElement);
                                break;
                        }
                        modelElement.IsCompilerInstruction = true;
                        if (repository != null)
                        {
                            modelElement.BaseElement.Path = GetModelElementPath(repository, baseElement);
                        }
                        break;
                }
                if (modelElement != null && repository != null)
                {
                    modelElement.Path = GetModelElementPath(repository, element);

                    // is element is in profile definition package this is a compiler instruction
                    EA.Package ProfileDefinitionPackage = repository.GetPackageByID(((EA.Element)modelElement.SourceObject).PackageID);
                    if (R2Const.ST_FM_PROFILEDEFINITION.Equals(ProfileDefinitionPackage.StereotypeEx))
                    {
                        modelElement.IsCompilerInstruction = true;

                        // The ProfileType is needed for R2FunctionCI's in the FunctionForm
                        if (modelElement is R2Function)
                        {
                            R2Function function = (R2Function)modelElement;
                            function.ProfileDefinition = (R2ProfileDefinition)Create(EAHelper.repository, ProfileDefinitionPackage.Element);
                        }
                        else if (modelElement is R2Criterion)
                        {
                            R2Criterion criterion = (R2Criterion)modelElement;
                            criterion.ProfileDefinition = (R2ProfileDefinition)Create(EAHelper.repository, ProfileDefinitionPackage.Element);
                        }
                    }
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

        public class R2ProfileDefinition : Base.R2ProfileDefinition
        {
            public R2ProfileDefinition(EA.Element sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                EA.Element element = (EA.Element)SourceObject;
                IsReadOnly = element.Locked;
                Stereotype = element.Stereotype;
                LastModified = Util.FormatLastModified(element.Modified);
                ChangeNote = EAHelper.GetTaggedValueNotes(element, R2Const.TV_CHANGENOTE);
                Name = element.Name;
                Type = EAHelper.GetTaggedValue(element, R2Const.TV_TYPE);
                LanguageTag = EAHelper.GetTaggedValue(element, R2Const.TV_LANGUAGETAG);
                Rationale = EAHelper.GetTaggedValueNotes(element, R2Const.TV_RATIONALE);
                Scope = EAHelper.GetTaggedValueNotes(element, R2Const.TV_SCOPE);
                PrioDef = EAHelper.GetTaggedValueNotes(element, R2Const.TV_PRIODEF);
                ConfClause = EAHelper.GetTaggedValueNotes(element, R2Const.TV_CONFCLAUSE);
            }

            public override void SaveToSource()
            {
                EA.Element element = (EA.Element) SourceObject;
                if (!isSet(R2Const.AT_NAME)) element.Name = get(R2Const.AT_NAME);
                element.Version = Version;
                element.Update();

                EAHelper.SetTaggedValue(element, R2Const.TV_TYPE, get(R2Const.TV_TYPE));
                EAHelper.SetTaggedValue(element, R2Const.TV_LANGUAGETAG, get(R2Const.TV_LANGUAGETAG));
                EAHelper.SetTaggedValue(element, R2Const.TV_RATIONALE, "<memo>", get(R2Const.TV_RATIONALE));
                EAHelper.SetTaggedValue(element, R2Const.TV_SCOPE, "<memo>", get(R2Const.TV_SCOPE));
                EAHelper.SetTaggedValue(element, R2Const.TV_PRIODEF, "<memo>", get(R2Const.TV_PRIODEF));
                EAHelper.SetTaggedValue(element, R2Const.TV_CONFCLAUSE, "<memo>", get(R2Const.TV_CONFCLAUSE));
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
                IsReadOnly = element.Locked;
                Stereotype = element.Stereotype;
                LastModified = Util.FormatLastModified(element.Modified);
                Priority = EAHelper.GetTaggedValue(element, R2Const.TV_PRIORITY, R2Const.EmptyPriority);
                ChangeNote = EAHelper.GetTaggedValueNotes(element, R2Const.TV_CHANGENOTE);
                SectionId = element.Alias;
                Name = element.Name;
                Dictionary<string, string> noteParts = Util.SplitNotes(element.Notes);
                Overview = noteParts.ContainsKey("OV") ? noteParts["OV"] : "";
                Example = noteParts.ContainsKey("EX") ? noteParts["EX"] : "";
                Actors = noteParts.ContainsKey("AC") ? noteParts["AC"] : "";
                SetRefId(EAHelper.GetTaggedValue(element, "Reference.Alias"), EAHelper.GetTaggedValue(element, "Reference.SectionID"));
            }

            public override void SaveToSource()
            {
                EA.Element element = (EA.Element)SourceObject;
                if (!isDefault(R2Const.AT_SECTIONID)) element.Alias = get(R2Const.AT_SECTIONID);
                if (!isDefault(R2Const.AT_NAME)) element.Name = get(R2Const.AT_NAME);
                if (!isDefault(R2Const.AT_OVERVIEW) || !isDefault(R2Const.AT_EXAMPLE) || !isDefault(R2Const.AT_ACTORS))
                    element.Notes = string.Format("$OV${0}$EX${1}$AC${2}", get(R2Const.AT_OVERVIEW), get(R2Const.AT_EXAMPLE), get(R2Const.AT_ACTORS));
                element.Update();
                string priority = get(R2Const.TV_PRIORITY);
                if (!string.IsNullOrEmpty(priority)) EAHelper.SetTaggedValue(element, R2Const.TV_PRIORITY, priority);
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_PRIORITY);
                if (isSet(R2Const.TV_CHANGENOTE)) EAHelper.SetTaggedValue(element, R2Const.TV_CHANGENOTE, "<memo>", get(R2Const.TV_CHANGENOTE));
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_CHANGENOTE);
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
                IsReadOnly = element.Locked;
                Stereotype = element.Stereotype;
                LastModified = Util.FormatLastModified(element.Modified);
                Priority = EAHelper.GetTaggedValue(element, R2Const.TV_PRIORITY, R2Const.EmptyPriority);
                ChangeNote = EAHelper.GetTaggedValueNotes(element, R2Const.TV_CHANGENOTE);
                FunctionId = element.Alias;
                Name = element.Name;
                Dictionary<string, string> noteParts = Util.SplitNotes(element.Notes);
                Statement = noteParts.ContainsKey("ST") ? noteParts["ST"] : "";
                Description = noteParts.ContainsKey("DE") ? noteParts["DE"] : "";
                SetRefId (EAHelper.GetTaggedValue(element, "Reference.Alias"), EAHelper.GetTaggedValue(element, "Reference.FunctionID"));
            }

            public override void SaveToSource()
            {
                EA.Element element = (EA.Element)SourceObject;
                if (!isDefault(R2Const.AT_FUNCTIONID)) element.Alias = get(R2Const.AT_FUNCTIONID);
                if (!isDefault(R2Const.AT_NAME)) element.Name = get(R2Const.AT_NAME);
                if (!isDefault(R2Const.AT_STATEMENT) || !isDefault(R2Const.AT_DESCRIPTION))
                    element.Notes = string.Format("$ST${0}$DE${1}", get(R2Const.AT_STATEMENT), get(R2Const.AT_DESCRIPTION));
                element.Update();
                string priority = get(R2Const.TV_PRIORITY);
                if (!string.IsNullOrEmpty(priority)) EAHelper.SetTaggedValue(element, R2Const.TV_PRIORITY, priority);
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_PRIORITY);
                if (isSet(R2Const.TV_CHANGENOTE)) EAHelper.SetTaggedValue(element, R2Const.TV_CHANGENOTE, "<memo>", get(R2Const.TV_CHANGENOTE));
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_CHANGENOTE);
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
                IsReadOnly = element.Locked;
                Stereotype = element.Stereotype;
                LastModified = Util.FormatLastModified(element.Modified);
                Priority = EAHelper.GetTaggedValue(element, R2Const.TV_PRIORITY, R2Const.EmptyPriority);
                ChangeNote = EAHelper.GetTaggedValueNotes(element, R2Const.TV_CHANGENOTE);
                Name = element.Name;
                Text = element.Notes;
                SetRow(EAHelper.GetTaggedValue(element, R2Const.TV_ROW));
                SetConditional(EAHelper.GetTaggedValue(element, R2Const.TV_CONDITIONAL));
                SetDependent(EAHelper.GetTaggedValue(element, R2Const.TV_DEPENDENT));
                Optionality = EAHelper.GetTaggedValue(element, R2Const.TV_OPTIONALITY, "");
                SetRefId(EAHelper.GetTaggedValue(element, "Reference.Alias"), EAHelper.GetTaggedValue(element, "Reference.FunctionID"), EAHelper.GetTaggedValue(element, "Reference.CriterionID"));
            }

            public override void SaveToSource()
            {
                EA.Element element = (EA.Element) SourceObject;
                if (!isDefault(R2Const.AT_NAME)) element.Name = Name;
                if (!isDefault(R2Const.AT_TEXT)) element.Notes = get(R2Const.AT_TEXT);
                else element.Notes = "";
                element.Update();
                if (!isDefault(R2Const.TV_ROW)) EAHelper.SetTaggedValue(element, R2Const.TV_ROW, Row.ToString());
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_ROW);
                if (!isDefault(R2Const.TV_CONDITIONAL)) EAHelper.SetTaggedValue(element, R2Const.TV_CONDITIONAL, Conditional ? "Y" : "N");
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_CONDITIONAL);
                if (!isDefault(R2Const.TV_DEPENDENT)) EAHelper.SetTaggedValue(element, R2Const.TV_DEPENDENT, Dependent ? "Y" : "N");
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_DEPENDENT);
                if (!isDefault(R2Const.TV_OPTIONALITY)) EAHelper.SetTaggedValue(element, R2Const.TV_OPTIONALITY, Optionality);
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_OPTIONALITY);
                string priority = get(R2Const.TV_PRIORITY);
                if (!string.IsNullOrEmpty(priority)) EAHelper.SetTaggedValue(element, R2Const.TV_PRIORITY, priority);
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_PRIORITY);
                if (isSet(R2Const.TV_CHANGENOTE)) EAHelper.SetTaggedValue(element, R2Const.TV_CHANGENOTE, "<memo>", get(R2Const.TV_CHANGENOTE));
                else EAHelper.DeleteTaggedValue(element, R2Const.TV_CHANGENOTE);
                // TODO: update visual style
                R2Config.config.updateStyle(element);
            }
        }
    }
}
