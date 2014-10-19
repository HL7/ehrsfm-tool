using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using HL7_FM_EA_Extension.R2ModelV2.Base;
using MAX_EA.MAXSchema;

namespace HL7_FM_EA_Extension
{
    /**
     * This is the implementation of the Base Model with MAX as SourceObjects
     * TODO: Implement SaveToSource
     */
    namespace R2ModelV2.MAX
    {
        public class Factory
        {
            /**
             * Factory method to create correct model class based on the EA.Element
             */
            public static R2ModelElement Create(ObjectType objectType)
            {
                R2ModelElement modelElement = null;
                switch (objectType.stereotype)
                {
                    case R2Const.ST_FM:
                        modelElement = new R2Model(objectType);
                        break;
                    case R2Const.ST_FM_PROFILEDEFINITION:
                    case R2Const.ST_FM_PROFILE:
                        modelElement = new R2ProfileDefinition(objectType);
                        break;
                    case R2Const.ST_SECTION:
                        modelElement = new R2Section(objectType);
                        break;
                    case R2Const.ST_HEADER:
                    case R2Const.ST_FUNCTION:
                        modelElement = new R2Function(objectType);
                        break;
                    case R2Const.ST_CRITERION:
                        modelElement = new R2Criterion(objectType);
                        break;
                    case R2Const.ST_COMPILERINSTRUCTION:
                        // TODO: baseElement comes from BaseModel, mock for now
                        ObjectType baseElement = new ObjectType();
                        switch (baseElement.stereotype)
                        {
                            case R2Const.ST_SECTION:
                                modelElement = new R2Section(objectType);
                                modelElement.Defaults = new R2Section(baseElement);
                                break;
                            case R2Const.ST_HEADER:
                            case R2Const.ST_FUNCTION:
                                modelElement = new R2Function(objectType);
                                modelElement.Defaults = new R2Function(baseElement);
                                break;
                            case R2Const.ST_CRITERION:
                                modelElement = new R2Criterion(objectType);
                                modelElement.Defaults = new R2Criterion(baseElement);
                                break;
                        }
                        modelElement.IsCompilerInstruction = true;
                        break;
                }
                return modelElement;
            }

            public static R2ModelElement CreateCompilerInstruction(R2ModelElement profileElement, R2ModelElement baseElement)
            {
                // only set in compiler instruction what is different that in base model element
                R2ModelElement element = null;
                if (profileElement is Base.R2Section)
                {
                    element = new R2Section();
                }
                else if (profileElement is Base.R2Function)
                {
                    element = new R2Function();
                }
                else if (profileElement is Base.R2Criterion)
                {
                    element = new R2Criterion();
                }
                element.Defaults = baseElement;
                element.Stereotype = R2Const.ST_COMPILERINSTRUCTION;
                element.IsCompilerInstruction = true;
                element.LastModified = Util.FormatLastModified(DateTime.Now);
                element.Priority = profileElement.Priority;
                element.Path = profileElement.Path;
                if (profileElement is Base.R2Section)
                {
                    Base.R2Section profileSection = (Base.R2Section)profileElement;
                    R2Section section = (R2Section) element;
                    section.SectionId = profileSection.SectionId;
                    section.Name = profileSection.Name;
                    section.Overview = profileSection.Overview;
                    section.Example = profileSection.Example;
                    section.Actors = profileSection.Actors;
                }
                else if (profileElement is Base.R2Function)
                {
                    Base.R2Function profileFunction = (Base.R2Function)profileElement;
                    R2Function function = (R2Function)element;
                    function.FunctionId = profileFunction.FunctionId;
                    function.Name = profileFunction.Name;
                    function.Statement = profileFunction.Statement;
                    function.Description = profileFunction.Description;
                }
                else if (profileElement is Base.R2Criterion)
                {
                    Base.R2Criterion profileCriterion = (Base.R2Criterion)profileElement;
                    R2Criterion criterion = (R2Criterion)element;
                    criterion.FunctionId = profileCriterion.FunctionId;
                    criterion.CriterionSeqNo = profileCriterion.CriterionSeqNo;
                    criterion.Text = profileCriterion.Text;
                    criterion.Row = profileCriterion.Row;
                    criterion.Conditional = profileCriterion.Conditional;
                    criterion.Dependent = profileCriterion.Dependent;
                    criterion.Optionality = profileCriterion.Optionality;
                }
                return element;
            }

            /**
             * Load a HL7 Model from a MAX file.
             * The MAX file contains a HL7-FM or a HL7-Profile
             */
            public static R2Model LoadModel(string maxFileName)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
                StreamReader stream = new StreamReader(maxFileName);
                var sourceModel = (ModelType)serializer.Deserialize(stream);

                ObjectType modelObjectType =
                    sourceModel.objects.Single(o => R2Const.ST_FM.Equals(o.stereotype) || R2Const.ST_FM_PROFILE.Equals(o.stereotype));

                R2Model model = new R2Model(modelObjectType);
                foreach (ObjectType objectType in sourceModel.objects)
                {
                    R2ModelElement modelElement = Create(objectType);
                    // modelElement can be null when it is a not supported object
                    if (modelElement != null && !(modelElement is R2RootElement))
                    {
                        model.children.Add(modelElement);
                    }
                }
                return model;
            }

            /**
             * Load a Profile Definition model from a MAX file.
             * The MAX file contains a HL7-ProfileDefinition.
             */
            public static R2ProfileDefinition LoadProfileDefinition(R2Model baseModel, string maxFileName)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
                StreamReader stream = new StreamReader(maxFileName);
                var sourceModel = (ModelType)serializer.Deserialize(stream);
                ObjectType profDefObj = sourceModel.objects.Single(o => R2Const.ST_FM_PROFILEDEFINITION.Equals(o.stereotype));
                R2ProfileDefinition profileDefinition = new R2ProfileDefinition(profDefObj);
                foreach (ObjectType objectType in sourceModel.objects)
                {
                    R2ModelElement modelElement = Create(objectType);
                    profileDefinition.children.Add(modelElement);
                }
                return profileDefinition;
            }
        }

        /**
         * An R2Model is a HL7-FM or a HL7-Profile
         */
        public class R2Model : Base.R2Model
        {
            public R2Model(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                Id = objectType.id;
                Stereotype = objectType.stereotype;
                Name = objectType.name;
            }
        }

        public class R2ProfileDefinition : Base.R2ProfileDefinition
        {
            public R2ProfileDefinition()
            {
                SourceObject = new ObjectType() { id = Guid.NewGuid().ToString(), type = ObjectTypeEnum.Package, typeSpecified = true, stereotype = R2Const.ST_FM_PROFILEDEFINITION };
                LoadFromSource();
            }

            public R2ProfileDefinition(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                Id = objectType.id;
                Stereotype = objectType.stereotype;
                Name = objectType.name;
                LastModified = Util.FormatLastModified(objectType.modified);
                ChangeNote = objectType.GetTagValue(R2Const.TV_CHANGENOTE);
                Type = objectType.GetTagValue(R2Const.TV_TYPE);
                Version = objectType.GetTagValue(R2Const.TV_VERSION);
                LanguageTag = objectType.GetTagValue(R2Const.TV_LANGUAGETAG);
                Rationale = objectType.GetTagValueNotes(R2Const.TV_RATIONALE);
                Scope = objectType.GetTagValueNotes(R2Const.TV_SCOPE);
                PrioDef = objectType.GetTagValueNotes(R2Const.TV_PRIODEF);
                ConfClause = objectType.GetTagValueNotes(R2Const.TV_CONFCLAUSE);
            }

            public override void SaveToSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                objectType.id = Id;
                objectType.stereotype = Stereotype;
                objectType.name = Name;
                objectType.modified = DateTime.Parse(get(PropertyName.LastModified));
                objectType.modifiedSpecified = true;
                if (isSet(PropertyName.Type)) objectType.SetTagValue(R2Const.TV_TYPE, get(PropertyName.Type));
                else objectType.DeleteTagValue(R2Const.TV_TYPE);
                if (isSet(PropertyName.Version)) objectType.SetTagValue(R2Const.TV_VERSION, get(PropertyName.Version));
                else objectType.DeleteTagValue(R2Const.TV_VERSION);
                if (isSet(PropertyName.LanguageTag)) objectType.SetTagValue(R2Const.TV_LANGUAGETAG, get(PropertyName.LanguageTag));
                else objectType.DeleteTagValue(R2Const.TV_LANGUAGETAG);
                if (isSet(PropertyName.Rationale)) objectType.SetTagValue(R2Const.TV_RATIONALE, "<memo>", get(PropertyName.Rationale));
                else objectType.DeleteTagValue(R2Const.TV_RATIONALE);
                if (isSet(PropertyName.Scope)) objectType.SetTagValue(R2Const.TV_SCOPE, "<memo>", get(PropertyName.Scope));
                else objectType.DeleteTagValue(R2Const.TV_SCOPE);
                if (isSet(PropertyName.PrioDef)) objectType.SetTagValue(R2Const.TV_PRIODEF, "<memo>", get(PropertyName.PrioDef));
                else objectType.DeleteTagValue(R2Const.TV_PRIODEF);
                if (isSet(PropertyName.ConfClause)) objectType.SetTagValue(R2Const.TV_CONFCLAUSE, "<memo>", get(PropertyName.ConfClause));
                else objectType.DeleteTagValue(R2Const.TV_CONFCLAUSE);
            }
        }

        class R2Section : Base.R2Section
        {
            public R2Section()
            {
                SourceObject = new ObjectType() { id = Guid.NewGuid().ToString(), type = ObjectTypeEnum.Package, typeSpecified = true, stereotype = R2Const.ST_SECTION };
                LoadFromSource();
            }

            public R2Section(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                Id = objectType.id;
                Stereotype = objectType.stereotype;
                LastModified = Util.FormatLastModified(objectType.modified);
                Priority = objectType.GetTagValue(R2Const.TV_PRIORITY);
                ChangeNote = objectType.GetTagValue(R2Const.TV_CHANGENOTE);
                SectionId = objectType.alias;
                Name = objectType.name;
                string notes = objectType.GetNotes();
                Dictionary<string, string> noteParts = Util.SplitNotes(notes);
                Overview = noteParts.ContainsKey("OV") ? noteParts["OV"] : "";
                Example = noteParts.ContainsKey("EX") ? noteParts["EX"] : "";
                Actors = noteParts.ContainsKey("AC") ? noteParts["AC"] : "";
                Path = Name;
                string refAlias = objectType.GetTagValue("Reference.Alias");
                if (refAlias != null)
                {
                    RefId = string.Format("{0}.{1}", refAlias, objectType.GetTagValue("Reference.SectionID"));
                }
            }

            public override void SaveToSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                objectType.id = Id;
                objectType.stereotype = get(PropertyName.Stereotype);
                objectType.modified = DateTime.Parse(get(PropertyName.LastModified));
                objectType.modifiedSpecified = true;
                if (!isDefault(PropertyName.Priority)) objectType.SetTagValue(R2Const.TV_PRIORITY, get(PropertyName.Priority));
                else objectType.DeleteTagValue(R2Const.TV_PRIORITY);
                if (!isDefault(PropertyName.ChangeNote)) objectType.SetTagValue(R2Const.TV_CHANGENOTE, get(PropertyName.ChangeNote));
                else objectType.DeleteTagValue(R2Const.TV_CHANGENOTE);
                if (!isDefault(PropertyName.SectionId)) objectType.alias = get(PropertyName.SectionId);
                else objectType.alias = null;
                objectType.name = get(PropertyName.Name);
                if (!isDefault(PropertyName.Overview) || !isDefault(PropertyName.Example) || !isDefault(PropertyName.Actors)) objectType.SetNotes (string.Format("$OV${0}$EX${1}$AC${2}", get(PropertyName.Statement), get(PropertyName.Description), get(PropertyName.Actors)));
                else objectType.notes = null;
            }
        }

        class R2Function : Base.R2Function
        {
            public R2Function()
            {
                SourceObject = new ObjectType() { id = Guid.NewGuid().ToString(), type = ObjectTypeEnum.Feature, typeSpecified = true };
                LoadFromSource();
            }

            public R2Function(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                Id = objectType.id;
                Stereotype = objectType.stereotype;
                LastModified = Util.FormatLastModified(objectType.modified);
                Priority = objectType.GetTagValue(R2Const.TV_PRIORITY);
                ChangeNote = objectType.GetTagValue(R2Const.TV_CHANGENOTE);
                FunctionId = objectType.alias;
                Name = objectType.name;
                string notes = objectType.GetNotes();
                Dictionary<string, string> noteParts = Util.SplitNotes(notes);
                Statement = noteParts.ContainsKey("ST") ? noteParts["ST"] : "";
                Description = noteParts.ContainsKey("DE") ? noteParts["DE"] : "";
                Path = Name;
                string refAlias = objectType.GetTagValue("Reference.Alias");
                if (refAlias != null)
                {
                    RefId = string.Format("{0}.{1}", refAlias, objectType.GetTagValue("Reference.FunctionID"));
                }
            }

            public override void SaveToSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                objectType.stereotype = get(PropertyName.Stereotype);
                objectType.modified = DateTime.Parse(get(PropertyName.LastModified));
                objectType.modifiedSpecified = true;
                if (!isDefault(PropertyName.Priority)) objectType.SetTagValue(R2Const.TV_PRIORITY, get(PropertyName.Priority));
                else objectType.DeleteTagValue(R2Const.TV_PRIORITY);
                if (!isDefault(PropertyName.ChangeNote)) objectType.SetTagValue(R2Const.TV_CHANGENOTE, get(PropertyName.ChangeNote));
                else objectType.DeleteTagValue(R2Const.TV_CHANGENOTE);
                if (!isDefault(PropertyName.FunctionId)) objectType.alias = get(PropertyName.FunctionId);
                else objectType.alias = null;
                objectType.name = get(PropertyName.Name);
                if (!isDefault(PropertyName.Statement) || !isDefault(PropertyName.Description)) objectType.SetNotes(string.Format("$ST${0}$DE${1}", get(PropertyName.Statement), get(PropertyName.Description)));
                else objectType.notes = null;
            }
        }

        internal class R2Criterion : Base.R2Criterion
        {
            public R2Criterion()
            {
                SourceObject = new ObjectType() { id = Guid.NewGuid().ToString(), type = ObjectTypeEnum.Requirement, typeSpecified = true, stereotype = R2Const.ST_CRITERION };
                LoadFromSource();
            }

            public R2Criterion(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType) SourceObject;
                Id = objectType.id;
                Stereotype = objectType.stereotype;
                LastModified = Util.FormatLastModified(objectType.modified);
                Priority = objectType.GetTagValue(R2Const.TV_PRIORITY);
                ChangeNote = objectType.GetTagValue(R2Const.TV_CHANGENOTE);
                Name = objectType.name;
                Text = objectType.GetNotes();
                // Row
                string value = objectType.GetTagValue(R2Const.TV_ROW, "").Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    Row = decimal.Parse(value);
                }
                else
                {
                    _values.Remove(PropertyName.Row);
                }

                string conditionalValue = objectType.GetTagValue(R2Const.TV_CONDITIONAL);
                if (conditionalValue != null)
                {
                    Conditional = "Y".Equals(conditionalValue);
                }
                else
                {
                    _values.Remove(PropertyName.Conditional);
                }
                string dependentValue = objectType.GetTagValue(R2Const.TV_DEPENDENT);
                if (dependentValue != null)
                {
                    Dependent = "Y".Equals(dependentValue);
                }
                else
                {
                    _values.Remove(PropertyName.Dependent);
                }
                Optionality = objectType.GetTagValue(R2Const.TV_OPTIONALITY, "");
                Path = Name;
                string refAlias = objectType.GetTagValue("Reference.Alias");
                if (refAlias != null)
                {
                    RefId = string.Format("{0}.{1}#{2}", refAlias, objectType.GetTagValue("Reference.FunctionID"), objectType.GetTagValue("Reference.CriterionID"));
                }
            }

            public override void SaveToSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                objectType.id = Id;
                objectType.stereotype = get(PropertyName.Stereotype);
                objectType.modified = DateTime.Parse(LastModified);
                objectType.modifiedSpecified = true;
                if (!isDefault(PropertyName.Priority)) objectType.SetTagValue(R2Const.TV_PRIORITY, get(PropertyName.Priority));
                else objectType.DeleteTagValue(R2Const.TV_PRIORITY);
                if (!isDefault(PropertyName.ChangeNote)) objectType.SetTagValue(R2Const.TV_CHANGENOTE, get(PropertyName.ChangeNote));
                else objectType.DeleteTagValue(R2Const.TV_CHANGENOTE);
                objectType.name = Name;
                if (!isDefault(PropertyName.Text)) objectType.SetNotes(get(PropertyName.Text));
                else objectType.notes = null;
                if (!isDefault(PropertyName.Row)) objectType.SetTagValue(R2Const.TV_ROW, get(PropertyName.Row));
                else objectType.DeleteTagValue(R2Const.TV_ROW);
                if (!isDefault(PropertyName.Conditional)) objectType.SetTagValue(R2Const.TV_CONDITIONAL, get(PropertyName.Conditional));
                else objectType.DeleteTagValue(R2Const.TV_CONDITIONAL);
                if (!isDefault(PropertyName.Dependent)) objectType.SetTagValue(R2Const.TV_DEPENDENT, get(PropertyName.Dependent));
                else objectType.DeleteTagValue(R2Const.TV_DEPENDENT);
                if (!isDefault(PropertyName.Optionality)) objectType.SetTagValue(R2Const.TV_OPTIONALITY, get(PropertyName.Optionality));
                else objectType.DeleteTagValue(R2Const.TV_OPTIONALITY);
            }
        }
    }
}
