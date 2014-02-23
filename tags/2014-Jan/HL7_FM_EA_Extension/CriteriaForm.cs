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

        public void Show(EA.Element el, string path, R2Config config)
        {
            element = el;

            BackColor = config.getSectionColor(el.Name, DefaultBackColor);
            Text = string.Format("EHR-S FM Criteria: {0}", el.Name);
            pathLabel.Text = path;

            // Other properties
            int sepIdx = el.Name.IndexOf('#');
            int sepIdx2 = el.Name.IndexOf(' ', sepIdx);
            if (sepIdx2 == -1) sepIdx2 = el.Name.Length;
            decimal criteriaID = decimal.Parse(el.Name.Substring(sepIdx + 1, sepIdx2-sepIdx-1));
            criteriaIDNumericUpDown.Minimum = 1;
            criteriaIDNumericUpDown.Maximum = 99;
            criteriaIDNumericUpDown.Value = criteriaID;
            criteriaTextTextBox.Text = el.Notes;

            EA.TaggedValue tvRow = (EA.TaggedValue)el.TaggedValues.GetByName("Row");
            if (tvRow != null)
            {
                decimal row = decimal.Parse(tvRow.Value);
                rowNumericUpDown.Minimum = 1;
                rowNumericUpDown.Maximum = 10000;
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
            int sepIdx = element.Name.IndexOf('#');
            string functionID = element.Name.Substring(0, sepIdx);
            element.Name = string.Format("{0}#{1:00}", functionID, criteriaIDNumericUpDown.Value);
            element.Notes = criteriaTextTextBox.Text;
            element.Update();
            updateTaggedValue("Row", rowNumericUpDown.Value.ToString());
            updateTaggedValue("Conditional", conditionalCheckBox.Checked ? "Y" : "N");
            updateTaggedValue("Dependent", dependentCheckBox.Checked ? "Y" : "N");
            updateTaggedValue("Optionality", optionalityComboBox.SelectedItem.ToString());
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void optionalityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string newOptionality = optionalityComboBox.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(newOptionality))
            {
                string newText = criteriaTextTextBox.Text.Replace("SHALL", newOptionality);
                newText = newText.Replace("SHOULD", newOptionality);
                newText = newText.Replace("MAY", newOptionality);
                criteriaTextTextBox.Text = newText;
            }
        }
    }
}
