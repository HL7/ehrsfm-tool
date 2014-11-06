using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HL7_FM_EA_Extension.R2ModelV2.Base;

namespace HL7_FM_EA_Extension
{
    public partial class CriterionForm : Form
    {
        public CriterionForm()
        {
            InitializeComponent();
        }

        private bool ignoreEvents = false;
        private R2Criterion _criterion;

        public void Show(R2Criterion criterion)
        {
            _criterion = criterion;
            BackColor = R2Config.config.getSectionColor(_criterion.Name, DefaultBackColor);
            setShowingCriterion(criterion, true);
            switchLinkLabel.Visible = criterion.IsCompilerInstruction;
            ShowDialog();
        }

        /**
         * Set Criterion used to populate Components
         * and Show
         */
        private void setShowingCriterion(R2Criterion criterion, bool enableEdit)
        {
            ignoreEvents = true;

            lockIcon.Visible = criterion.IsReadOnly;
            if (criterion.IsReadOnly)
            {
                enableEdit = false;
            }
            okButton.Enabled = enableEdit;
            applyButton.Enabled = enableEdit;

            seqNoNumericUpDown.Enabled = enableEdit;
            rowNumericUpDown.Enabled = enableEdit;
            textTextBox.Enabled = enableEdit;
            optionalityComboBox.Enabled = enableEdit;
            conditionalCheckBox.Enabled = enableEdit;
            dependentCheckBox.Enabled = enableEdit;
            changeNoteTextBox.Enabled = enableEdit;

            // always show ChangeNote
            changeNoteLinkLabel.Visible = true;
            changeNoteLinkLabel.Enabled = true;
            changeNoteTextBox.Visible = false;
            changeNoteTextBox.Text = criterion.ChangeNote;
            changeNoteTextBox.Enabled = enableEdit;

            if (criterion.IsCompilerInstruction)
            {
                textLinkLabel.Enabled = false;
                textTextBox.Visible = true;

                Text = string.Format("EHR-S FM Criterion: {0} (Profile Definition) @{1}", criterion.Name, criterion.LastModified);
                switchLinkLabel.Text = "Switch to base Element";
                priorityLabel.Visible = true;
                priorityComboBox.Visible = true;
                priorityComboBox.SelectedItem = criterion.Priority;

                updateLabels();
            }
            else
            {
                textTextBox.Visible = true;
                textLinkLabel.Enabled = false;

                Text = string.Format("EHR-S FM Criterion: {0} @{1}", criterion.Name, criterion.LastModified);
                switchLinkLabel.Text = "Back to Profile Definition Element";
                priorityLabel.Visible = false;
                priorityComboBox.Visible = false;
                priorityComboBox.SelectedItem = R2Const.EmptyPriority;

                updateLabels(false);
            }
            pathLabel.Text = criterion.Path;

            seqNoNumericUpDown.Minimum = 0;
            seqNoNumericUpDown.Maximum = 99;
            rowNumericUpDown.Minimum = 0;
            rowNumericUpDown.Maximum = 10000;

            seqNoNumericUpDown.Value = criterion.CriterionSeqNo;
            textTextBox.Text = criterion.Text;
            rowNumericUpDown.Value = criterion.Row;
            conditionalCheckBox.Checked = criterion.Conditional;
            dependentCheckBox.Checked = criterion.Dependent;
            optionalityComboBox.SelectedItem = criterion.Optionality;

            ignoreEvents = false;
        }

        /**
         * Append star(*) after label of Label is value is different from Default
         */ 
        private void updateLabel(string key, string labelText, Label label, bool? star)
        {
            if (star == null)
            {
                if (!_criterion.isDefault(key)) labelText = string.Format("{0}*", labelText);
            }
            else
            {
                if (star == true) labelText = string.Format("{0}*", labelText);
            }
            label.Text = labelText;
        }

        /**
         * Append star(*) after label of Label is value is different from Default
         */
        private void updateLabel(string key, string labelText, CheckBox checkBox, bool? star)
        {
            if (star == null)
            {
                if (!_criterion.isDefault(key)) labelText = string.Format("{0}*", labelText);
            }
            else
            {
                if (star == true) labelText = string.Format("{0}*", labelText);
            }
            checkBox.Text = labelText;
        }

        /**
         * Update labels for all editable elements
         */
        private void updateLabels(bool? star = null)
        {
            updateLabel(R2Const.AT_CRITSEQNO, "Sequence#", seqNoLabel, star);
            updateLabel(R2Const.TV_ROW, "Row#", rowLabel, star);
            updateLabel(R2Const.AT_TEXT, "Text", textLinkLabel, star);
            updateLabel(R2Const.TV_CONDITIONAL, "Conditional", conditionalCheckBox, star);
            updateLabel(R2Const.TV_DEPENDENT, "Dependent", dependentCheckBox, star);
            updateLabel(R2Const.TV_OPTIONALITY, "Optionality", optionalityLabel, star);
            updateLabel(R2Const.TV_PRIORITY, "Priority", priorityLabel, star);
        }

        private void applyChanges()
        {
            _criterion.CriterionSeqNo = seqNoNumericUpDown.Value;
            _criterion.Text = textTextBox.Text;
            _criterion.Row = rowNumericUpDown.Value;
            _criterion.Conditional = conditionalCheckBox.Checked;
            _criterion.Dependent = dependentCheckBox.Checked;
            _criterion.Optionality = optionalityComboBox.SelectedItem.ToString();
            if (_criterion.IsCompilerInstruction)
            {
                _criterion.Priority = priorityComboBox.Text;
                _criterion.ChangeNote = changeNoteTextBox.Text;
            }
            _criterion.SaveToSource();

            seqNoNumericUpDown.Value = _criterion.CriterionSeqNo;
            textTextBox.Text = _criterion.Text;
            rowNumericUpDown.Value = _criterion.Row;
            conditionalCheckBox.Checked = _criterion.Conditional;
            dependentCheckBox.Checked = _criterion.Dependent;
            optionalityComboBox.SelectedItem = _criterion.Optionality;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            applyChanges();
            updateLabels();
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
            if (!ignoreEvents)
            {
                updateLabel(R2Const.AT_TEXT, "Text", textLinkLabel, true);
                updateLabel(R2Const.TV_OPTIONALITY, "Optionality", optionalityLabel, true);
            }
        }

        private bool switched = false;
        private void switchLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!switched)
            {
                switched = true;
                R2Criterion baseCriterion = (R2Criterion) _criterion.BaseElement;
                // Disable edit of base to prevent accidental changes
                setShowingCriterion(baseCriterion, false);
            }
            else
            {
                switched = false;
                setShowingCriterion(_criterion, true);
            }
        }

        private void textTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreEvents) updateLabel(R2Const.AT_TEXT, "Text", textLinkLabel, true);
        }

        private void conditionalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreEvents) updateLabel(R2Const.TV_CONDITIONAL, "Conditional", conditionalCheckBox, true);
        }

        private void dependentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!ignoreEvents) updateLabel(R2Const.TV_DEPENDENT, "Dependent", dependentCheckBox, true);
        }

        private void rowNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreEvents) updateLabel(R2Const.TV_ROW, "Row#", rowLabel, true);
        }

        private void idNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!ignoreEvents) updateLabel(R2Const.AT_CRITSEQNO, "Sequence#", seqNoLabel, true);
        }

        private void textLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textLinkLabel.Enabled = false;
            textTextBox.Visible = true;
            changeNoteLinkLabel.Enabled = true;
            changeNoteTextBox.Visible = false;
        }

        private void changeNoteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textLinkLabel.Enabled = true;
            textTextBox.Visible = false;
            changeNoteLinkLabel.Enabled = false;
            changeNoteTextBox.Visible = true;
        }
    }
}
