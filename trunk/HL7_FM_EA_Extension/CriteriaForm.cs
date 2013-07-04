using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HL7_FM_EA_Extension
{
    public partial class CriteriaForm : Form
    {
        public CriteriaForm()
        {
            InitializeComponent();
        }

        private EA.Element element;

        public void Show(EA.Element el, R2Config config)
        {
            element = el;

            BackColor = config.getSectionColor(el.Name, DefaultBackColor);
            sectionLabel.Text = config.getSectionTitle(el.Name);

            // Other properties
            int sepIdx = el.Name.IndexOf('#');
            int sepIdx2 = el.Name.IndexOf(' ', sepIdx);
            if (sepIdx2 == -1) sepIdx2 = el.Name.Length;
            string functionID = el.Name.Substring(0, sepIdx);
            functionIDTextBox.Text = functionID;
            decimal criteriaID = decimal.Parse(el.Name.Substring(sepIdx + 1, sepIdx2-sepIdx-1));
            criteriaIDNumericUpDown.Minimum = criteriaID;
            criteriaIDNumericUpDown.Maximum = criteriaID;
            criteriaIDNumericUpDown.Value = criteriaID;
            criteriaTextTextBox.Text = el.Notes;

            EA.TaggedValue tvRow = (EA.TaggedValue)el.TaggedValues.GetByName("Row");
            if (tvRow != null)
            {
                decimal row = decimal.Parse(tvRow.Value);
                rowNumericUpDown.Minimum = row;
                rowNumericUpDown.Maximum = row;
                rowNumericUpDown.Value = row;
            }

            EA.TaggedValue tvConditional = (EA.TaggedValue)el.TaggedValues.GetByName("Conditional");
            if (tvConditional != null)
            {
                conditionalCheckBox.Checked = "Y".Equals(tvConditional.Value);
            }
            EA.TaggedValue tvDependent = (EA.TaggedValue)el.TaggedValues.GetByName("Dependent");
            if (tvDependent != null)
            {
                dependentCheckBox.Checked = "Y".Equals(tvDependent.Value);
            }
            EA.TaggedValue tvOptionality = (EA.TaggedValue)el.TaggedValues.GetByName("Optionality");
            if (tvOptionality != null)
            {
                optionalityComboBox.SelectedItem = tvOptionality.Value;
            }
            ShowDialog();
        }


        private void updateTaggedValue(string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(name);
                if (tv == null)
                {
                    tv = (EA.TaggedValue)element.TaggedValues.AddNew(name, "TaggedValue");
                }
                tv.Value = value;
                tv.Update();
            }
        }

        private string getTaggedValue(string name)
        {
            EA.TaggedValue tv = (EA.TaggedValue)element.TaggedValues.GetByName(name);
            if (tv != null)
            {
                return tv.Value;
            }
            else
            {
                return "";
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
            element.Name = string.Format("{0}#{1:00}", functionIDTextBox.Text, criteriaIDNumericUpDown.Value);
            element.Notes = criteriaTextTextBox.Text;
            element.Update();
            updateTaggedValue("Row", rowNumericUpDown.Value.ToString());
            updateTaggedValue("Conditional", conditionalCheckBox.Checked ? "Y" : "N");
            updateTaggedValue("Dependent", dependentCheckBox.Checked ? "Y" : "N");
            updateTaggedValue("Optionality", optionalityComboBox.SelectedItem.ToString());
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
