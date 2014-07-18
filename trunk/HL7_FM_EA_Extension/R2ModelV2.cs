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
                Combined
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

            public const string EmptyPriority = "";
        }

        class Util
        {
            internal static string FormatLastModified(DateTime dateTime)
            {
                return dateTime.ToString();
            }

            internal static Dictionary<string, string> SplitNotes(string notes)
            {
                System.Text.RegularExpressions.Regex regex2 = new System.Text.RegularExpressions.Regex(@"\$([A-Z]{2})\$");
                string[] parts = regex2.Split(notes, 10);
                Dictionary<string, string> dict = new Dictionary<string, string>();
                for (int i = 1; i < parts.Length; i += 2)
                {
                    dict[parts[i]] = (parts.Length > i + 1) ? parts[i + 1] : "";
                }
                return dict;
            }
        }

        public abstract class R2ModelElement
        {
            public Object SourceObject { get; set; }
            public virtual void LoadFromSource()
            {
            }

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
                CriterionId, Text, Row, Dependent, Conditional, Optionality,
                Priority, ChangeNote
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
                    // if (!IsCompilerInstruction) throw Illegal;
                    set(PropertyName.ChangeNote, value);
                }
            }
        }

        public class R2Section : R2ModelElement
        {
            public string SectionId { get { return get(PropertyName.SectionId); } set { set(PropertyName.SectionId, value); } }
            public string Name { get { return get(PropertyName.Name); } set { set(PropertyName.Name, value); } }
            public string Overview { get { return get(PropertyName.Overview); } set { set(PropertyName.Overview, value); } }
            public string Example { get { return get(PropertyName.Example); } set { set(PropertyName.Example, value); } }
            public string Actors { get { return get(PropertyName.Actors); } set { set(PropertyName.Actors, value); } }
        }

        public class R2Function : R2ModelElement
        {
            public string FunctionId { get { return get(PropertyName.FunctionId); } set { set(PropertyName.FunctionId, value); } }
            public string Name { get { return get(PropertyName.Name); } set { set(PropertyName.Name, value); } }
            public string Statement { get { return get(PropertyName.Statement); } set { set(PropertyName.Statement, value); } }
            public string Description { get { return get(PropertyName.Description); } set { set(PropertyName.Description, value); } }

            // TODO: This has to go to R2Profile/R2ProfileDefinition
            public string ProfileType { get; set; }
        }

        public class R2Criterion : R2ModelElement
        {
            public string Name
            {
                get { return string.Format("{0}#{1:00}", get(PropertyName.FunctionId), getDecimal(PropertyName.CriterionId)); }
                set
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
                        CriterionId = 0;
                    }
                    else
                    {
                        int sepIdx2 = value.IndexOf(' ', sepIdx);
                        if (sepIdx2 == -1) sepIdx2 = value.Length;
                        CriterionId = decimal.Parse(value.Substring(sepIdx + 1, sepIdx2 - sepIdx - 1));
                    }
                }
            }
            public string FunctionId { get { return get(PropertyName.FunctionId); } set { set(PropertyName.FunctionId, value); } }
            public decimal CriterionId { get { return getDecimal(PropertyName.CriterionId); } set { set(PropertyName.CriterionId, value); } }
            public string Text { get { return get(PropertyName.Text); } set { set(PropertyName.Text, value); } }
            public decimal Row { get { return getDecimal(PropertyName.Row); } set { set(PropertyName.Row, value); } }
            public bool Conditional { get { return getBool(PropertyName.Conditional); } set { set(PropertyName.Conditional, value); } }
            public bool Dependent { get { return getBool(PropertyName.Dependent); } set { set(PropertyName.Dependent, value); } }
            public string Optionality { get { return get(PropertyName.Optionality); } set { set(PropertyName.Optionality, value); } }
        }
    }
}
