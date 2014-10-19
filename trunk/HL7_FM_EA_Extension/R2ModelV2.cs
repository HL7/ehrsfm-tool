using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HL7_FM_EA_Extension
{
    namespace R2ModelV2.Base
    {
        public class R2Const
        {
            public const string ST_FM = "HL7-FM";
            public const string ST_BASEMODEL = "use";
            public const string ST_TARGETPROFILE = "create";
            public const string ST_FM_PROFILE = "HL7-FM-Profile";
            public const string ST_SECTION = "Section";
            public const string ST_HEADER = "Header";
            public const string ST_FUNCTION = "Function";
            public const string ST_CONSEQUENCELINK = "ConsequenceLink";
            public const string ST_SEEALSO = "SeeAlso";
            
            public const string ST_FM_PROFILEDEFINITION = "HL7-FM-ProfileDefinition";
            public const string TV_TYPE = "Type";
            public const string TV_VERSION = "Version";
            public const string TV_LANGUAGETAG = "LanguageTag";
            public const string TV_RATIONALE = "Rationale";
            public const string TV_SCOPE = "Scope";
            public const string TV_PRIODEF = "PrioritiesDefinition";
            public const string TV_CONFCLAUSE = "ConformanceClause";

            public const string ST_COMPILERINSTRUCTION = "CI";
            public const string TV_PRIORITY = "Priority";
            public const string TV_CHANGENOTE = "ChangeNote";
            public const string TV_QUALIFIER = "Qualifier";
            public const string TV_ROW = "Row";

            public const string ST_CRITERION = "Criteria";
            public const string TV_OPTIONALITY = "Optionality";
            public const string TV_DEPENDENT = "Dependent";
            public const string TV_CONDITIONAL = "Conditional";

            public enum ProfileType
            {
                Companion,
                Domain,
                Realm,
                Derived,
                Combined,
                Merged
            };

            public enum Qualifier
            {
                None,
                Deprecate,    // DEP
                Delete,       // D
                Exclude       // Special qualifier to explicitely exclude a Criterion
            };

            public enum Priority
            {
                EN,     // Essential Now
                EF,     // Essential Future
                OPT     // Optional
            };

            public Dictionary<Priority, string> PriorityDescription = new Dictionary<Priority, string>()
            {
                { Priority.EN, "Essential Now" },
                { Priority.EF, "Essential Future" },
                { Priority.OPT, "Optional" }
            };

            public const string EmptyPriority = "";
        }

        class Util
        {
            internal static string FormatLastModified(DateTime dateTime)
            {
                string formatted = dateTime.ToString();
                if ("1/1/0001 00:00:00".Equals(formatted))
                {
                    return "";
                }
                else
                {
                    return formatted;
                }
            }

            internal static Dictionary<string, string> SplitNotes(string notes)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                if (notes != null)
                {
                    System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@"\$([A-Z]{2})\$");
                    string[] parts = regex2.Split(notes, 10);
                    for (int i = 1; i < parts.Length; i += 2)
                    {
                        dict[parts[i]] = (parts.Length > i + 1) ? parts[i + 1] : "";
                    }
                }
                return dict;
            }
        }

        public abstract class R2ModelElement
        {
            // The SourceObject is a backingstore that can be serialized
            public Object SourceObject { get; set; }
            // Load the values from the SourceObject
            public virtual void LoadFromSource()
            {
            }

            // Save the values to the SourceObject
            public virtual void SaveToSource()
            {
            }

            // use another R2ModelElement for Default values
            public R2ModelElement Defaults { get; set; }
            internal enum PropertyName
            {
                Path,
                LastModified,
                Stereotype,
                SectionId, Name, Overview, Example, Actors,
                FunctionId, Statement, Description,
                CriterionSeqNo, Text, Row, Dependent, Conditional, Optionality,
                Priority, ChangeNote,
                Version, Type, LanguageTag, Rationale, Scope, PrioDef, ConfClause
            };
            internal Dictionary<PropertyName, string> _values = new Dictionary<PropertyName, string>();
            internal string get(PropertyName key)
            {
                string value = "";
                if (_values.ContainsKey(key))
                {
                    value = _values[key];
                }
                else if (Defaults != null)
                {
                    value = Defaults.get(key);
                }
                return value;
            }
            internal decimal getDecimal(PropertyName key)
            {
                string value = get(key);
                if (string.IsNullOrEmpty(value))
                {
                    value = "0";
                }
                return decimal.Parse(value);
            }
            internal bool getBool(PropertyName key)
            {
                string value = get(key);
                return "Y".Equals(value);
            }
            internal void set(PropertyName key, string value)
            {
                _values[key] = value;
                if (string.IsNullOrEmpty(value))
                {
                    _values.Remove(key);
                }
                else if (Defaults != null && value.Equals(Defaults.get(key)))
                {
                    _values.Remove(key);
                }
            }
            internal void set(PropertyName key, decimal value)
            {
                set(key, value.ToString());
            }
            internal void set(PropertyName key, bool value)
            {
                set(key, value ? "Y" : "N");
            }
            internal bool isDefault(PropertyName key)
            {
                if (Defaults != null)
                {
                    if (_values.ContainsKey(key))
                    {
                        return _values[key].Equals(Defaults.get(key));
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            internal bool isSet(PropertyName key)
            {
                return _values.ContainsKey(key);
            }

            // Get the Externalized Id, e.g. CP.1.2
            public abstract string GetExtId();
            // Get the Referenced Id
            public string RefId { get; set; }
            // Get the unique Id, should be a GUID
            public string Id { get; set; }
            public string LastModified { get { return get(PropertyName.LastModified); } set { set(PropertyName.LastModified, value); } }
            public string Path
            {
                get
                {
                    if (IsCompilerInstruction && Defaults != null)
                    {
                        return Defaults.Path;
                    }
                    else
                    {
                        return get(PropertyName.Path);
                    }
                } 
                set
                {
                    set(PropertyName.Path, value);
                }
            }
            public string Stereotype
            {
                get
                {
                    if (IsCompilerInstruction && Defaults != null)
                    {
                        return Defaults.Stereotype;
                    }
                    else
                    {
                        return get(PropertyName.Stereotype);
                    }
                }
                internal set
                {
                    set(PropertyName.Stereotype, value);
                }
            }

            public bool IsCompilerInstruction { get; set; }
            public string Priority
            {
                get
                {
                    return get(PropertyName.Priority);
                }
                set
                {
                    // if (!IsCompilerInstruction) throw Illegal;
                    set(PropertyName.Priority, value);
                }
            }
            public string ChangeNote
            {
                get
                {
                    return get(PropertyName.ChangeNote);
                }
                set
                {
                    set(PropertyName.ChangeNote, value);
                }
            }
        }

        public abstract class R2RootElement : R2ModelElement
        {
            public override string GetExtId()
            {
                throw new NotImplementedException();
            }
            public string Name { get { return get(PropertyName.Name); } set { set(PropertyName.Name, value); } }

            public List<R2ModelElement> children = new List<R2ModelElement>();
        }

        public abstract class R2Model : R2RootElement
        {
        }

        public abstract class R2ProfileDefinition : R2RootElement
        {
            public string Version { get { return get(PropertyName.Version); } set { set(PropertyName.Version, value); } }
            public string Type { get { return get(PropertyName.Type); } set { set(PropertyName.Type, value); } }
            public string LanguageTag { get { return get(PropertyName.LanguageTag); } set { set(PropertyName.LanguageTag, value); } }
            public string Rationale { get { return get(PropertyName.Rationale); } set { set(PropertyName.Rationale, value); } }
            public string Scope { get { return get(PropertyName.Scope); } set { set(PropertyName.Scope, value); } }
            public string PrioDef { get { return get(PropertyName.PrioDef); } set { set(PropertyName.PrioDef, value); } }
            public string ConfClause { get { return get(PropertyName.ConfClause); } set { set(PropertyName.ConfClause, value); } }
            public string BaseModel { get; set; }
        }

        public class R2Section : R2ModelElement
        {
            public override string GetExtId()
            {
                return SectionId;
            }
            public string SectionId { get { return get(PropertyName.SectionId); } set { set(PropertyName.SectionId, value); } }
            public string Name { get { return get(PropertyName.Name); } set { set(PropertyName.Name, value); } }
            public string Overview { get { return get(PropertyName.Overview); } set { set(PropertyName.Overview, value); } }
            public string Example { get { return get(PropertyName.Example); } set { set(PropertyName.Example, value); } }
            public string Actors { get { return get(PropertyName.Actors); } set { set(PropertyName.Actors, value); } }
        }

        public class R2Function : R2ModelElement
        {
            public override string GetExtId()
            {
                return FunctionId;
            }
            public string FunctionId { get { return get(PropertyName.FunctionId); } set { set(PropertyName.FunctionId, value); } }
            public string Name { get { return get(PropertyName.Name); } set { set(PropertyName.Name, value); } }
            public string Statement { get { return get(PropertyName.Statement); } set { set(PropertyName.Statement, value); } }
            public string Description { get { return get(PropertyName.Description); } set { set(PropertyName.Description, value); } }

            // TODO: This has to go to R2Profile/R2ProfileDefinition
            public string ProfileType { get; set; }
        }

        public class R2Criterion : R2ModelElement
        {
            public override string GetExtId()
            {
                return Name;
            }
            public string Name
            {
                get { return string.Format("{0}#{1:00}", get(PropertyName.FunctionId), getDecimal(PropertyName.CriterionSeqNo)); }
                set
                {
                    if (value != null)
                    {
                        // FuntionId
                        int sepIdx = value.IndexOf('#');
                        if (sepIdx == -1)
                        {
                            FunctionId = value;
                        }
                        else
                        {
                            FunctionId = value.Substring(0, sepIdx);
                        }

                        // CriterionId
                        // If the name doesnot contain a '#' yet, assume 0
                        if (sepIdx == -1)
                        {
                            CriterionSeqNo = 0;
                        }
                        else
                        {
                            int sepIdx2 = value.IndexOf(' ', sepIdx);
                            if (sepIdx2 == -1) sepIdx2 = value.Length;
                            CriterionSeqNo = decimal.Parse(value.Substring(sepIdx + 1, sepIdx2 - sepIdx - 1));
                        }
                    }
                    else
                    {
                        FunctionId = "";
                        CriterionSeqNo = 0;
                    }
                }
            }
            public string FunctionId { get { return get(PropertyName.FunctionId); } set { set(PropertyName.FunctionId, value); } }
            public decimal CriterionSeqNo { get { return getDecimal(PropertyName.CriterionSeqNo); } set { set(PropertyName.CriterionSeqNo, value); } }
            public string Text { get { return get(PropertyName.Text); } set { set(PropertyName.Text, value); } }
            public decimal Row { get { return getDecimal(PropertyName.Row); } set { set(PropertyName.Row, value); } }
            public bool Conditional { get { return getBool(PropertyName.Conditional); } set { set(PropertyName.Conditional, value); } }
            public bool Dependent { get { return getBool(PropertyName.Dependent); } set { set(PropertyName.Dependent, value); } }
            public string Optionality { get { return get(PropertyName.Optionality); } set { set(PropertyName.Optionality, value); } }
        }
    }
}
