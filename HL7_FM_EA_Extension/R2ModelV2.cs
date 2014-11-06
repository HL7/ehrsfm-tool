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
            public const string AT_STEREOTYPE = "@Stereotype";
            public const string AT_LASTMODIFIED = "@LastModified";
            public const string AT_NAME = "@Name";
            public const string AT_TEXT = "@Text";
            public const string AT_PATH = "@Path";
            public const string AT_STATEMENT = "@Statement";
            public const string AT_DESCRIPTION = "@Description";
            public const string AT_OVERVIEW = "@Overview";
            public const string AT_EXAMPLE = "@Example";
            public const string AT_ACTORS = "@Actors";
            public const string AT_SECTIONID = "@SectionId";
            public const string AT_FUNCTIONID = "@FunctionId";
            public const string AT_CRITSEQNO = "@CriterionSeqNo";

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

            public bool IsReadOnly { get; set; }

            // This R2ModelElement is a Compiler Instruction for a Base Element, base element has Default values
            public R2ModelElement BaseElement { get; set; }
            internal Dictionary<string, string> _values = new Dictionary<string, string>();
            internal string get(string key)
            {
                string value = "";
                if (_values.ContainsKey(key))
                {
                    value = _values[key];
                }
                else if (BaseElement != null)
                {
                    value = BaseElement.get(key);
                }
                return value;
            }
            internal decimal getDecimal(string key)
            {
                string value = get(key);
                if (string.IsNullOrEmpty(value))
                {
                    value = "0";
                }
                return decimal.Parse(value);
            }
            internal bool getBool(string key)
            {
                string value = get(key);
                return "Y".Equals(value);
            }
            internal void set(string key, string value)
            {
                _values[key] = value;
                if (string.IsNullOrEmpty(value))
                {
                    _values.Remove(key);
                }
                else if (BaseElement != null && value.Equals(BaseElement.get(key)))
                {
                    _values.Remove(key);
                }
            }
            internal void set(string key, decimal value)
            {
                set(key, value.ToString());
            }
            internal void set(string key, bool value)
            {
                set(key, value ? "Y" : "N");
            }
            internal bool isDefault(string key)
            {
                if (BaseElement != null)
                {
                    if (_values.ContainsKey(key))
                    {
                        return _values[key].Equals(BaseElement.get(key));
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
            internal bool isSet(string key)
            {
                return _values.ContainsKey(key);
            }

            // Get the Externalized Id, e.g. CP.1.2
            public abstract string GetExtId();
            // Get the Referenced Id
            public string RefId { get; internal set; }
            // Get Align Id returns RefId or if not set the ExtId
            public string GetAlignId()
            {
                if (!string.IsNullOrEmpty(RefId))
                {
                    return RefId;
                }
                else
                {
                    return GetExtId();
                }
            }
            // Get the unique Id, should be a GUID
            public string Id { get; set; }
            public string LastModified { get { return get(R2Const.AT_LASTMODIFIED); } set { set(R2Const.AT_LASTMODIFIED, value); } }
            public string Path
            {
                get
                {
                    if (IsCompilerInstruction && BaseElement != null)
                    {
                        return BaseElement.Path;
                    }
                    else
                    {
                        return get(R2Const.AT_PATH);
                    }
                } 
                set
                {
                    set(R2Const.AT_PATH, value);
                }
            }
            public string Stereotype
            {
                get
                {
                    if (IsCompilerInstruction && BaseElement != null)
                    {
                        return BaseElement.Stereotype;
                    }
                    else
                    {
                        return get(R2Const.AT_STEREOTYPE);
                    }
                }
                internal set
                {
                    set(R2Const.AT_STEREOTYPE, value);
                }
            }

            public bool IsCompilerInstruction { get; set; }
            public string Priority
            {
                get
                {
                    return get(R2Const.TV_PRIORITY);
                }
                set
                {
                    // if (!IsCompilerInstruction) throw Illegal;
                    set(R2Const.TV_PRIORITY, value);
                }
            }
            public string ChangeNote
            {
                get
                {
                    return get(R2Const.TV_CHANGENOTE);
                }
                set
                {
                    set(R2Const.TV_CHANGENOTE, value);
                }
            }
        }

        public abstract class R2RootElement : R2ModelElement
        {
            public override string GetExtId()
            {
                throw new NotImplementedException();
            }
            public string Name { get { return get(R2Const.AT_NAME); } set { set(R2Const.AT_NAME, value); } }

            public List<R2ModelElement> children = new List<R2ModelElement>();
        }

        public class R2Model : R2RootElement
        {
        }

        public class R2ProfileDefinition : R2RootElement
        {
            public R2ProfileDefinition()
            {
                Stereotype = R2Const.ST_FM_PROFILEDEFINITION;
            }

            public string Version { get { return get(R2Const.TV_VERSION); } set { set(R2Const.TV_VERSION, value); } }
            public string Type { get { return get(R2Const.TV_TYPE); } set { set(R2Const.TV_TYPE, value); } }
            public string LanguageTag { get { return get(R2Const.TV_LANGUAGETAG); } set { set(R2Const.TV_LANGUAGETAG, value); } }
            public string Rationale { get { return get(R2Const.TV_RATIONALE); } set { set(R2Const.TV_RATIONALE, value); } }
            public string Scope { get { return get(R2Const.TV_SCOPE); } set { set(R2Const.TV_SCOPE, value); } }
            public string PrioDef { get { return get(R2Const.TV_PRIODEF); } set { set(R2Const.TV_PRIODEF, value); } }
            public string ConfClause { get { return get(R2Const.TV_CONFCLAUSE); } set { set(R2Const.TV_CONFCLAUSE, value); } }
            public string BaseModelName { get; set; }
        }

        public class R2Section : R2ModelElement
        {
            public R2Section()
            {
                Stereotype = R2Const.ST_SECTION;
            }

            public override string GetExtId()
            {
                return SectionId;
            }

            public void SetRefId(string RefAlias, string RefSectionId)
            {
                RefId = RefSectionId;
            }

            public string SectionId { get { return get(R2Const.AT_SECTIONID); } set { set(R2Const.AT_SECTIONID, value); } }
            public string Name { get { return get(R2Const.AT_NAME); } set { set(R2Const.AT_NAME, value); } }
            public string Overview { get { return get(R2Const.AT_OVERVIEW); } set { set(R2Const.AT_OVERVIEW, value); } }
            public string Example { get { return get(R2Const.AT_EXAMPLE); } set { set(R2Const.AT_EXAMPLE, value); } }
            public string Actors { get { return get(R2Const.AT_ACTORS); } set { set(R2Const.AT_ACTORS, value); } }
        }

        public class R2Function : R2ModelElement
        {
            public R2Function()
            {
                Stereotype = R2Const.ST_FUNCTION;
            }

            public override string GetExtId()
            {
                return FunctionId;
            }

            public void SetRefId(string RefAlias, string RefFunctionId)
            {
                RefId = RefFunctionId;
            }

            public string FunctionId { get { return get(R2Const.AT_FUNCTIONID); } set { set(R2Const.AT_FUNCTIONID, value); } }
            public string Name { get { return get(R2Const.AT_NAME); } set { set(R2Const.AT_NAME, value); } }
            public string Statement { get { return get(R2Const.AT_STATEMENT); } set { set(R2Const.AT_STATEMENT, value); } }
            public string Description { get { return get(R2Const.AT_DESCRIPTION); } set { set(R2Const.AT_DESCRIPTION, value); } }

            // TODO: This has to go to R2Profile/R2ProfileDefinition
            public string ProfileType { get; set; }
        }

        public class R2Criterion : R2ModelElement
        {
            public R2Criterion()
            {
                Stereotype = R2Const.ST_CRITERION;
            }

            public override string GetExtId()
            {
                return Name;
            }

            public void SetRefId(string RefAlias, string RefFunctionId, string RefCriterionId)
            {
                if (!string.IsNullOrEmpty(RefFunctionId) && !string.IsNullOrEmpty(RefCriterionId))
                {
                    RefId = string.Format("{0}#{1:00}", RefFunctionId, int.Parse(RefCriterionId));
                }
            }

            public string Name
            {
                get { return string.Format("{0}#{1:00}", get(R2Const.AT_FUNCTIONID), getDecimal(R2Const.AT_CRITSEQNO)); }
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
            public string FunctionId { get { return get(R2Const.AT_FUNCTIONID); } set { set(R2Const.AT_FUNCTIONID, value); } }
            public decimal CriterionSeqNo { get { return getDecimal(R2Const.AT_CRITSEQNO); } set { set(R2Const.AT_CRITSEQNO, value); } }
            public string Text { get { return get(R2Const.AT_TEXT); } set { set(R2Const.AT_TEXT, value); } }
            public decimal Row { get { return getDecimal(R2Const.TV_ROW); } set { set(R2Const.TV_ROW, value); } }
            public void SetRow(string raw)
            {
                string value = null;
                if (raw != null)
                {
                    value = raw.Trim();
                }
                if (string.IsNullOrEmpty(value))
                {
                    _values.Remove(R2Const.TV_ROW);
                }
                else
                {
                    Row = decimal.Parse(value);
                }
            }
            public bool Conditional { get { return getBool(R2Const.TV_CONDITIONAL); } set { set(R2Const.TV_CONDITIONAL, value); } }
            public void SetConditional(string raw)
            {
                if (raw != null)
                {
                    Conditional = "Y".Equals(raw);
                }
                else
                {
                    _values.Remove(R2Const.TV_CONDITIONAL);
                }
            }
            public bool Dependent { get { return getBool(R2Const.TV_DEPENDENT); } set { set(R2Const.TV_DEPENDENT, value); } }
            public void SetDependent(string raw)
            {
                if (raw != null)
                {
                    Dependent = "Y".Equals(raw);
                }
                else
                {
                    _values.Remove(R2Const.TV_DEPENDENT);
                }
            }
            public string Optionality { get { return get(R2Const.TV_OPTIONALITY); } set { set(R2Const.TV_OPTIONALITY, value); } }
        }
    }
}
