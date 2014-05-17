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
    public partial class CriterionForm : Form
    {
        public CriterionForm()
        {
            InitializeComponent();
        }

        private R2Criterion _criterion;

        public void Show(R2Criterion criterion)
        {
            _criterion = criterion;
            setShowingCriterion(criterion, true);
            switchLinkLabel.Visible = (criterion is CompilerInstruction);
            ShowDialog();
        }

        /**
         * Set Criterion used to populate Components
         * and Show
         */
        private void setShowingCriterion(R2Criterion criterion, bool enableEdit)
        {
            idNumericUpDown.Enabled = enableEdit;
            rowNumericUpDown.Enabled = enableEdit;
            textTextBox.Enabled = enableEdit;
            optionalityComboBox.Enabled = enableEdit;
            conditionalCheckBox.Enabled = enableEdit;
            dependentCheckBox.Enabled = enableEdit;

            if (criterion is CompilerInstruction)
            {
                Text = string.Format("EHR-S FM Criterion: {0} (Profile Definition) @{1}", criterion.Name, criterion.LastModified);
                switchLinkLabel.Text = "Switch to base Element";
                label1.Visible = true;
                priorityComboBox.Visible = true;
                priorityComboBox.SelectedItem = ((R2CriterionCI)criterion).Priority;
            }
            else
            {
                Text = string.Format("EHR-S FM Criterion: {0} @{1}", criterion.Name, criterion.LastModified);
                switchLinkLabel.Text = "Back to Profile Definition Element";
                label1.Visible = false;
                priorityComboBox.Visible = false;
                priorityComboBox.SelectedItem = R2CriterionCI.EmptyPriority;
            }

            BackColor = R2Config.config.getSectionColor(criterion.Name, DefaultBackColor);
            pathLabel.Text = criterion.Path;

            idNumericUpDown.Minimum = 0;
            idNumericUpDown.Maximum = 99;
            idNumericUpDown.Value = criterion.CriterionID;
            textTextBox.Text = criterion.Text;

            rowNumericUpDown.Minimum = 0;
            rowNumericUpDown.Maximum = 10000;
            rowNumericUpDown.Value = criterion.Row;

            conditionalCheckBox.Checked = criterion.Conditional;
            dependentCheckBox.Checked = criterion.Dependent;
            optionalityComboBox.SelectedItem = criterion.Optionality;
        }

        private void applyChanges()
        {
            _criterion.CriterionID = idNumericUpDown.Value;
            _criterion.Text = textTextBox.Text;
            _criterion.Update();

            _criterion.Row = rowNumericUpDown.Value;
            _criterion.Conditional = conditionalCheckBox.Checked;
            _criterion.Dependent = dependentCheckBox.Checked;
            _criterion.Optionality = optionalityComboBox.SelectedItem.ToString();

            if (_criterion is CompilerInstruction)
            {
                ((R2CriterionCI)_criterion).Priority = priorityComboBox.Text;
            }
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            applyChanges();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Close();
            applyChanges();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void optionalityComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string newOptionality = optionalityComboBox.SelectedItem.ToString();
            if (!string.IsNullOrEmpty(newOptionality))
            {
                string newText = textTextBox.Text.Replace("SHALL", newOptionality);
                newText = newText.Replace("SHOULD", newOptionality);
                newText = newText.Replace("MAY", newOptionality);
                textTextBox.Text = newText;
            }
        }

        private bool switched = false;
        private void switchLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!switched)
            {
                switched = true;
                R2Criterion baseCriterion = (R2Criterion) R2Model.GetBase(_criterion);
                // Disable edit of base to prevent accidental changes
                setShowingCriterion(baseCriterion, false);
            }
            else
            {
                switched = false;
                setShowingCriterion(_criterion, true);
            }
        }
    }
}
