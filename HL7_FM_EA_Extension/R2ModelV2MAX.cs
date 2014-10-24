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
                        // TODO: baseElement comes from BaseModel, mock to Criterion for now
                        Console.Write("!! R2ModelV2MAX.Create ST_COMPILERINSTRUCTION mocked to Criterion");
                        modelElement = new R2Criterion(objectType);
                        //modelElement.IsCompilerInstruction = true;
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
            public static Base.R2Model LoadModel(string maxFileName)
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
            public static Base.R2ProfileDefinition LoadProfileDefinition(Base.R2Model baseModel, string maxFileName)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
                StreamReader stream = new StreamReader(maxFileName);
                var sourceModel = (ModelType)serializer.Deserialize(stream);
                ObjectType profDefObj = sourceModel.objects.Single(o => R2Const.ST_FM_PROFILEDEFINITION.Equals(o.stereotype));
                R2ProfileDefinition profileDefinition = new R2ProfileDefinition(profDefObj);
                foreach (ObjectType objectType in sourceModel.objects)
                {
                    R2ModelElement modelElement = Create(objectType);
                    if (modelElement != null && !(modelElement is R2RootElement))
                    {
                        profileDefinition.children.Add(modelElement);
                    }
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
                objectType.modified = DateTime.Parse(get(R2Const.AT_LASTMODIFIED));
                objectType.modifiedSpecified = true;
                if (isSet(R2Const.TV_TYPE)) objectType.SetTagValue(R2Const.TV_TYPE, get(R2Const.TV_TYPE));
                else objectType.DeleteTagValue(R2Const.TV_TYPE);
                if (isSet(R2Const.TV_VERSION)) objectType.SetTagValue(R2Const.TV_VERSION, get(R2Const.TV_VERSION));
                else objectType.DeleteTagValue(R2Const.TV_VERSION);
                if (isSet(R2Const.TV_LANGUAGETAG)) objectType.SetTagValue(R2Const.TV_LANGUAGETAG, get(R2Const.TV_LANGUAGETAG));
                else objectType.DeleteTagValue(R2Const.TV_LANGUAGETAG);
                if (isSet(R2Const.TV_RATIONALE)) objectType.SetTagValue(R2Const.TV_RATIONALE, "<memo>", get(R2Const.TV_RATIONALE));
                else objectType.DeleteTagValue(R2Const.TV_RATIONALE);
                if (isSet(R2Const.TV_SCOPE)) objectType.SetTagValue(R2Const.TV_SCOPE, "<memo>", get(R2Const.TV_SCOPE));
                else objectType.DeleteTagValue(R2Const.TV_SCOPE);
                if (isSet(R2Const.TV_PRIODEF)) objectType.SetTagValue(R2Const.TV_PRIODEF, "<memo>", get(R2Const.TV_PRIODEF));
                else objectType.DeleteTagValue(R2Const.TV_PRIODEF);
                if (isSet(R2Const.TV_CONFCLAUSE)) objectType.SetTagValue(R2Const.TV_CONFCLAUSE, "<memo>", get(R2Const.TV_CONFCLAUSE));
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
                SetRefId (objectType.GetTagValue("Reference.Alias"), objectType.GetTagValue("Reference.SectionID"));
            }

            public override void SaveToSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                objectType.id = Id;
                objectType.stereotype = get(R2Const.AT_STEREOTYPE);
                objectType.modified = DateTime.Parse(get(R2Const.AT_LASTMODIFIED));
                objectType.modifiedSpecified = true;
                if (!isDefault(R2Const.TV_PRIORITY)) objectType.SetTagValue(R2Const.TV_PRIORITY, get(R2Const.TV_PRIORITY));
                else objectType.DeleteTagValue(R2Const.TV_PRIORITY);
                if (!isDefault(R2Const.TV_CHANGENOTE)) objectType.SetTagValue(R2Const.TV_CHANGENOTE, get(R2Const.TV_CHANGENOTE));
                else objectType.DeleteTagValue(R2Const.TV_CHANGENOTE);
                if (!isDefault(R2Const.AT_SECTIONID)) objectType.alias = get(R2Const.AT_SECTIONID);
                else objectType.alias = null;
                objectType.name = get(R2Const.AT_NAME);
                if (!isDefault(R2Const.AT_OVERVIEW) || !isDefault(R2Const.AT_EXAMPLE) || !isDefault(R2Const.AT_ACTORS)) objectType.SetNotes(string.Format("$OV${0}$EX${1}$AC${2}", get(R2Const.AT_STATEMENT), get(R2Const.AT_DESCRIPTION), get(R2Const.AT_ACTORS)));
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
                SetRefId (objectType.GetTagValue("Reference.Alias"), objectType.GetTagValue("Reference.FunctionID"));
            }

            public override void SaveToSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                objectType.stereotype = get(R2Const.AT_STEREOTYPE);
                objectType.modified = DateTime.Parse(get(R2Const.AT_LASTMODIFIED));
                objectType.modifiedSpecified = true;
                if (!isDefault(R2Const.TV_PRIORITY)) objectType.SetTagValue(R2Const.TV_PRIORITY, get(R2Const.TV_PRIORITY));
                else objectType.DeleteTagValue(R2Const.TV_PRIORITY);
                if (!isDefault(R2Const.TV_CHANGENOTE)) objectType.SetTagValue(R2Const.TV_CHANGENOTE, get(R2Const.TV_CHANGENOTE));
                else objectType.DeleteTagValue(R2Const.TV_CHANGENOTE);
                if (!isDefault(R2Const.AT_FUNCTIONID)) objectType.alias = get(R2Const.AT_FUNCTIONID);
                else objectType.alias = null;
                objectType.name = get(R2Const.AT_NAME);
                if (!isDefault(R2Const.AT_STATEMENT) || !isDefault(R2Const.AT_DESCRIPTION)) objectType.SetNotes(string.Format("$ST${0}$DE${1}", get(R2Const.AT_STATEMENT), get(R2Const.AT_DESCRIPTION)));
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
                SetRow(objectType.GetTagValue(R2Const.TV_ROW, ""));
                SetConditional(objectType.GetTagValue(R2Const.TV_CONDITIONAL));
                SetDependent(objectType.GetTagValue(R2Const.TV_DEPENDENT));
                Optionality = objectType.GetTagValue(R2Const.TV_OPTIONALITY, "");
                Path = Name;
                SetRefId(objectType.GetTagValue("Reference.Alias"), objectType.GetTagValue("Reference.FunctionID"), objectType.GetTagValue("Reference.CriterionID"));
            }

            public override void SaveToSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                objectType.id = Id;
                objectType.stereotype = get(R2Const.AT_STEREOTYPE);
                objectType.modified = DateTime.Parse(LastModified);
                objectType.modifiedSpecified = true;
                if (!isDefault(R2Const.TV_PRIORITY)) objectType.SetTagValue(R2Const.TV_PRIORITY, get(R2Const.TV_PRIORITY));
                else objectType.DeleteTagValue(R2Const.TV_PRIORITY);
                if (!isDefault(R2Const.TV_CHANGENOTE)) objectType.SetTagValue(R2Const.TV_CHANGENOTE, get(R2Const.TV_CHANGENOTE));
                else objectType.DeleteTagValue(R2Const.TV_CHANGENOTE);
                objectType.name = Name;
                if (!isDefault(R2Const.AT_TEXT)) objectType.SetNotes(get(R2Const.AT_TEXT));
                else objectType.notes = null;
                if (!isDefault(R2Const.TV_ROW)) objectType.SetTagValue(R2Const.TV_ROW, get(R2Const.TV_ROW));
                else objectType.DeleteTagValue(R2Const.TV_ROW);
                if (!isDefault(R2Const.TV_CONDITIONAL)) objectType.SetTagValue(R2Const.TV_CONDITIONAL, get(R2Const.TV_CONDITIONAL));
                else objectType.DeleteTagValue(R2Const.TV_CONDITIONAL);
                if (!isDefault(R2Const.TV_DEPENDENT)) objectType.SetTagValue(R2Const.TV_DEPENDENT, get(R2Const.TV_DEPENDENT));
                else objectType.DeleteTagValue(R2Const.TV_DEPENDENT);
                if (!isDefault(R2Const.TV_OPTIONALITY)) objectType.SetTagValue(R2Const.TV_OPTIONALITY, get(R2Const.TV_OPTIONALITY));
                else objectType.DeleteTagValue(R2Const.TV_OPTIONALITY);
            }
        }
    }
}
