using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EA;

namespace HL7_FM_EA_Extension
{
    /**
     * This model represents the EHR-S FM R2 UML Profile
     * and wraps all the EA Model classes
     */
    public class R2Model
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
                    EA.Connector generalization = element.Connectors.Cast<EA.Connector>().SingleOrDefault(t => "Generalization".Equals(t.Type));
                    if (generalization == null)
                    {
                        MessageBox.Show("Generalization to Base Element missing");
                        return null;
                    }
                    EA.Element baseElement = repository.GetElementByID(generalization.SupplierID);
                    switch(baseElement.Stereotype)
                    {
                        case R2Const.ST_SECTION:
                            modelElement = new R2SectionCI(element, baseElement);
                            break;
                        case R2Const.ST_HEADER:
                        case R2Const.ST_FUNCTION:
                            modelElement = new R2FunctionCI(element, baseElement);
                            break;
                        case R2Const.ST_CRITERION:
                            modelElement = new R2CriterionCI(element, baseElement);
                            break;
                    }
                    break;
            }
            if (modelElement != null && repository != null)
            {
                modelElement.Path = getModelElementPath(repository, element);
            }
            return modelElement;
        }

        public static EA.Element findCompilerInstruction(EA.Repository repository, EA.Element element)
        {
            // Find compilerinstruction by looking for the generalization connector that points
            // to an Element with stereotype Compiler Instruction
            foreach (EA.Connector con in element.Connectors.Cast<EA.Connector>().Where(c => "Generalization".Equals(c.Type)))
            {
                EA.Element _element = (EA.Element)repository.GetElementByID(con.ClientID);
                if (R2Const.ST_COMPILERINSTRUCTION.Equals(_element.Stereotype))
                {
                    return _element;
                }
            }
            return null;
        }

        /**
         * Return the (normal) Model Element from a Compiler Instruction
         */
        public static R2ModelElement GetBase(R2ModelElement ci)
        {
            if (ci is CompilerInstruction)
            {
                // Repository is not used here
                R2ModelElement baseElement = Create(null, ci._element);
                baseElement.Path = ci.Path;
                return baseElement;
            }
            else
            {
                return null;
            }
        }

        /**
         * Create a string containing the element path joined with '/' up to the <HL7-FM> stereotyped package.
         * This is used for as title for Section/Header/Function/Criteria Forms.
         */
        public static string getModelElementPath(EA.Repository repository, EA.Element element)
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

    public class R2Const
    {
        public const string ST_FM = "HL7-FM";
        public const string ST_FM_PROFILEDEFINITION = "HL7-FM-ProfileDefinition";
        public const string ST_COMPILERINSTRUCTION = "CI";
        public const string ST_BASEMODEL = "use";
        public const string ST_TARGETPROFILE = "create";
        public const string ST_FM_PROFILE = "HL7-FM-Profile";
        public const string ST_SECTION = "Section";
        public const string ST_HEADER = "Header";
        public const string ST_FUNCTION = "Function";
        public const string ST_CONSEQUENCELINK = "ConsequenceLink";
        public const string ST_SEEALSO = "SeeAlso";

        public const string TV_PRIORITY = "Priority";
        public const string TV_CHANGENOTE = "ChangeNote";
        public const string TV_QUALIFIER = "Qualifier";
        public const string TV_ROW = "Row";

        public const string ST_CRITERIA__OBSOLETE = "Criteria";
        public const string ST_CRITERION = "Criteria";
        public const string TV_OPTIONALITY = "Optionality";
        public const string TV_DEPENDENT = "Dependent";
        public const string TV_CONDITIONAL = "Conditional";

        public enum ProfileTypes
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
            EF      // Essential Future
        };
    }

    public interface CompilerInstruction
    {
        // string Priority;
        // string ChangeNote;
        // string Qualifier;
    }

    public abstract class R2ModelElement
    {
        internal string _path;
        internal EA.Element _element;

        public R2ModelElement(EA.Element element)
        {
            _element = element;
        }

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        public string Stereotype
        {
            get { return _element.Stereotype; }
        }

        public void Update()
        {
            _element.Update();
        }

        internal Dictionary<string, string> splitNotes(string notes)
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

    public class R2Section : R2ModelElement
    {
        public R2Section(EA.Element element) : base(element)
        {
            string notes = element.Notes;
            Dictionary<string, string> noteParts = splitNotes(notes);
            _overview = noteParts.ContainsKey("OV") ? noteParts["OV"] : "";
            _example = noteParts.ContainsKey("EX") ? noteParts["EX"] : "";
            _actors = noteParts.ContainsKey("AC") ? noteParts["AC"] : "";
        }

        private void updateElementNotes()
        {
            string notes = string.Format("$OV${0}$EX${1}$AC${2}", _overview, _example, _actors);
            _element.Notes = notes;
        }

        private string _overview;
        private string _example;
        private string _actors;

        public string Name
        {
            get { return _element.Name; }
            set { _element.Name = value; }
        }

        public string SectionID
        {
            get { return _element.Alias; }
            set { _element.Alias = value; }
        }

        public string Overview
        {
            get { return _overview; }
            set { _overview = value; updateElementNotes(); }
        }

        public string Example
        {
            get { return _example; }
            set { _example = value; updateElementNotes(); }
        }

        public string Actors
        {
            get { return _actors; }
            set { _actors = value; updateElementNotes(); }
        }
    }

    public class R2Function : R2ModelElement
    {
        public R2Function(EA.Element element) : base(element)
        {
            string notes = element.Notes;
            Dictionary<string, string> noteParts = splitNotes(notes);
            _statement = noteParts.ContainsKey("ST") ? noteParts["ST"] : "";
            _description = noteParts.ContainsKey("DE") ? noteParts["DE"] : "";
            _example = noteParts.ContainsKey("EX") ? noteParts["EX"] : "";
        }

        private void updateElementNotes()
        {
            string notes = string.Format("$ST${0}$DE${1}$EX${2}", _statement, _description, _example);
            _element.Notes = notes;
        }

        private string _statement;
        private string _description;
        private string _example;

        public string Name
        {
            get { return _element.Name; }
            set { _element.Name = value; }
        }

        public string FunctionID
        {
            get { return _element.Alias; }
            set { _element.Alias = value; }
        }

        public string Statement
        {
            get { return _statement; }
            set { _statement = value; updateElementNotes(); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; updateElementNotes(); }
        }

        public string Example
        {
            get { return _example; }
            set { _example = value; updateElementNotes(); }
        }
    }

    public class R2Criterion : R2ModelElement
    {
        public R2Criterion(EA.Element element) : base(element)
        {
        }

        public virtual string Name
        {
            get { return _element.Name; }
        }

        public virtual string FunctionID
        {
            get
            {
                int sepIdx = _element.Name.IndexOf('#');
                return _element.Name.Substring(0, sepIdx);
            }
        }

        public virtual decimal CriterionID
        {
            get
            {
                int sepIdx = _element.Name.IndexOf('#');
                // If the name doesnot contain a '#' yet, assume 1
                if (sepIdx == -1)
                {
                    return 1;
                }
                else
                {
                    int sepIdx2 = _element.Name.IndexOf(' ', sepIdx);
                    if (sepIdx2 == -1) sepIdx2 = _element.Name.Length;
                    return decimal.Parse(_element.Name.Substring(sepIdx + 1, sepIdx2 - sepIdx - 1));
                }
            }
            set
            {
                int sepIdx = _element.Name.IndexOf('#');
                if (sepIdx == -1) sepIdx = _element.Name.Length;
                string functionID = _element.Name.Substring(0, sepIdx);
                _element.Name = string.Format("{0}#{1:00}", functionID, value);
            }
        }

        public virtual string Text
        {
            get { return _element.Notes; }
            set { _element.Notes = value; }
        }

        public virtual decimal Row
        {
            get
            {
                string value = EAHelper.getTaggedValue(_element, R2Const.TV_ROW).Trim();
                if (string.IsNullOrEmpty(value))
                {
                    value = "0";
                }
                return decimal.Parse(value);
            }
            set { EAHelper.updateTaggedValue(_element, R2Const.TV_ROW, value.ToString()); }
        }

        public virtual bool Conditional
        {
            get { return "Y".Equals(EAHelper.getTaggedValue(_element, R2Const.TV_CONDITIONAL, "N")); }
            set { EAHelper.updateTaggedValue(_element, R2Const.TV_CONDITIONAL, value ? "Y" : "N"); }
        }

        public virtual bool Dependent
        {
            get { return "Y".Equals(EAHelper.getTaggedValue(_element, R2Const.TV_DEPENDENT, "N")); }
            set { EAHelper.updateTaggedValue(_element, R2Const.TV_DEPENDENT, value ? "Y" : "N"); }
        }            

        // Don't use Enum
        // We know Optionality can be extended in profiles
        public virtual string Optionality
        {
            get { return EAHelper.getTaggedValue(_element, R2Const.TV_OPTIONALITY, ""); }
            set { EAHelper.updateTaggedValue(_element, R2Const.TV_OPTIONALITY, value); }
        }
    }

    /**
     * Changes are recorded in the ciElement.
     */
    public class R2CriterionCI : R2Criterion, CompilerInstruction
    {
        private EA.Element _ciElement;

        public R2CriterionCI(EA.Element ciElement, EA.Element element) : base(element)
        {
            _ciElement = ciElement;
        }

        public override string Name
        {
            get { return _ciElement.Name; }
        }

        public override string FunctionID
        {
            get
            {
                int sepIdx = _ciElement.Name.IndexOf('#');
                return _ciElement.Name.Substring(0, sepIdx);
            }
        }

        public override decimal CriterionID
        {
            get
            {
                int sepIdx = _ciElement.Name.IndexOf('#');
                int sepIdx2 = _ciElement.Name.IndexOf(' ', sepIdx);
                if (sepIdx2 == -1) sepIdx2 = _ciElement.Name.Length;
                return decimal.Parse(_ciElement.Name.Substring(sepIdx + 1, sepIdx2 - sepIdx - 1));
            }
            set
            {
                int sepIdx = _ciElement.Name.IndexOf('#');
                string functionID = _ciElement.Name.Substring(0, sepIdx);
                _ciElement.Name = string.Format("{0}#{1:00}", functionID, value);
            }
        }

        public override string Text
        {
            get
            {
                if (string.IsNullOrEmpty(_ciElement.Notes))
                {
                    return base.Text;
                }
                else
                {
                    return _ciElement.Notes;
                }
            }
            set
            {
                if (value.Equals(base.Text))
                {
                    _ciElement.Notes = "";
                }
                else
                {
                    _ciElement.Notes = value;
                }
                _ciElement.Update();
            }
        }

        public override decimal Row
        {
            get
            {
                string value = EAHelper.getTaggedValue(_ciElement, R2Const.TV_ROW).Trim();
                if (string.IsNullOrEmpty(value))
                {
                    value = base.Row.ToString();
                }
                return decimal.Parse(value);
            }
            set
            {
                if (value.Equals(base.Row))
                {
                    EAHelper.deleteTaggedValue(_ciElement, R2Const.TV_ROW);
                }
                else
                {
                    EAHelper.updateTaggedValue(_ciElement, R2Const.TV_ROW, value.ToString());
                }
            }
        }

        public override bool Conditional
        {
            get
            {
                string conditionalValue = EAHelper.getTaggedValue(_ciElement, R2Const.TV_CONDITIONAL, null);
                if (conditionalValue == null)
                {
                    return base.Conditional;
                }
                else
                {
                    return "Y".Equals(conditionalValue);
                }
            }
            set
            {
                if (value.Equals(base.Conditional))
                {
                    EAHelper.deleteTaggedValue(_ciElement, R2Const.TV_CONDITIONAL);
                }
                else
                {
                    EAHelper.updateTaggedValue(_ciElement, R2Const.TV_CONDITIONAL, value ? "Y" : "N");
                }
            }
        }

        public override bool Dependent
        {
            get
            {
                string dependentValue = EAHelper.getTaggedValue(_ciElement, R2Const.TV_DEPENDENT, null);
                if (dependentValue == null)
                {
                    return base.Dependent;
                }
                else
                {
                    return "Y".Equals(dependentValue);
                }
            }
            set
            {
                if (value.Equals(base.Dependent))
                {
                    EAHelper.deleteTaggedValue(_ciElement, R2Const.TV_DEPENDENT);
                }
                else
                {
                    EAHelper.updateTaggedValue(_ciElement, R2Const.TV_DEPENDENT, value ? "Y" : "N");
                }
            }
        }

        public override string Optionality
        {
            get
            {
                return EAHelper.getTaggedValue(_ciElement, R2Const.TV_OPTIONALITY, base.Optionality);
            }
            set
            {
                if (value.Equals(base.Optionality))
                {
                    EAHelper.deleteTaggedValue(_ciElement, R2Const.TV_OPTIONALITY);
                }
                else
                {
                    EAHelper.updateTaggedValue(_ciElement, R2Const.TV_OPTIONALITY, value);
                }
            }
        }
    }

    /**
     * Changes are recorded in the ciElement.
     */
    public class R2FunctionCI : R2Function, CompilerInstruction
    {
        private EA.Element _ciElement;

        public R2FunctionCI(EA.Element ciElement, EA.Element element) : base(element)
        {
            _ciElement = ciElement;
        }
    }

    /**
     * Changes are recorded in the ciElement.
     */
    public class R2SectionCI : R2Section, CompilerInstruction
    {
        private EA.Element _ciElement;

        public R2SectionCI(EA.Element ciElement, EA.Element element): base(element)
        {
            _ciElement = ciElement;
        }
    }
}
