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

            /**
             * Load a HL7 Model from a MAX file.
             * The MAX file should contain a HL7-FM or a HL7-Profile
             */
            public static R2Model LoadModel(string maxFileName)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
                StreamReader stream = new StreamReader(maxFileName);
                var sourceModel = (ModelType)serializer.Deserialize(stream);

                ObjectType modelObjectType =
                    sourceModel.objects.Single(
                        o => R2Const.ST_FM.Equals(o.stereotype) || R2Const.ST_FM_PROFILE.Equals(o.stereotype));

                R2Model model = new R2Model(modelObjectType);
                foreach (ObjectType objectType in sourceModel.objects)
                {
                    R2ModelElement modelElement = Create(objectType);
                    // modelElement can be null when it is a not supported object
                    if (modelElement != null)
                    {
                        model.elements.Add(modelElement);
                    }
                }
                return model;
            }

            /**
             * Load a Profile Definition model from a MAX file.
             * The MAX file should contain a HL7-ProfileDefinition.
             */
            public static R2ProfileDefinition LoadProfileDefinition(R2Model baseModel, string maxFileName)
            {
                R2ProfileDefinition profileDefinition = new R2ProfileDefinition();
                /*                XmlSerializer serializer = new XmlSerializer(typeof(ModelType));
                                StreamReader stream = new StreamReader(maxFileName);
                                var sourceModel = (ModelType)serializer.Deserialize(stream);

                                foreach (ObjectType objectType in sourceModel.objects)
                                {
                                    R2ModelElement modelElement = Create(objectType);
                                    profileDefinition.elements.Add(modelElement);
                                }*/
                return profileDefinition;
            }
        }

        /**
         * An R2Model is a HL7-FM or a HL7-Profile
         */
        public class R2Model : Base.R2Model
        {
            public readonly List<R2ModelElement> elements = new List<R2ModelElement>();

            public R2Model(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                Stereotype = objectType.stereotype;
                Name = objectType.name;
            }
        }

        public class R2ProfileDefinition : Base.R2Model
        {
            public readonly List<R2ModelElement> elements = new List<R2ModelElement>();
        }

        class R2Section : Base.R2Section
        {
            public R2Section(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                Stereotype = objectType.stereotype;
                LastModified = Util.FormatLastModified(objectType.modified);
                Priority = objectType.TagValue(R2Const.TV_PRIORITY);
                ChangeNote = objectType.TagValue(R2Const.TV_CHANGENOTE);
                SectionId = objectType.alias;
                Name = objectType.name;
                string notes = objectType.notes.Text[0];
                Dictionary<string, string> noteParts = Util.SplitNotes(notes);
                Overview = noteParts.ContainsKey("OV") ? noteParts["OV"] : "";
                Example = noteParts.ContainsKey("EX") ? noteParts["EX"] : "";
                Actors = noteParts.ContainsKey("AC") ? noteParts["AC"] : "";
                Path = Name;
            }
        }

        class R2Function : Base.R2Function
        {
            public R2Function(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                Stereotype = objectType.stereotype;
                LastModified = Util.FormatLastModified(objectType.modified);
                Priority = objectType.TagValue(R2Const.TV_PRIORITY);
                ChangeNote = objectType.TagValue(R2Const.TV_CHANGENOTE);
                FunctionId = objectType.alias;
                Name = objectType.name;
                string notes = objectType.notes.Text[0];
                Dictionary<string, string> noteParts = Util.SplitNotes(notes);
                Statement = noteParts.ContainsKey("ST") ? noteParts["ST"] : "";
                Description = noteParts.ContainsKey("DE") ? noteParts["DE"] : "";
                Path = Name;
            }
        }

        class R2Criterion : Base.R2Criterion
        {
            public R2Criterion(ObjectType sourceObject)
            {
                SourceObject = sourceObject;
                LoadFromSource();
            }

            public override void LoadFromSource()
            {
                ObjectType objectType = (ObjectType)SourceObject;
                Stereotype = objectType.stereotype;
                LastModified = Util.FormatLastModified(objectType.modified);
                Priority = objectType.TagValue(R2Const.TV_PRIORITY);
                ChangeNote = objectType.TagValue(R2Const.TV_CHANGENOTE);
                Name = objectType.name;
                Text = objectType.notes.Text[0];
                // Row
                string value = objectType.TagValue(R2Const.TV_ROW, "").Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    Row = decimal.Parse(value);
                }
                else
                {
                    _values.Remove(PropertyName.Row);
                }

                string conditionalValue = objectType.TagValue(R2Const.TV_CONDITIONAL);
                if (conditionalValue != null)
                {
                    Conditional = "Y".Equals(conditionalValue);
                }
                else
                {
                    _values.Remove(PropertyName.Conditional);
                }
                string dependentValue = objectType.TagValue(R2Const.TV_DEPENDENT);
                if (dependentValue != null)
                {
                    Dependent = "Y".Equals(dependentValue);
                }
                else
                {
                    _values.Remove(PropertyName.Dependent);
                }
                Optionality = objectType.TagValue(R2Const.TV_OPTIONALITY, "");
                Path = Name;
            }
        }
    }
}
