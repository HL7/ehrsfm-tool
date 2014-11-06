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
    public partial class FunctionForm : Form
    {
        public FunctionForm()
        {
            InitializeComponent();
        }

        private bool ignoreEvents = false;
        private R2Function _function;

        public void Show(R2Function function)
        {
            _function = function;
            BackColor = R2Config.config.getSectionColor(_function.FunctionId, DefaultBackColor);
            setShowingFunction(function, true);
            switchLinkLabel.Visible = function.IsCompilerInstruction;
            ShowDialog();
        }

        private void setShowingFunction(R2Function function, bool enableEdit)
        {
            ignoreEvents = true;

            lockIcon.Visible = function.IsReadOnly;
            if (function.IsReadOnly)
            {
                enableEdit = false;
            }
            okButton.Enabled = enableEdit;
            applyButton.Enabled = enableEdit;

            // always show ChangeNote
            changeNoteLinkLabel.Visible = true;
            changeNoteLinkLabel.Enabled = true;
            changeNoteTextBox.Visible = false;
            changeNoteTextBox.Text = function.ChangeNote;
            changeNoteTextBox.Enabled = enableEdit;

            if (function.IsCompilerInstruction)
            {
                descriptionLinkLabel.Enabled = false;
                descriptionTextBox.Visible = true;

                Text = string.Format("EHR-S FM {0}: {1} (Profile Definition) @{2}", function.Stereotype, function.Name, function.LastModified);
                switchLinkLabel.Text = "Switch to base Element";
                priorityLabel.Visible = true;
                priorityComboBox.SelectedItem = function.Priority;
                priorityComboBox.Visible = true;

                updateLabels();
            }
            else
            {
                descriptionTextBox.Visible = true;
                descriptionLinkLabel.Enabled = false;

                Text = string.Format("EHR-S FM {0}: {1} @{2}", function.Stereotype, function.Name, function.LastModified);
                switchLinkLabel.Text = "Back to Profile Definition Element";
                priorityLabel.Visible = false;
                priorityComboBox.SelectedItem = R2Const.EmptyPriority;
                priorityComboBox.Visible = false;

                updateLabels(false);
            }
            pathLabel.Text = function.Path;

            // Other properties
            idTextBox.Text = function.FunctionId;
            nameTextBox.Text = function.Name;
            statementTextBox.Text = function.Statement;
            descriptionTextBox.Text = function.Description;

            if (enableEdit)
            {
                if (function.IsCompilerInstruction)
                {
                    idTextBox.Enabled = false;
                    bool isRealm = "Realm".Equals(function.ProfileType);
                    nameTextBox.Enabled = isRealm;
                    statementTextBox.Enabled = isRealm;
                    descriptionTextBox.Enabled = true;
                }
                else
                {
                    idTextBox.Enabled = false;
                    nameTextBox.Enabled = true;
                    statementTextBox.Enabled = true;
                    descriptionTextBox.Enabled = true;
                }
            }
            else
            {
                idTextBox.Enabled = false;
                nameTextBox.Enabled = false;
                statementTextBox.Enabled = false;
                descriptionTextBox.Enabled = false;
            }
            ignoreEvents = false;
        }

        /**
         * Append star(*) after label of Label is value is different from Default
         */ 
        private void updateLabel(string key, string labelText, Label label, bool? star)
        {
            if (star == null)
            {
                if (!_function.isDefault(key)) labelText = string.Format("{0}*", labelText);
            }
            else
            {
                if (star == true) labelText = string.Format("{0}*", labelText);
            }
            label.Text = labelText;
        }

        /**
         * Update labels for all editable elements
         */
        private void updateLabels(bool? star = null)
        {
            updateLabel(R2Const.AT_NAME, "Name", nameLabel, star);
            updateLabel(R2Const.AT_STATEMENT, "Statement", statementLabel, star);
            updateLabel(R2Const.AT_DESCRIPTION, "Description", descriptionLinkLabel, star);
            updateLabel(R2Const.TV_PRIORITY, "Priority", priorityLabel, star);
        }

        private void applyChanges()
        {
            _function.FunctionId = idTextBox.Text;
            _function.Name = nameTextBox.Text;
            _function.Statement = statementTextBox.Text;
            _function.Description = descriptionTextBox.Text;
            if (_function.IsCompilerInstruction)
            {
                _function.Priority = priorityComboBox.Text;
                _function.ChangeNote = changeNoteTextBox.Text;
            }
            _function.SaveToSource();

            idTextBox.Text = _function.FunctionId;
            nameTextBox.Text = _function.Name;
            statementTextBox.Text = _function.Statement;
            descriptionTextBox.Text = _function.Description;
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

        private void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            int spidx = nameTextBox.Text.IndexOf(' ');
            if (spidx == -1) spidx = nameTextBox.Text.Length;
            string id = nameTextBox.Text.Substring(0, spidx);
            idTextBox.Text = id;
            updateLabel(R2Const.AT_NAME, "Name", nameLabel, true);
            BackColor = R2Config.config.getSectionColor(id, DefaultBackColor);
        }

        private bool switched = false;
        private void switchLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!switched)
            {
                switched = true;
                R2Function baseFunction = (R2Function)_function.BaseElement;
                // Disable edit of base to prevent accidental changes
                setShowingFunction(baseFunction, false);
            }
            else
            {
                switched = false;
                setShowingFunction(_function, true);
            }
        }

        private void statementTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreEvents) updateLabel(R2Const.AT_STATEMENT, "Statement", statementLabel, true);
        }

        private void descriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreEvents) updateLabel(R2Const.AT_DESCRIPTION, "Description", descriptionLinkLabel, true);
        }

        private void priorityComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!ignoreEvents) updateLabel(R2Const.TV_PRIORITY, "Priority", priorityLabel, true);
        }

        private void descriptionLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            descriptionLinkLabel.Enabled = false;
            descriptionTextBox.Visible = true;
            changeNoteLinkLabel.Enabled = true;
            changeNoteTextBox.Visible = false;
        }

        private void changeNoteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            descriptionLinkLabel.Enabled = true;
            descriptionTextBox.Visible = false;
            changeNoteLinkLabel.Enabled = false;
            changeNoteTextBox.Visible = true;
        }
    }
}
